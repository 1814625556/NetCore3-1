using Rx.ExtensionHelper;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rx
{
    public class RxCreateTest
    {
        public static void Test()
        {
            
        }

        /// <summary>
        /// OnCompleted方法只会执行一次
        /// </summary>
        static void OnCompletedTest()
        {
            var subject = new Subject<int>();
            subject.Subscribe(val => Console.WriteLine(val), ex => { }, () => { Console.WriteLine("Completed"); });
            subject.OnNext(1);
            subject.OnCompleted();
            subject.OnCompleted();
            subject.OnCompleted();
            subject.OnCompleted();
            subject.OnCompleted();
        }

        /// <summary>
        /// task转化observable流
        /// </summary>
        public static void GetObservableFromTask()
        {
            var t = Task.Factory.StartNew(() => "Test");
            var source = t.ToObservable();
            source.Subscribe(
            Console.WriteLine,
            () => Console.WriteLine("completed"));
        }

        #region 默认情况下，处理将在 ThreadPool 线程上异步完成，然后调用OnNext方法和OnCompleted方法
        static void StartAction()
        {
            var start = Observable.Start(() =>
            {
                Console.Write("Working away");
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    Console.Write(".");
                }
            });
            start.Subscribe(
            unit => Console.WriteLine("Unit published"),
            () => Console.WriteLine("Action completed"));
        }
        static void StartFunc()
        {
            var start = Observable.Start(() =>
            {
                Console.Write("Working away");
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    Console.Write(".");
                }
                return "Published value";
            });
            start.Subscribe(
            Console.WriteLine,
            () => Console.WriteLine("Func completed"));
        }
        #endregion

        //这里是实现Timer的一种方式
        public static void Timer(TimeSpan dueTime)
        {
            var observable = Observable.Generate
                (
                0,
                x => { Console.WriteLine($"condition:{x<10}"); return x < 10; },
                x => { Console.WriteLine($"iterate:"); return x + 1; },
                x => $"rs {x}",
                x => { Console.WriteLine($"duetime"); return TimeSpan.FromMilliseconds(500); }
                );

            observable.SubscribeX();

            //return Observable.Generate(
            //0,
            //i => i < 1,
            //i => i + 1,
            //i => i,
            //i => dueTime);
        }

        /// <summary>
        /// 一段时间之后只会发射一个值，然后执行completed函数，interval 则会持续发射
        /// </summary>
        public static void TimerTest()
        {
            var timer = Observable.Timer(TimeSpan.FromSeconds(1));
            timer.Subscribe(
            Console.WriteLine,
            () => Console.WriteLine("completed"));
        }

        /// <summary>
        /// interval的一种实现方式
        /// </summary>
        public static void TimerIntervalTest()
        {
            var timer = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(500));
            timer.Subscribe(
            Console.WriteLine,
            () => Console.WriteLine("completed"));
        }

        /// <summary>
        /// range实现的一个例子
        /// </summary>
        public static void RangeTest()
        {
            var range = Observable.Range(10, 15);
            range.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }

        /// <summary>
        /// range函数的一种实现
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IObservable<int> Range(int start, int count)
        {
            var max = start + count;
            return Observable.Generate(
            start,
            value => value < max,
            value => value + 1,
            value => value);
        }

        public static void UsingInterval()
        {
            //interval 里面会使用多线程
            Observable.Interval(TimeSpan.FromMilliseconds(1000))
                //.Sample(TimeSpan.FromMilliseconds(500))
                .Subscribe(number => {
                    Console.WriteLine($"Interval threadId: {Thread.CurrentThread.ManagedThreadId}");
                    Console.WriteLine($"number is {number}");
                    Console.WriteLine($"--------------------------------------");
                });

            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Main threadId: {Thread.CurrentThread.ManagedThreadId}");
                Console.ResetColor();
            }
        }

        public static void CompleteTest()
        {
            Observable.Range(1, 10)
          .Subscribe(
                x => Console.WriteLine(x.ToString()), 
                e => Console.WriteLine("Error" + e.Message), 
                () => Console.WriteLine("Completed"));
        }


        public static void UsingReturn()
        {
            var greeting = Observable.Return("Hello world");

            //相当于：
            var subject = new ReplaySubject<string>();
            subject.OnNext("Value");
            subject.OnCompleted();

            //这个是同步的
            greeting.Subscribe(str=> {
                Thread.Sleep(1000);
                Console.WriteLine($"1 threadid: {Thread.CurrentThread.ManagedThreadId}");            
            });
            greeting.Subscribe(str=> {
                Thread.Sleep(10);
                Console.WriteLine($"2 threadid: {Thread.CurrentThread.ManagedThreadId}");
            });
        }

        public static void UsingCreate()
        {
            var greeting = Observable.Create<string>(observer =>
            {
                observer.OnNext("Hello world");

                //执行完 completed之后会执行 Observer has unsubscribed
                observer.OnCompleted();
                return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
            });

            greeting.Subscribe(
                str => Console.WriteLine(str),
                ()=> Console.WriteLine("completed1"));

            greeting.Subscribe(
                str => Console.WriteLine(str),
                () => Console.WriteLine("completed2"));
        }

        public static void TestRxException()
        {
            var greeting = Observable.Create<string>(observer =>
            {
                observer.OnNext("Hello world1");

                //onerror 执行完之后不会执行后续操作了，但是仍然会执行 observer has unsubscribed
                observer.OnError(new Exception("occur exception"));

                observer.OnNext("Hello world2");
                observer.OnNext("Hello world3");
                observer.OnCompleted();
                return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
            });

            greeting.Subscribe(
                str => Console.WriteLine(str),
                ex=>Console.WriteLine(ex.Message),
                () => Console.WriteLine("completed2"));
        }

        public static void UsingGenerate()
        {
            var range = Observable.Generate(0, x => x < 10, x => x + 1, x => x);
            range.Subscribe(Console.Write);

            Console.WriteLine();
            range.Subscribe(Console.Write);
            Console.WriteLine();

            //同样的效果
            Observable.Range(1, 10).Subscribe(Console.Write);
        }

        public static void UsingRepeat()
        {
            Observable.Repeat(6,6, NewThreadScheduler.Default).Subscribe(i =>
            {
                Console.WriteLine($"threadId:{Thread.CurrentThread.ManagedThreadId},val:{i}");
            });
        }

    }
}
