using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace Rx
{
    public class ReduceSequenceTest
    {
        public static void Test()
        {
        }

        /// <summary>
        /// 所有的重复值 都会被过滤掉
        /// </summary>
        private static void DistinctTest()
        {
            var subject = new Subject<int>();
            //var distinct = subject.Distinct();//所有的重复值 都会被过滤掉
            var distinct = subject.DistinctUntilChanged();//只会跟前一个对比

            subject.Subscribe(
            i => 
            {
                Console.WriteLine("{0}", i);
            },
            () => Console.WriteLine("subject.OnCompleted()"));

            distinct.Subscribe(
            i => Console.WriteLine("distinct.OnNext({0})", i),
            () => Console.WriteLine("distinct.OnCompleted()"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(2);
            subject.OnNext(2);
            subject.OnNext(2);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(4);
            subject.OnCompleted();
        }

        public static void IgnoreElementTest()
        {
            var subject = new Subject<int>();
            //Could use subject.Where(_=>false);

            //使用这个方法之后只会触发 OnCompleted 和 OnError
            var noElements = subject.IgnoreElements();
            subject.Subscribe(
                i => Console.WriteLine("subject.OnNext({0})", i),
                () => Console.WriteLine("subject.OnCompleted()"));
            noElements.Subscribe(
                i => Console.WriteLine("noElements.OnNext({0})", i),
                () => Console.WriteLine("noElements.OnCompleted()"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
            subject.OnNext(3);//执行完 OnCompleted方法之后，OnNext方法就不再会被触发了
        }

        public static void SkipAndTakeTest()
        {
            Console.WriteLine($"Skip function--------------------------------");
            Observable.Range(0, 10)
                .Skip(3)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));

            Console.WriteLine($"Take function--------------------------------");

            Observable.Range(0, 10)
                .Take(3)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));

            Console.WriteLine();
            Console.WriteLine("计数器");
            //以防万一漏掉了任何读取器，Take操作符会在收到计数后完成。
            //我们可以通过将其应用于无限序列来证明这一点
            Observable.Interval(TimeSpan.FromMilliseconds(1000))
                .Take(3)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }

        public static void SkipWhileTest()
        {
            var subject = new Subject<int>();
            subject
                .SkipWhile//跳过第一个返回false之前的所有值
                //.TakeWhile
                (i =>
                {
                    //Console.WriteLine(i);
                    return i > 10;//当为false的时候 OnNext方法就不会执行了
                })
                .Subscribe(
                    val=>Console.WriteLine(val),
                    () => Console.WriteLine("Completed"));
            subject.OnNext(11);
            subject.OnNext(21);
            subject.OnNext(21);
            subject.OnNext(21);
            subject.OnNext(31);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(11);
            subject.OnNext(10);
            subject.OnCompleted();
        }

        public static void TakeWhileTest()
        {
            var subject = new Subject<int>();
            subject
                .TakeWhile(i => i < 4)// 返回false之前的所有数据
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(5);
            subject.OnNext(4);
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(0);
            subject.OnCompleted();
        }

        public static void SkipLastTest()
        {
            var subject = new Subject<int>();
            subject
                .SkipLast(2)
                .Subscribe(
                    i =>
                    {
                        Console.WriteLine($"subscribe threadId:{Thread.CurrentThread.ManagedThreadId},value:{i}");
                    }, () => Console.WriteLine("Completed"));
            Console.WriteLine($"Pushing 1,threadId:{Thread.CurrentThread.ManagedThreadId}");
            subject.OnNext(1);
            Console.WriteLine("Pushing 2");
            subject.OnNext(2);
            Console.WriteLine("Pushing 3");
            subject.OnNext(3);
            Console.WriteLine("Pushing 4");
            subject.OnNext(4);
            Console.WriteLine("Pushing 5");
            subject.OnNext(5);
            subject.OnCompleted();
        }

        /// <summary>
        /// 使用场景应该很少
        /// </summary>
        public static void TakeUntilTest()
        {
            var subject = new Subject<int>();
            var otherSubject = new Subject<Unit>();
            subject
                .TakeUntil(otherSubject)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            otherSubject.OnNext(Unit.Default);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnNext(6);
            subject.OnNext(7);
            subject.OnNext(8);
            subject.OnCompleted();
        }

    }
}
