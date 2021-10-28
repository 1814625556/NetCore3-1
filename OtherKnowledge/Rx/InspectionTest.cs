using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Rx.ExtensionHelper;

namespace Rx
{
    /// <summary>
    /// Min, Max, Sum and Average. Just like Count, these all return a sequence with a single value.
    /// Once the source completes the result sequence will produce its value and then complete
    /// </summary>
    public static class InspectionTest
    {
        public static void Test()
        {
            //ScanTest();
        }

        public static void GroupTest()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(0.1)).Take(10);
            var group = source.GroupBy(i => i % 3);
            //group.Subscribe(
            //    grp =>
            //        grp.Min().Subscribe(
            //            minValue =>
            //                Console.WriteLine("{0} min value = {1}", grp.Key, minValue)),
            //    () => Console.WriteLine("Completed"));

            group.SelectMany(
                    grp =>
                        grp.Max()
                            .Select(value => new { grp.Key, value }))
                .Dump("group");
        }

        /// <summary>
        /// 聚合函数-Aggregate allows us to get a final value for sequences that will complete
        /// 获取最终值，并且需要调用Completed函数
        /// </summary>
        public static void AggergateTest()
        {
            var sb = new Subject<int>();

            //这个方法其实是一个sum函数
            sb.Aggregate(0, (sum, val) => sum += val)
                .Subscribe(sum => Console.WriteLine($"sum is {sum}"),
                ()=>{ Console.WriteLine($"Completed");
            });
            //这个方法其实是一个count函数
            sb.Aggregate(0, (sum, val) => sum += 1)
                .Subscribe(count => Console.WriteLine($"count is {count}"));

            //min函数实现
            sb.Aggregate(-1, (min, val) =>
                {
                    if (min == -1) return val;

                    return min < val ? min : val;
                })
                .Subscribe(min => Console.WriteLine($"min is {min}"));

            //sb.OnCompleted(); 直接执行这一个的话 就是用默认值10，即Aggregate函数的第一个默认参数
            sb.OnNext(1);
            sb.OnNext(2);
            sb.OnNext(-3);
            sb.OnNext(4);
            sb.OnNext(0);
            //只有执行完 OnCompleted函数之后才会执行 Aggregate 函数中订阅的 OnNext方法和OnCompleted方法
            sb.OnCompleted();
        }

        /// <summary>
        /// 跟Aggregate 一致，Scan的触发是每次发射值的时候触发,不同的地方在于Scan 的OnNext方法被触发是每次序列OnNext的时候就被触发
        /// the difference is that Scan will push the result from every call to the accumulator function
        /// </summary>
        public static void ScanTest()
        {
            var sb = new Subject<int>();

            //这个方法其实是一个sum函数
            sb.Scan(0, (sum, val) => sum += val)
                .Subscribe(sum => Console.WriteLine($"sum is {sum}"),
                    () => {
                        Console.WriteLine($"Completed");
                    });
            //这个方法其实是一个count函数
            sb.Scan(0, (sum, val) => sum += 1)
                .Subscribe(count => Console.WriteLine($"count is {count}"));

            //min函数实现
            sb.Scan(-1, (min, val) =>
                {
                    if (min == -1) return val;

                    return min < val ? min : val;
                })
                .Subscribe(min => Console.WriteLine($"min is {min}"));

            //sb.OnCompleted(); 直接执行这一个的话 就是用默认值10，即Scan函数的第一个默认参数
            sb.OnNext(1);
            sb.OnNext(2);
            sb.OnNext(3);
            sb.OnNext(4);
            sb.OnNext(0);

            // 调用Scan的OnComPleted方法
            sb.OnCompleted();
        }

        public static void FirstAndLastTest()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, 
                ex => { }, 
                () => {Console.WriteLine($"Completed"); });

            subject
                //.LastAsync()
                //.FirstAsync()
                .Subscribe(Console.WriteLine, 
                ex => { }, () => {Console.WriteLine($"First Completed"); });
            subject.OnNext(1);
            subject.OnNext(3);
            subject.OnNext(14);
            subject.OnCompleted();
        }

        public static void CountTest()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, 
                ex => { }, () => { Console.WriteLine("Completed"); });

            subject.Count().Subscribe(cu => Console.WriteLine($"count {cu}"),
                ex => { }, () => { Console.WriteLine($"count is completed");});

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnNext(5);
            //sequence只有当执行Completed方法的时候，才会触发Count订阅的OnNext方法和OnCompleted方法
            subject.OnCompleted();
        }

        public static void AnyExTest()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine,
                ex => Console.WriteLine("subject OnError : {0}", ex),
                () => Console.WriteLine("Subject completed"));
            var any = subject.Any();
            any.Subscribe(b => Console.WriteLine("The subject has any values? {0}", b),
                ex => Console.WriteLine(".Any() OnError : {0}", ex),
                () => Console.WriteLine(".Any() completed"));
            subject.OnError(new Exception());
        }

        public static void AllTest()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () =>
            {
                Console.WriteLine("Subject completed");
            });
            var all = subject.All(i => i < 5);
            all.Subscribe(
                b =>
                {
                    Console.WriteLine("All values less than 5? {0}", b);//遇到false的时候就会执行
                },
                () =>
                {
                    Console.WriteLine("all Completed");//遇到false的时候就会执行了
                });
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(6);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnCompleted();
        }

        public static void ContainTest()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var contains = subject.Contains(2);
            contains.Subscribe(
                b => Console.WriteLine("Contains the value 2? {0}", b),
                () => Console.WriteLine("contains completed"));
            subject.OnNext(1);
            subject.OnNext(2);//执行到这一步的时候 就会运行 contains订阅的OnNext方法和OnCompleted
            subject.OnNext(3);
            subject.OnCompleted();
        }

        public static void DefaultEmptyTest()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var defaultIfEmpty = subject.DefaultIfEmpty();
            defaultIfEmpty.Subscribe(
                b => Console.WriteLine("defaultIfEmpty value: {0}", b),
                () => Console.WriteLine("defaultIfEmpty completed"));
            var default42IfEmpty = subject.DefaultIfEmpty(42);
            default42IfEmpty.Subscribe(
                b => Console.WriteLine("default42IfEmpty value: {0}", b),
                () => Console.WriteLine("default42IfEmpty completed"));
            subject.OnCompleted();
        }
    }

}
