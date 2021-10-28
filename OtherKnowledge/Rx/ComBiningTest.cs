using Rx.ExtensionHelper;
using Rx.Models;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;

namespace Rx
{
    public class ComBiningTest
    {
        public static void Test()
        {
            //ConcatTest();
            //RepeatTest();
            //StartWithTest();
            //AmbTest();
            //AmbTest2();
            //MergeTest2();
            //ZipTest();
            //ZipTest2();
        }

        #region 并发序列的产生 组合
        static void ZipTest2()
        {
            var mm = new Subject<Coord>();
            var s1 = mm.Skip(1);
            var delta = mm.Zip(s1,
            (prev, curr) => new Coord
            {
                X = curr.X - prev.X,
                Y = curr.Y - prev.Y
            });
            delta.Subscribe(
            Console.WriteLine,
            () => Console.WriteLine("Completed"));
            mm.OnNext(new Coord { X = 0, Y = 0 });
            mm.OnNext(new Coord { X = 1, Y = 0 }); //Move across 1
            mm.OnNext(new Coord { X = 3, Y = 2 }); //Diagonally up 2
            mm.OnNext(new Coord { X = 0, Y = 0 }); //Back to 0,0
            mm.OnCompleted();
        }
        /// <summary>
        /// the result sequence will complete when the first of the sequences complete, 
        /// it will error if either of the sequences error and it will only publish once it has a pair of fresh values 
        /// from each source sequence
        /// </summary>
        static void ZipTest()
        {
            //Generate values 0,1,2 
            var nums = Observable.Interval(TimeSpan.FromMilliseconds(1000))
            .Take(3);
            //Generate values a,b,c,d,e,f 
            var chars = Observable.Interval(TimeSpan.FromMilliseconds(150))
            .Take(6)
            .Select(i => Char.ConvertFromUtf32((int)i + 97));
            //Zip values together
            nums.Zip(chars, (lhs, rhs) => new { Left = lhs, Right = rhs })
            .Dump("Zip");
        }
        /// <summary>
        /// 这里需要研究下
        /// </summary>
        static void SwitchTest()
        {
            
        }

        static void MergeTest2()
        {
            //Generate values 0,1,2 
            var s1 = Observable.Interval(TimeSpan.FromMilliseconds(250))
            .Take(3);
            //Generate values 100,101,102,103,104 
            var s2 = Observable.Interval(TimeSpan.FromMilliseconds(200))
            .Take(5)
            .Select(i => i + 100);
            s1.Merge(s2)
            .SubscribeX();
        }

        /// <summary>
        /// The result of a Merge will complete only once all input sequences complete. By contrast, 
        /// the Merge operator will error if any of the input sequences terminates erroneously
        /// </summary>
        static void MergeTest()
        {
            var s1 = new Subject<string>();
            var s2 = new Subject<string>();
            var s3 = new Subject<string>();
            var result = Observable.Merge(s2, s1, s3);
            result.SubscribeX();
            s1.OnNext("s1-1");
            s2.OnNext("s2-1");
            s3.OnNext("s3-1");

            s1.OnNext("s1-2");
            s2.OnNext("s2-2");
            s3.OnNext("s3-2");

            s1.OnNext("s1-3");
            s2.OnNext("s2-3");
            s3.OnNext("s3-3");

            s1.OnCompleted();
            s2.OnCompleted();
            s3.OnCompleted();
        }
        static void AmbTest2()
        {
            GetSequences().Amb().SubscribeX();
        }

        /// <summary>
        /// 只会发射第一个调用OnNext方法的 序列数据
        /// </summary>
        static void AmbTest()
        {
            var s1 = new Subject<string>();
            var s2 = new Subject<string>();
            var s3 = new Subject<string>();
            var result = Observable.Amb(s2, s1, s3);
            result.SubscribeX();
            s1.OnNext("s1-1");
            s2.OnNext("s2-1");
            s3.OnNext("s3-1");

            s1.OnNext("s1-2");
            s2.OnNext("s2-2");
            s3.OnNext("s3-2");

            s1.OnCompleted();
            s2.OnCompleted();
            s3.OnCompleted();
        }

        /// <summary>
        /// 有空看下
        /// </summary>
        /// <returns></returns>
        static IEnumerable<IObservable<long>> GetSequences()
        {
            Console.WriteLine("GetSequences() called");
            Console.WriteLine("Yield 1st sequence");
            yield return Observable.Create<long>(o =>
            {
                Console.WriteLine("1st subscribed to");
                return Observable.Timer(TimeSpan.FromMilliseconds(500))
                .Select(i => 1L)
                .Subscribe(o);
            });
            Console.WriteLine("Yield 2nd sequence");
            yield return Observable.Create<long>(o =>
            {
                Console.WriteLine("2nd subscribed to");
                return Observable.Timer(TimeSpan.FromMilliseconds(300))
                .Select(i => 2L)
                .Subscribe(o);
            });
            Thread.Sleep(1000);     //Force a delay
            Console.WriteLine("Yield 3rd sequence");
            yield return Observable.Create<long>(o =>
            {
                Console.WriteLine("3rd subscribed to");
                return Observable.Timer(TimeSpan.FromMilliseconds(100))
                .Select(i => 3L)
                .Subscribe(o);
            });
            Console.WriteLine("GetSequences() complete");
        }

        #endregion

        //有先后顺序的连接
        static void ConcatTest()
        {
            //Generate values 0,1,2 
            //Generate values 5,6,7,8,9 
            //var s1 = Observable.Range(0, 3);
            //var s2 = Observable.Range(5, 5);

            //Concat作用等S1序列执行OnCompleted方法之后 再和 S2序列拼接
            var s1 = Observable.Interval(TimeSpan.FromMilliseconds(500)).Select(i=> $"s1-{i}");
            var s2 = Observable.Interval(TimeSpan.FromMilliseconds(300)).Select(i=>$"s2-{i}");


            s1.Concat(s2)
            .SubscribeX();
        }

        static void RepeatTest()
        {
            Observable.Range(1, 3).Repeat(3).SubscribeX();
        }

        static void StartWithTest()
        {
            var source = Observable.Range(0, 3);
            var result = source.StartWith(-3, -2, -1);
            result.SubscribeX();
        }
    }
}
