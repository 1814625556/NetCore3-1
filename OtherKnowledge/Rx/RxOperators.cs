using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rx
{
    public class RxOperators
    {
        /// <summary>
        /// The Sample method simply takes the last value for every specified TimeSpan
        /// Throttle的周期是一个滑动窗口。每次Throttle收到一个值时，窗口就会重置。
        /// 只有经过一段时间后，才会传播最后一个值-这是与sample的区别
        /// </summary>
        public static async void SampleTest()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
            var sampleDispose = interval.Sample(TimeSpan.FromSeconds(1))
                .Subscribe(num =>
                {
                    Console.WriteLine($"value :{num},threadId:{Thread.CurrentThread.ManagedThreadId}");
                });

            //等待三秒之后 unsubscribe
            await Task.Delay(3000);
            sampleDispose.Dispose();
        }

        public static void Test()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            subject.OnCompleted();
            subject.OnNext(2);
        }

    }
}
