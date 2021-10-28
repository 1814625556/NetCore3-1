using Rx.ExtensionHelper;
using Rx.Models;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace Rx
{
    public class ErrorHandlerTest
    {
        public static void Test()
        {
            //CatchTest();
            //FinallyTest();
            //UsingTest();
            //RetryTest();
        }

        /// <summary>
        /// catch 之后会调用 subscribe订阅的OnCompleted方法，catch的触发条件就是suject调用OnError方法
        /// 正常情况下 OnError 和 OnCompleted方法只会调用一个
        /// </summary>
        static void CatchTest()
        {
            var source = new Subject<int>();

            //var result = source.Catch(Observable.Empty<int>());

            var result = source.Catch<int,Exception>(e => {
                Console.WriteLine($"catch:{e.Message}");
                return Observable.Return(10);
            });
            //result.SubscribeX();
            var ds = source.SubscribeX();
            source.OnNext(1);
            source.OnNext(2);
            source.OnError(new Exception("Fail!"));

            ds.Dispose();
        }
        static void CatchTest2()
        {
            var source = new Subject<int>();
            //会出现 捕捉不到的情况(原因 异常类型不一样)，这时候还是会调用OnError方法,这时候就不会调用OnCompleted方法了
            var result = source.Catch<int, TimeoutException>(tx => { Console.WriteLine($"demo-{tx.Message}"); return Observable.Return(-1); });
            result.SubscribeX();
            source.OnNext(11);
            source.OnNext(21);
            source.OnError(new ArgumentException("Fail!-no"));
        }

        /// <summary>
        /// OnError OnCompleted Dispose都会执行 Finally函数
        /// </summary>
        static void FinallyTest()
        {
            try 
            {
                var source = new Subject<int>();
                var result = source.Finally(() => Console.WriteLine("Finally action ran"));
                var ds = result.SubscribeX();

                //新线程的话 异常捕获不到了
                //var ds = result.SubscribeOn(Scheduler.Default).SubscribeNoError();

                //这种方式会被抛到 下面的catch中
                //var ds = result.SubscribeNoError();
                source.OnNext(1);
                source.OnNext(2);
                source.OnNext(3);

                source.OnError(new ArgumentException("参数错误"));
                //source.OnCompleted();
                //ds.Dispose();
            }
            catch (Exception e) 
            {}
        }

        /// <summary>
        /// 将一个正常对象转换为 Observable 对象 不过我实在是没看出来这个方法的使用场景是什么
        /// </summary>
        static void UsingTest()
        {
            var bindDemo = new ItemDispose("12345");
            //var source = Observable.Interval(TimeSpan.FromSeconds(1));
            var result = Observable.Using<string, ItemDispose>(
                () => bindDemo,
                timeIt => Observable.Return(timeIt.Msg));
            result.SubscribeX();

            //并不会触发OnNext方法
            bindDemo.Msg = "demo01";
            bindDemo.Msg = "demo02";
            bindDemo.Msg = "demo03";
            
        }

        /// <summary>
        /// Retry 只有在 调用OnError方法的时候才会执行
        /// </summary>
        static void RetryTest()
        {
            //Observable.Range(11, 2).Retry(2).SubscribeX();

            var observable = Observable.Create<int>(o => {
                o.OnNext(1);
                o.OnNext(2);
                o.OnNext(3);
                o.OnError(new Exception("Create Error..."));
                o.OnCompleted();
                return Disposable.Empty;
            });

            observable.Retry(3).SubscribeX();
        }

    }
}
