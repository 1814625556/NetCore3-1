using Rx.ExtensionHelper;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;

namespace Rx
{
    public class SubjectTest
    {
        public static void Test()
        {
            //AsyncSubjectTest();
            //BehaviorSubjectTest();
        }

        public static void AsyncSubjectTest()
        {
            //只存储最后一个值，只会发射一次(调用OnCompleted方法才会发射)，发射之后 调用 OnCompleted方法
            var subject = new AsyncSubject<string>();
            subject.OnNext("a");
            subject.SubscribeX();
            subject.OnNext("b");
            subject.OnNext("c");
            subject.OnNext("d");
            subject.OnCompleted();
        }

        public static void BehaviorSubjectTest()
        {
            //必须有默认值，subscribe之后，只会保留最后OnNext的值，如果执行了OnCompleted方法，
            //再subscribe，那么不会发射任何值了
            var subject = new BehaviorSubject<string>("a");
            subject.OnNext("b");
            subject.OnNext("c");
            subject.OnNext("e");
            subject.OnNext("f");
            var ds = subject.SubscribeX();
            //ds.Dispose();
            subject.OnNext("d");
            subject.OnNext("g");
            subject.OnNext("h");
            subject.OnNext("j");
            //subject.OnCompleted();
            //subject.SubscribeX();
        }

        public static void ReplaySubjectTest()
        {
            //配置缓存区大小 - 两个，获取的是最后的两个值，还可以配置时间
            var subject = new ReplaySubject<string>(2);
            subject.OnNext("a1");
            subject.OnNext("a2");
            subject.OnNext("a3");
            subject.OnNext("a4");
            subject.OnNext("a5");
            subject.OnNext("a6");
            WriteSequenceToConsole(subject);
            subject.OnNext("b");
            subject.OnNext("c");

            //配置时间
            var window = TimeSpan.FromMilliseconds(150); 
            subject = new ReplaySubject<string>(window);
            subject.OnNext("w");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            subject.OnNext("x");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            subject.OnNext("y");
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("z");
        }

        /// <summary>
        /// 正常subject方法测试
        /// </summary>
        public static void NormalSubjectTest()
        {
            var subject = new Subject<string>();
            var subDispose = subject
                //.Catch<string, Exception>(e => Observable.Return("error"))
                .Subscribe(
                    Console.WriteLine,
                    ex => Console.WriteLine(ex.Message),
                    () => Console.WriteLine("Completed"));

            subject.OnNext("a");
            //subject.OnError(new Exception("xxx")); //执行完error之后不会再执行 OnNext，或者OnCompleted方法了，
            //除非这里用Catch扩展方法获取，然后还可以执行OnCompleted方法
            subject.OnNext("b");
            subject.OnNext("b");
            subject.OnCompleted();//执行完OnCompleted方法之后 ，后面的OnNext方法都会被阻塞掉
            subject.OnNext("Excute completed");
            Thread.Sleep(500);
            subDispose.Dispose();//取消订阅-> UnSubscribe
            subject.OnNext("c");
            subject.OnNext("d");
        }

        static void WriteSequenceToConsole(IObservable<string> sequence)
        {
            //The next two lines are equivalent.
            //sequence.Subscribe(value=>Console.WriteLine(value));
            sequence.Subscribe(Console.WriteLine);
        }

    }
}
