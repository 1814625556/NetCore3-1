using System;
using System.Diagnostics;
using System.Linq;

namespace OtherKnowledge
{
    public class ProcessDemo
    {
        public static void FindAllProcesses()
        {


           
            SocketClient.TestStartAndSendInfo();
            

            //foreach(var pro in processList)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.BackgroundColor = ConsoleColor.Black;

            //    //这里有个误区，进程名 不带后缀名
            //    if (pro.ProcessName.Contains("Test01bak"))
            //    {
            //        Console.WriteLine(pro.ProcessName);
            //        pro.Kill();

            //    }
            //}

        }

        public static void KillTest01bak()
        {
            var processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                if (p.ProcessName == "Test01bak")
                {
                    p.Kill();
                    break;
                }
            }
        }
    }
}
