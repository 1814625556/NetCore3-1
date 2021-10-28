using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OtherKnowledge
{
    public class SocketClient
    {
        public static void client()
        {
            Process.Start(@"D:\Files\3dqt\Test01bak.exe");

            int port = 5010;
            //string host = "192.168.0.117";
            ////string host = "192.168.0.108";
            string host = "127.0.0.1";
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口转化为IPEndPoint实例  

            Socket c = null;
            c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket
            c.ReceiveTimeout = 3000;//通讯超时          
            c.Connect(ipe);//连接到服务器   
            ;
            bool flag = false;
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            string sendStr = "";
            string temp = "{\"Method\":\"SetModel\",\"Parameter\":{\"Model_list\":[{\"Path\":\"axial_area.mhd\",\"R\":50,\"G\":100,\"B\":150,\"Aplfa\":200,\"Type\":1},{\"Path\":\"result_cancer.mhd\",\"R\":50,\"G\":100,\"B\":150,\"Aplfa\":200,\"Type\":3}]}}";
            while (true)
            {
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                        sendStr = "{\"method\":\"setWindow\",\"parameters\":{\"height\":1022,\"left\":773,\"top\":30,\"width\":712,\"IsHidden\":1}}";
                        break;
                    case ConsoleKey.D2:
                        sendStr = "{\"method\":\"setWindow\",\"parameters\":{\"height\":1022,\"left\":773,\"top\":30,\"width\":712,\"IsHidden\":0}}";
                        break;
                    case ConsoleKey.D3:
                        sendStr = "{\"method\":\"setWindow\",\"parameters\":{\"height\":480,\"left\":773,\"top\":552,\"width\":712,\"IsHidden\":1}}";
                        break;
                    case ConsoleKey.D4:
                        sendStr = "{\"method\":\"setWindow\",\"parameters\":{\"height\":480,\"left\":773,\"top\":552,\"width\":712,\"IsHidden\":0}}";
                        break;
                    case ConsoleKey.D5:
                        sendStr = temp;
                        break;
                }

                byte[] bs = Encoding.ASCII.GetBytes(sendStr);
                c.Send(bs, bs.Length, 0);//发送测试信息

                byte[] recvBytes = new byte[1024];
                var bytes = c.Receive(recvBytes, recvBytes.Length, 0);//从服务器端接受返回信息 
                var recvStr = Encoding.ASCII.GetString(recvBytes, 0, bytes);

                Console.WriteLine(recvStr);
                keyInfo = Console.ReadKey();
            }

            c.Disconnect(true);
        }


        /// <summary>
        /// Test01bak如果 已经启动那么 kill ,重新启动，如果未启动 启动 等待 ，发送socket命令
        /// </summary>
        public static void TestStartAndSendInfo()
        {
            var processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                if (p.ProcessName == "Test01bak")
                {
                    p.Kill();
                    Thread.Sleep(100);
                    break;
                }
            }

            Process.Start(@"D:\Files\3dqt\Test01bak.exe");
            var i = 0;
            while (true)
            {
                Thread.Sleep(100);
                i++;
                var isBreak = false;
                processes = Process.GetProcesses();
                foreach (var p in processes)
                {
                    if (p.ProcessName == "Test01bak")
                        isBreak = true;
                }
                if (isBreak)
                {
                    Console.WriteLine($"wait for {i * 100} ms");
                    break;
                };
            }

            int port = 5010;
            string host = "127.0.0.1";
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口转化为IPEndPoint实例  

            Socket c = null;
            c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket
            c.ReceiveTimeout = 3000;//通讯超时          
            c.Connect(ipe);//连接到服务器   
            ;
            bool flag = false;
            string sendStr = "{\"method\":\"setWindow\",\"parameters\":{\"height\":1022,\"left\":773,\"top\":30,\"width\":712,\"IsHidden\":0}}";

            byte[] bs = Encoding.ASCII.GetBytes(sendStr);
            c.Send(bs, bs.Length, 0);//发送测试信息
            byte[] recvBytes = new byte[1024];
            var bytes = c.Receive(recvBytes, recvBytes.Length, 0);//从服务器端接受返回信息 
            var recvStr = Encoding.ASCII.GetString(recvBytes, 0, bytes);
            Console.WriteLine(recvStr);

            c.Disconnect(true);
        }
    }
}
