using System;

namespace ConsoleExcuteCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoExcute();
            Console.ReadKey();
        }

        /// <summary>
        /// 
        /// </summary>
        static void AutoExcute()
        {
            System.Timers.Timer t = new System.Timers.Timer(3000);//实例化Timer类，设置间隔时间为10000毫秒；
            t.Elapsed += new System.Timers.ElapsedEventHandler(Execute);//到达时间的时候执行事件；
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            t.Start(); //启动定时器
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        static void Execute(object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine($"Begin - {DateTime.Now.ToString()}");
            cmdOutput();
            Console.WriteLine($"End - {DateTime.Now.ToString()}");
        }

        static void cmdOutput()
        {
            //string str = Console.ReadLine();
            var str = "taskkill /s 192.168.1.38 /u AD005\\z0044t5x /p AD005\\@Xx12345678 /im H.Infrastructure.Container.Process.exe /f";
            //str = "taskkill /s 192.168.1.37 /u AD005\\z0044t5x /p AD005\\@Xx12345678 /im Teams.exe /f";

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(str);

            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

            //获取cmd窗口的输出信息
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();//等待程序执行完退出进程
            p.Close();

            Console.WriteLine(output);
            Console.WriteLine("*******************************************************************");
        }
    }
}
