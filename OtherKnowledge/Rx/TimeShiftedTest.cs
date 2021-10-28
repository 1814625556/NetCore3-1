using Rx.ExtensionHelper;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace Rx
{
    public class TimeShiftedTest
    {
        public static void Test()
        {
            //ThrottleTest();
        }

        /// <summary>
        /// 窗口滑动，每次收到值之后都会重置窗口时间，然后等待指定时间才发射出去最后收到的值
        /// </summary>
        static void ThrottleTest()
        {
            var obs = Observable.Create<int>(o => {

                o.OnNext(1);
                o.OnNext(2);
                Thread.Sleep(2000);
                o.OnNext(3);
                o.OnCompleted();
                return Disposable.Empty;
            });

            obs.Throttle(TimeSpan.FromMilliseconds(1500)).SubscribeX();
        }

        static void TimeOutTest()
        {
            var obs = Observable.Create<int>(o=>{

                o.OnNext(1);
                o.OnNext(2);
                Thread.Sleep(2000);
                o.OnNext(3);
                o.OnCompleted();
                return Disposable.Empty;
            });

            obs.Timeout(TimeSpan.FromMilliseconds(1500)).SubscribeX();
        }

        /// <summary>
        /// The Sample method simply takes the last value for every specified TimeSpan
        /// </summary>
        static void SampleTest()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
            interval.Sample(TimeSpan.FromSeconds(1))
            .Subscribe(Console.WriteLine);
        }
        /// <summary>
        /// just delay the time
        /// </summary>
        static void DelayTest()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(5)
            .Timestamp();
            var delay = source.Delay(TimeSpan.FromSeconds(2));
            source.Subscribe(
            value => Console.WriteLine("source : {0}", value),
            () => Console.WriteLine("source Completed"));
            delay.Subscribe(
            value => Console.WriteLine("delay : {0}", value),
            () => Console.WriteLine("delay Completed"));
        }

        //重载 运行对比着来看就可以明白了
        static void BufferTest2()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(50);
            source
            .Buffer(TimeSpan.FromSeconds(5),TimeSpan.FromSeconds(2))
            //.Buffer(3, 1)
            //.Buffer(3)
            .Subscribe(
            buffer =>
            {
                foreach (var value in buffer)
                {
                    Console.Write($"{value}, ");
                }
                Console.WriteLine($"--Buffered values,{DateTime.Now.Second}:{DateTime.Now.Millisecond}    ");
            }, () => Console.WriteLine($"Completed:{DateTime.Now.Second}:{DateTime.Now.Millisecond}"));

            Console.WriteLine($"BeginTime:{DateTime.Now.Second}:{DateTime.Now.Millisecond}");
        }
        /// <summary>
        /// 这个例子说明Buffer 缓存数据，直到size满了或者时间满了就会发射数据
        /// </summary>
        static void BufferTest()
        {
            var idealBatchSize = 15;
            var maxTimeDelay = TimeSpan.FromSeconds(2);
            var source = Observable.Interval(TimeSpan.FromSeconds(0.3)).Take(10)
            .Concat(Observable.Interval(TimeSpan.FromSeconds(0.1)).Take(100));
            source.Buffer(maxTimeDelay, idealBatchSize)
            .Subscribe(
            buffer => {
                var now = DateTime.Now;
                foreach (var b in buffer)
                {
                    Console.Write($"{b},");
                }
                Console.Write("  ");
                Console.WriteLine($"BCount:{buffer?.Count}, BTime:{DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond}");
            });

            Console.WriteLine($"BeginTime:{DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond}");
        }
    }
}
