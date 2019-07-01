using DBServer.DAL.HardwareSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace DBServer
{
    partial class ServiceSQLite : ServiceBase
    {
        public ServiceSQLite()
        {
            InitializeComponent();
        }


        Thread thread = null;
        TcpListenerSocket tcpListenerSocket = null;
        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            //thread = new Thread(new ThreadStart(() => ThreadRun()));
            //thread.IsBackground = true;
            //thread.Start();

            tcpListenerSocket = new TcpListenerSocket(12000);
            tcpListenerSocket.Start();
            Console.WriteLine($"tcpListenerSocket.Start().");
            Console.WriteLine($"Press any key to stop server.");
            Console.ReadLine();
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。

            tcpListenerSocket.Stop();
        }
    }
}
