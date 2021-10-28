using Rx.ExtensionHelper;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rx
{
    public class HardOperatesTest
    {
        public static IObservable<string> GetObservable(int valNum = 10, int duetime = 100, bool isCompleted = true)
        {
            return Observable.Create<string>(o =>
            {
                for (var i = 0; i < valNum; i++)
                {
                    o.OnNext($"{i}");
                    Thread.Sleep(duetime);
                }
                if (isCompleted)
                    o.OnCompleted();
                //return Disposable.Create(() => Console.WriteLine("dispose"));
                return Disposable.Empty;
            });
        }

        public static void Test()
        {
            TimerDesignTest();
        }

        public static void TimerDesignTest()
        {
            var observable = Observable.Generate
                (
                0,
                x => { Console.WriteLine($"condition:{x < 10}"); return x < 3; },
                x => { Console.WriteLine($"iterate:"); return x + 1; },
                x => $"rs {x}",
                x => { Console.WriteLine($"duetime"); return TimeSpan.FromMilliseconds(500); }
                );

            observable.SubscribeX();
        }

        static void PublishLastTest()
        {
            var observable = GetObservable(10, 500)
                .Take(5)//这个会截断原来的数据序列
                .Do(l => Console.WriteLine("Publishing {0}", l)) //side effect to show it is running
                .PublishLast();//这里会保留最后一个
            observable.Connect();
            observable.SubscribeTime();
        }

        /// <summary>
        /// task转化observable流
        /// </summary>
        public static void GetObservableFromTask()
        {
            var t = Task.Factory.StartNew(() => {
                Console.WriteLine($"task will sleep 3s...");
                Thread.Sleep(3000);
                Console.WriteLine($"3s pass away...");
                return "test";
            });
            var source = t.ToObservable();
            source.Subscribe(
            Console.WriteLine,
            () => Console.WriteLine("completed"));
        }

        /// <summary>
        /// 测试Take是否是阻塞的--非阻塞
        /// </summary>
        static void TakeTest()
        {
            var obs = Observable.Interval(TimeSpan.FromSeconds(1));
            Console.WriteLine($"BeginTime:{DateTimeOffset.Now}");
            obs.Take(3).SubscribeTime();
        }
    }
}
