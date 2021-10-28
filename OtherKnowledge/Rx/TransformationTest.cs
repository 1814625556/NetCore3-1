using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Rx.ExtensionHelper;

namespace Rx
{
    public static class TransformationTest
    {
        public static void Test()
        {
            //IObservableSelectManyTest();
            //Observable.Generate(0, i => i < 3, i => i+1, i => 2 * 10 + i).Subscribe(Console.WriteLine);
            //IObservableSelectManyTest();
        }

        static void IObservableSelectManyTest()
        {
            var sequence = Observable.Interval(TimeSpan.FromMilliseconds(2000));
            sequence.Select(i=>Convert.ToInt32(i))
                //.SelectMany(val => { return Observable.Range(val,3); })
                .SelectMany(val => {
                    var max = val + 2;
                    var origin = val;
                    return Observable.Generate(0, i => i < 3, i => i+1, i => origin*10+i);
                })
                .Subscribe(v=>Console.WriteLine(v),
                ex=> { },
                ()=> { Console.WriteLine("Completed"); });
        }
        private static void IEnumerableSelectManyTest()
        {
            var enumerableSource = new[] { 1, 2, 3 };
            var enumerableResult = enumerableSource.SelectMany(GetSubValues);
            foreach (var value in enumerableResult)
            {
                Console.WriteLine(value);
            }
        }

        private static IEnumerable<int> GetSubValues(int offset)
        {
            yield return offset * 10;
            yield return (offset * 10) + 1;
            yield return (offset * 10) + 2;
        }

        private static IObservable<int> GetObsValues(int offset)
        {
            return Observable.Return(offset * 10);
            //return Observable.Return(offset * 10 + 1);
            //return Observable.Return(offset * 10 + 2);
        }

        static void SelectManyTest()
        {
            //Observable.Return(3)
            //    .SelectMany(i => Observable.Range(1, i))
            //    .Dump("SelectMany");

            //Func<int, char> letter = i => (char)(i + 64);
            //Observable.Range(1, 3)
            //    .SelectMany(i => Observable.Return(letter(i)))
            //    .Dump("SelectMany");

            Func<int, char> letter = i =>
            {
                var rs = (char)(i + 64);
                return rs;
            };
            var rangeObject = Observable.Range(1, 30);
                rangeObject.SelectMany(
                    i =>
                    {
                        if (0 < i && i < 27)
                        {
                            return Observable.Return(letter(i));
                        }
                        else
                        {
                            return Observable.Empty<char>();//这个方法实际上执行了OnCompleted方法
                        }
                    })
                .SubscribeX();
        }

        /// <summary>
        /// TimeStamp,TimeInterval just wrapped value with time
        /// </summary>
        static void TimestampAndTimeIntervalTest()
        {
            //绝对时间
            //Observable.Interval(TimeSpan.FromSeconds(1))
            //    .Take(3)
            //    .Timestamp()
            //    .Subscribe(val=>Console.WriteLine($"val:{val.Value},  timestamp:{val.Timestamp}"));

            //上一个序列的时间差
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(3)
                .TimeInterval()
                .Subscribe(val => Console.WriteLine($"val:{val.Value},  timestamp:{val.Interval}"));
        }

        static void SelectTest()
        {
            var source = Observable.Range(0, 5);
            source.Select(i => i + 3)
                .Dump("+3");

            Observable.Range(1, 5)
                .Select(
                    i => new { Number = i, Character = (char)(i + 64) })
                .Dump("anon");
        }

        static void CastTest()
        {
            var objects = new Subject<object>();
            objects.Cast<int>().Dump("cast");
            objects.OnNext(1);
            objects.OnNext(2);
            objects.OnNext(3);
            objects.OnCompleted();
        }

        static void OfTypeTest()
        {
            var objects = new Subject<object>();
            objects.OfType<int>().Dump("OfType");
            objects.OnNext(1);
            objects.OnNext(2);
            objects.OnNext("3");//Ignored
            objects.OnNext(4);
            objects.OnCompleted();
        }
    }
}
