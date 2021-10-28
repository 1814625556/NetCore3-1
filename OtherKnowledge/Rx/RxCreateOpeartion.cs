using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Timers;

namespace Rx
{
    /// <summary>
    /// 这里需要记住Create 属于懒加载，只有订阅的时候 才会执行OnNext方法
    /// </summary>
    public class RxCreateOperation
    {
        public static void Test()
        {
        }

        #region 使用Create 实现自己的 Empty Never Throw Return
        public static IObservable<T> Empty<T>()
        {
            return Observable.Create<T>(o =>
            {
                o.OnCompleted();
                return Disposable.Empty;
            });
        }
        public static IObservable<T> Return<T>(T value)
        {
            return Observable.Create<T>(o =>
            {
                o.OnNext(value);
                o.OnCompleted();
                return Disposable.Empty;
            });
        }
        public static IObservable<T> Never<T>()
        {
            return Observable.Create<T>(o =>
            {
                return Disposable.Empty;
            });
        }
        public static IObservable<T> Throws<T>(Exception exception)
        {
            return Observable.Create<T>(o =>
            {
                o.OnError(exception);
                return Disposable.Empty;
            });
        }
        #endregion
        public static void NonBlocking_event_driven()
        {
            //返回 dispose-这个委托执行dispose方法之后仍然会继续输出 原因timer.Elapsed += OnTimerElapsed;
            var ob = Observable.Create<string>(
            observer =>
            {
                var timer = new System.Timers.Timer();
                timer.Interval = 1000;
                timer.Elapsed += (s, e) => observer.OnNext("tick");
                timer.Elapsed += OnTimerElapsed;
                timer.Start();
                return Disposable.Empty;
            });

            //返回调用dispose之后执行的委托
            var ob2 = Observable.Create<string>(
            observer =>
            {
                var timer = new System.Timers.Timer();
                timer.Enabled = true;
                timer.Interval = 100;
                timer.Elapsed += OnTimerElapsed;
                timer.Start();

                //这就是 执行dispose之后执行的委托
                return () =>
                {
                    timer.Elapsed -= OnTimerElapsed;
                    timer.Dispose();
                };
            });
            var subscription = ob2.Subscribe(Console.WriteLine);
            Console.ReadLine();
            subscription.Dispose();
        }
        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(e.SignalTime);
        }

        public static void CreateLazyTest()
        {
            BlockingMethod();
            //.Subscribe(Console.WriteLine, e => { }, () => Console.WriteLine("Completed"));
            Console.WriteLine("-----------------------------");
            NonBlocking();
            //.Subscribe(Console.WriteLine, e => { }, () => Console.WriteLine("Completed"));
        }

        private static IObservable<string> BlockingMethod()
        {
            var subject = new ReplaySubject<string>();
            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnCompleted();
            Console.WriteLine(DateTime.Now);
            Thread.Sleep(2000);
            Console.WriteLine(DateTime.Now);
            return subject;
        }
        private static IObservable<string> NonBlocking()
        {
            //create是懒加载，只有当subscribe的时候才会 执行创建操作里面的方法
            return Observable.Create<string>(
                observer =>
                {
                    observer.OnNext($"non a,threadid:{Thread.CurrentThread.ManagedThreadId}");
                    observer.OnNext("non b");
                    observer.OnCompleted();
                    Console.WriteLine(DateTime.Now);
                    Thread.Sleep(2000);
                    Console.WriteLine(DateTime.Now);
                    return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
                    //or can return an Action like 
                    //return () => Console.WriteLine("Observer has unsubscribed"); 
                });
        }

        //这个方式 相当于直接触发Observe的 onerror方法
        public static void UsingThrow()
        {
            var throws = Observable.Throw<string>(new Exception());
            throws.Subscribe(Console.WriteLine, ex => Console.WriteLine(ex != null));
            //Behaviorally equivalent to
            var subject = new ReplaySubject<string>();
            subject.OnError(new Exception());
        }

        public static void UsingNever()
        {
            //一般中断事件流的时候使用，对比empty 不会调用OnCompleted方法
            var never = Observable.Never<string>();
            never.Subscribe(
                s => Console.WriteLine($"threadid:{Thread.CurrentThread.ManagedThreadId}-{s}")
            );
            //similar to a subject without notifications
            var subject = new Subject<string>();
        }

        //会触发一次OnNext方法和一次OnCompleted方法
        public static void UsingReturn()
        {
            var greeting = Observable.Return("Hello world");

            //相当于：
            var subject = new ReplaySubject<string>();
            subject.OnNext("Hello world");
            subject.OnCompleted();

            //Observable.Return("x").Subscribe(Console.WriteLine, e => { }, () => Console.WriteLine($"Completed"));
        }

        public static void UsingEmpty()
        {
            var empty = Observable.Empty<string>()
                .Subscribe(Console.WriteLine, e => { }, () => Console.WriteLine("Complete"));
            //Behaviorally equivalent to 相当于
            var subject = new ReplaySubject<string>();
            subject.OnCompleted();
        }
    }
}
