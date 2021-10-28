using Rx.ExtensionHelper;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;

namespace Rx
{
    public class Chapter3SideTest
    {
        public static void Test()
        {
            //TimeOutTest();
            //ToTaskTest();
            //ToEventTest();
        }

        /// <summary>
        /// select函数有个重载 里面可以设置索引
        /// </summary>
        static void ComposingTest()
        {
            var source = Observable.Range(5, 3);

            //var result = source.Select(
            //(value, idx) => new
            //{
            //    Index = idx,
            //    Letter = (char)(value + 65)
            //});

            var result = source.Scan(
                new
                {
                    Index = -1,
                    Letter = new char()
                },
                (acc, value) => new
                {
                    Index = acc.Index + 1,
                    Letter = (char)(value + 65)
                });

            result.Subscribe(
            x => Console.WriteLine("Received {0} at index {1}", x.Letter, x.Index),
            () => Console.WriteLine("completed"));
            result.Subscribe(
            x => Console.WriteLine("Also received {0} at index {1}", x.Letter, x.Index),
            () => Console.WriteLine("2nd completed"));

            
        }

        static void DoTest()
        {
            Observable.Range(1, 5).Do(i => { },()=> { Console.WriteLine("Do OnCompleted"); })
                .SubscribeX();
        }


        /// <summary>
        /// 阻塞操作符：Foreach First Last
        /// </summary>
        static void ForeachTest()
        {
            // 与 Subscribe的区别在于阻塞和不阻塞
            //var source = Observable.Interval(TimeSpan.FromSeconds(1))
            //.Take(5);
            //source.ForEach(i => Console.WriteLine("received {0} @ {1}", i, DateTime.Now));
            //Console.WriteLine("completed @ {0}", DateTime.Now);

            var source = Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(5);
            source.ObserveOn(Scheduler.CurrentThread).Subscribe(i => {
                //Console.WriteLine("received {0} @ {1}", i, DateTime.Now);
                Console.WriteLine($"Scheduler.CurrentThreadId:{Thread.CurrentThread.ManagedThreadId}");
            });

            Console.WriteLine($"main threadId:{Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("completed @ {0}", DateTime.Now);
        }

        static void ToEnumerableTest()
        {
            var period = TimeSpan.FromMilliseconds(1000);
            var source = Observable.Timer(TimeSpan.FromMilliseconds(1000), period)
            .Take(5);
            var result = source.ToEnumerable();
            foreach (var value in result)
            {
                Console.WriteLine("------");
                Console.WriteLine(value);
            }
            Console.WriteLine("done");
        }

        /// <summary>
        /// ToArray,ToList会阻塞 直到所有值返回才会发射出去一个值，是一个集合
        /// </summary>
        static void ToListOrToArrayTest()
        {
            var period = TimeSpan.FromMilliseconds(2000);
            var source = Observable.Timer(TimeSpan.Zero, period).Take(5);
            var result = source.ToArray();
            result.Subscribe(
            arr => {
                Console.WriteLine("Received array");
                foreach (var value in arr)
                {
                    Console.WriteLine(value);
                }
            },
            () => Console.WriteLine("Completed")
            );
            Console.WriteLine("Subscribed");
        }

        /// <summary>
        /// 规定的时间如果没有 发射出来值 就会报错
        /// </summary>
        static void TimeOutTest()
        {
            Observable.Range(1, 4).Do(i => Thread.Sleep(i * 500)).Timeout(TimeSpan.FromMilliseconds(1100)).SubscribeX();
        }


        /// <summary>
        /// show how to transition from a task to an observable sequence
        /// The ToTask extension method will allow you to convert an observable sequence into a Task<T>. Like an AsyncSubject<T>, 
        /// this method will ignore multiple values, only returning the last value
        /// </summary>
        static void ToTaskTest()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(5);
            var result = source.ToTask(); //Will arrive in 5 seconds. 
            Console.WriteLine(result.Result);
        }

        static void ToEventTest()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(5);
            var result = source.ToEvent();
            result.OnNext += val => Console.WriteLine(val);
        }
    }
}
