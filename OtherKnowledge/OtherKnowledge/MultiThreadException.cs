using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OtherKnowledge
{
    public class MultiThreadException
    {
        public static void DoWork()
        {
            throw new Exception("dowork throw ex");
        }

        //这种方式才能处理异常
        //    try 
        //    {
        //        var th = new Thread(MultiThreadException.DoWorkHandleException);
        //        th.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        public static void DoWorkHandleException()
        {
            try
            {
                throw new Exception("dowork throw ex");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// task的异常处理-这里需要使用 whenall方法
        /// </summary>
        public static async void TaskException()
        {
            Task task1 = Task.Run(() => { Thread.Sleep(100); throw new Exception("ex 1"); });
            Task task2 = Task.Run(() => { Thread.Sleep(200); throw new Exception("ex 2"); });

            Task all = Task.WhenAll(task1, task2);

            try
            {
                await all;
            }
            catch
            {
                Console.WriteLine(all.Exception.InnerExceptions.Count);
            }
        }

        /// <summary>
        /// 测试task任务时间,这种模式需要5秒钟
        /// </summary>
        /// <returns></returns>
        public static async Task WhenAllTaskTimeTest()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var t1 = Task.Run(() => { Thread.Sleep(1000); return "t1"; });
            var t2 = Task.Run(() => { Thread.Sleep(2000); return "t2"; });
            var t3 = Task.Run(() => { Thread.Sleep(3000); return "t3"; });
            var t4 = Task.Run(() => { Thread.Sleep(4000); return "t4"; });
            var t5 = Task.Run(() => { Thread.Sleep(5000); return "t5"; });
            var rs = await Task.WhenAll(t1, t2, t3, t4, t5);

            Console.WriteLine("WhenAll result：");
            foreach (var r in rs)
            {
                Console.WriteLine($"    {r}");
            }
            stopWatch.Stop();
            Console.WriteLine($"耗费时间：{stopWatch.Elapsed}");
        }

        //这种模式只需要一秒钟
        public static async Task WhenAnyTaskTimeTest()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var t1 = Task.Run(() => { Thread.Sleep(1000); return "t1"; });
            var t2 = Task.Run(() => { Thread.Sleep(2000); return "t2"; });
            var t3 = Task.Run(() => { Thread.Sleep(3000); return "t3"; });
            var t4 = Task.Run(() => { Thread.Sleep(4000); return "t4"; });
            var t5 = Task.Run(() => { Thread.Sleep(5000); return "t5"; });

            var rs = await Task.WhenAny(t1, t2, t3, t4, t5);
            Console.WriteLine($"whenAny result :{await rs}");

            stopWatch.Stop();
            Console.WriteLine($"耗费时间：{stopWatch.Elapsed}");
        }

        //需要十五秒钟
        public static async Task AwaitTask()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var t1 = await Task.Run(() => { Thread.Sleep(1000); return "t1"; });
            var t2 = await Task.Run(() => { Thread.Sleep(2000); return "t2"; });
            var t3 = await Task.Run(() => { Thread.Sleep(3000); return "t3"; });
            var t4 = await Task.Run(() => { Thread.Sleep(4000); return "t4"; });
            var t5 = await Task.Run(() => { Thread.Sleep(5000); return "t5"; });

            stopWatch.Stop();
            Console.WriteLine($"await task result :{t1},{t2},{t3},{t4},{t5}");
            Console.WriteLine($"AwaitTask 耗费时间：{stopWatch.Elapsed}");
        }

    }
}
