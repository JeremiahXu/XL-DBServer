using DAL;
using DBServer.DAL.HardwareSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace DBServer
{
    public class Program
    {

#if !DEBUG
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ServiceSQLite()
            };
            ServiceBase.Run(ServicesToRun);
        } 
#endif


        public static Thread thread = null;

#if DEBUG

        static void Main(string[] args)
        {
            thread = new Thread(new ThreadStart(() => ThreadRun()));
            thread.IsBackground = true;
            thread.Start();

            TcpListenerSocket tcpListenerSocket = new TcpListenerSocket(12000);
            tcpListenerSocket.Start();
            Console.WriteLine($"tcpListenerSocket.Start().");

            Console.WriteLine($"Press any key to stop server.");
            Console.ReadLine();
            tcpListenerSocket.Stop();
            //Console.ReadLine();
        }

#endif


        private static void ThreadRun()
        {

        }
    }
}
