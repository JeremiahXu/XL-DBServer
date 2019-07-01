using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DBServer.DAL.HardwareSocket
{
    public class TcpListenerSocket
    {
        readonly int LOCAL_PORT = 13000;
        TcpListener listener = null;
        readonly string path = @"C:\LogText.txt";

        public TcpListenerSocket(int port = 13000)
        {
            LOCAL_PORT = port;
            listener = new TcpListener(IPAddress.Any, LOCAL_PORT);
            path = $"{AppDomain.CurrentDomain.BaseDirectory}LogText.txt";
        }
        ~TcpListenerSocket()
        {

        }

        public void Start()
        {
            listener.Start();
            File.AppendAllLines(path, new string[] { $"{DateTime.Now},正在监听:{ IPAddress.Any}:{LOCAL_PORT.ToString()}" });
            listener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), listener);
            //Thread.Sleep(1000);
        }

        public void Stop()
        {
            listener.Stop();
            File.AppendAllLines(path, new string[] { $"{DateTime.Now},结束监听:{ IPAddress.Any}:{LOCAL_PORT.ToString()}" });
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            TcpListener lstn = (TcpListener)ar.AsyncState;
            TcpClient client = null;
            try
            {
                client = lstn.EndAcceptTcpClient(ar);
            }
            catch (ObjectDisposedException)
            {
                File.AppendAllLines(path, new string[] { $"{DateTime.Now},listener.Stop()" });
                //throw;
            }

            if (client == null)
            {
                return;
            }

            Task.Factory.StartNew(() =>
            {
                //Thread.Sleep(1000);
                string host = client.Client.RemoteEndPoint.ToString();

                // Buffer for reading data
                ;
                String message = null;

                using (NetworkStream stream = client.GetStream())
                {
                    int cutStrLength = 80;
                    //int lenth = 0;
                    //int ii;
                    // Loop to receive all the data sent by the client.

                    while (!stream.CanRead)
                    {
                        Thread.Sleep(10);
                    }

                    Byte[] Byslength = new byte[4];
                    int i1 = stream.Read(Byslength, 0, Byslength.Length);

                    int datalength = System.BitConverter.ToInt32(Byslength, 0);
                    datalength += 10;

                    Byte[] bytes = new byte[datalength];

                    int tmplength = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        int readlength = stream.Read(bytes, tmplength, datalength - tmplength);
                        tmplength += readlength;
                        if (tmplength == datalength - 10)
                        {
                            break;
                        }
                        else
                        {//数据不完整
                            Thread.Sleep(5);
                        }
                    }

                    // Translate data bytes to a ASCII string.
                    message = Encoding.UTF8.GetString(bytes, 0, tmplength);
                    var dfsf = message.Length;
                    //txtRecMsgs.Invoke(new Action(() => txtRecMsgs.AppendText(string.Format("Received: {host}的消息：{data}\n", , ))));
                    string strWrite = "";
                    if (message.Length > cutStrLength)
                    {
                        strWrite = message.Substring(0, cutStrLength);
                    }
                    else
                    {
                        strWrite = message;
                    }
                    File.AppendAllLines(path, new string[] { $"{DateTime.Now.ToLongTimeString()}:Received: {host}的消息：{strWrite} ." });
                    // Process the data sent by the client.
                    message = ProcessData(message);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(message);

                    int len = data.Length;

                    byte[] l_byteArray = System.BitConverter.GetBytes(len);

                    stream.Write(l_byteArray, 0, l_byteArray.Length);

                    // Send back a response.
                    stream.Write(data, 0, data.Length);
                    //txtRecMsgs.Invoke(new Action(() => txtRecMsgs.AppendText(string.Format("Sent: {host}的消息：{data}\n", , ))));

                    if (message.Length > cutStrLength)
                    {
                        strWrite = message.Substring(0, cutStrLength);
                    }
                    else
                    {
                        strWrite = message;
                    }
                    File.AppendAllLines(path, new string[] { $"{DateTime.Now.ToLongTimeString()}:Sent: {host}的消息：{strWrite} ." });
                    //}
                }
                // Shutdown and end connection
                client.Close();
            });
            lstn.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), lstn);
        }

        private string ProcessData(string data)
        {

            context context = JsonConvert.DeserializeObject<context>(data);

            if (context.Mode == "Post")
            {

                if (context.Function == "Insertreport")
                {

                    DataModel dm = JsonConvert.DeserializeObject<DataModel>(context.Content.ToString());
                    //var dm = context.Content as DataModel;
                    bool res = false;
                    try
                    {
                        ReportDal rd = new ReportDal();
                        res = rd.Insertreport(dm.name, dm.content, dm.path, dm.timeCreate, dm.timeModify, dm.timeAccess);
                    }
                    catch (Exception e)
                    {
                        File.AppendAllLines(path, new string[] { $"{DateTime.Now.ToLongTimeString()}:操作数据库 Insertreport 异常:{e.ToString()} ." });
                       //throw;
                    }

                    context.Content = res ? "Success" : "Failure";

                    string restr = JsonConvert.SerializeObject(context);
                    return restr;
                }

                return "";
            }

            if (context.Mode == "Get")
            {

                if (context.Function == "GetReportSingleContent")
                {
                    var name = context.Content as string;
                    //object res = null;
                    byte[] bytes = null;
                    try
                    {
                        ReportDal rd = new ReportDal();
                        var res = rd.GetReportSingleContent(name);
                        bytes = res as byte[];
                    }
                    catch (Exception e)
                    {
                        File.AppendAllLines(path, new string[] { $"{DateTime.Now.ToLongTimeString()}:操作数据库 GetReportSingleContent 异常:{e.ToString()} ." });
                        //throw;
                    }
                    string restr2 = JsonConvert.SerializeObject(bytes);
                    context.Content = restr2;
                    string restr = JsonConvert.SerializeObject(context);
                    return restr;
                }
                else if (context.Function == "GetReportSingle")
                {
                    var name = context.Content as string;
                    DataSet res = null;
                    try
                    {
                        ReportDal rd = new ReportDal();
                        res = rd.GetReportSingle(name);
                    }
                    catch (Exception e)
                    {
                        File.AppendAllLines(path, new string[] { $"{DateTime.Now.ToLongTimeString()}:操作数据库 GetReportSingle 异常:{e.ToString()} ." });
                        //throw;
                    }
                    string restr2 = JsonConvert.SerializeObject(res);
                    context.Content = restr2;
                    string restr = JsonConvert.SerializeObject(context);
                    return restr;
                }
                else if (context.Function == "GetreportLimitN")
                {
                    var dm = Convert.ToInt32(context.Content);
                    DataSet res = null;
                    try
                    {
                        ReportDal rd = new ReportDal();
                        res = rd.GetreportLimitN(dm);
                    }
                    catch (Exception e)
                    {
                        File.AppendAllLines(path, new string[] { $"{DateTime.Now.ToLongTimeString()}:操作数据库 GetreportLimitN 异常:{e.ToString()} ." });
                        //throw;
                    }
                    string restr2 = JsonConvert.SerializeObject(res);
                    context.Content = restr2;
                    string restr = JsonConvert.SerializeObject(context);
                    return restr;
                }
                return "";
            }

            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class context
    {
        public string Function { get; set; }
        public string Mode { get; set; }
        public object Content { get; set; }
    }
}
