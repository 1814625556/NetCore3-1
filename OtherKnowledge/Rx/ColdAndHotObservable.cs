using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;

namespace Rx
{
    //Observable 什么时候开始发出它的项目序列？这取决于 Observable。“热” Observable 可能会在创建后立即开始发出项目，
    //因此任何后来订阅该 Observable 的观察者都可以开始观察中间某处的序列。另一方面，“冷” Observable 会等待观察者订阅它，
    //然后才开始发出项目，因此这样的观察者可以保证从一开始就看到整个序列。
    //所以区分cold and hot observable the standard is 什么时候开始发射数据，冷：订阅（subscribe），热：随时

    //在 ReactiveX 的一些实现中，还有一种叫做“可连接”的 Observable。这样的 Observable 在其Connect方法被调用之前不会开始发出项目 ，
    //无论是否有任何观察者订阅它。

    /// <summary>
    /// publish 的作用应该是共享，即interval 如果没有publish 那么每次订阅（subscribe）都会是新的序列，用publish修饰之后 不同的订阅（subscribe）
    /// 使用的是同一个序列
    /// connect，RefCount,之后才开始发送数据
    /// </summary>
    public class ColdAndHotObservable
    {
        public static void Test()
        {
        }

        /// <summary>
        /// 多播
        /// </summary>
        public static void MultiCast()
        {
            var period = TimeSpan.FromSeconds(1);
            //var observable = Observable.Interval(period).Publish();
            var observable = Observable.Interval(period);
            var shared = new Subject<long>();
            shared.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            observable.Subscribe(shared);   //'Connect' the observable.
            Thread.Sleep(1500);
            shared.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
        }

        /// <summary>
        /// replay,缓存了序列的发射值，这里故意弄掉了一个发射值
        /// </summary>
        public static void ReplayTest()
        {
            var period = TimeSpan.FromSeconds(1);
            var hot = Observable.Interval(period)
                .Take(3)
                .Publish();
            hot.Connect();
            Thread.Sleep(period); //Run hot and ensure a value is lost.
            var observable = hot.Replay();
            observable.Connect();
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(period);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
            Console.ReadKey();
            observable.Subscribe(i => Console.WriteLine("third subscription : {0}", i));
            observable.Subscribe(i => Console.WriteLine("fourth subscription : {0}", i));
            Console.ReadKey();
        }

        public static void PublishLasTest()
        {
            var period = TimeSpan.FromMilliseconds(500);
            var observable = Observable.Interval(period)
                .Take(5)//这个会截断原来的数据序列
                .Do(l => Console.WriteLine("Publishing {0}", l)) //side effect to show it is running
                .PublishLast();//这里会保留最后一个
            observable.Connect();
            
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i)
                , ex => { },
                () => Console.WriteLine("Completed"));//会执行Completed
            Thread.Sleep(3000);

            //下面代码证明只会保留最后一个值
            observable.Subscribe(i => Console.WriteLine("--------------subscription2 : {0}", i)
                , ex => { },
                () => Console.WriteLine("------------Completed2"));//会执行Completed
            subscription.Dispose();
        }

        //序列的订阅被dispose之后，connect的对象没有被dispose ，所以序列依然在发送只是不会接收了
        public static void ConnectTest()
        {
            var period = TimeSpan.FromMilliseconds(500);
            var observable = Observable.Interval(period)
                .Do(l => Console.WriteLine("Publishing {0}", l)) //Side effect to show it is running
                .Publish();
            observable.Connect();//开始发射--会执行Do方法
            Thread.Sleep(1500);
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));
            Thread.Sleep(3000);
            //这里只是dispose了subscription 但是 并没有 dispose connect ，序列仍然会发送
            subscription.Dispose(); 
        }

        /// <summary>
        /// 这里对照上面看，一般采用connect之后 需要两次释放 一次释放subscribe 一次释放connect，
        /// 有一种简单的方式 
        /// 是用RefCount替换Connect，
        /// 那么释放一次就可以了
        /// </summary>
        public static void RefCountTest()
        {
            var period = TimeSpan.FromMilliseconds(500);
            var observable = Observable.Interval(period)
                .Do(l => Console.WriteLine("Publishing {0}", l)) //side effect to show it is running
                .Publish()
                .RefCount(); //如果注释掉 那么是不会发射数据的

            //先睡眠3秒钟-这里并不会发射 等待 订阅之后才会发射
            Thread.Sleep(1500);
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));
            Thread.Sleep(2000);
            observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));
            Thread.Sleep(1500);
            //这里只是释放掉了第一次的订阅，第二次的订阅仍然起作用
            subscription.Dispose();
        }

        /// <summary>
        /// connect 的连接与关闭测试,开闭之后每次都是新的数据
        /// </summary>
        public static void DisposeConnectTest()
        {
            var period = TimeSpan.FromMilliseconds(500);
            var observable = Observable.Interval(period).Publish();
            observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));
            
            //执行Connect方法的时候会返回一个IDisposable对象，可以调用Dispose方法 关闭发送
            var connection = observable.Connect(); 
            Thread.Sleep(2000);
            connection.Dispose();
            Console.WriteLine("connection.Dispose();");
            connection = observable.Connect(); 
            Thread.Sleep(2000);
            connection.Dispose(); 

            Thread.Sleep(2000);
        }

        /// <summary>
        /// 这个例子说明调用 Connect方法的时候才开始真正发送数据
        /// </summary>
        public static void PublishAndConnectTest()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period).Publish();
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Console.WriteLine("sleep 2s");
            Thread.Sleep(2000);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
            observable.Connect();
        }

        /// <summary>
        /// interval发射的序列是冷的 不能共享数据.
        /// 如果我们希望能够共享实际数据值而不仅仅是 observable 实例，我们可以使用Publish()扩展方法。
        /// 这将返回一个 IConnectableObservable<T>，它 通过添加一个Connect()方法扩展了IObservable<T>。
        /// 通过使用Publish()然后 Connect()方法，我们可以获得此共享功能。
        /// </summary>
        public static void ColdIntervalTest()
        {
            //可以共享的例子
            //var period = TimeSpan.FromSeconds(1);
            //var observable = Observable.Interval(period).Publish();
            //observable.Connect();
            //observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            //Thread.Sleep(3000);
            //observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));

            //原生不能共享
            var coldIntervalSequence = Observable.Interval(TimeSpan.FromMilliseconds(500));
            coldIntervalSequence.Subscribe(i => Console.WriteLine("noShare1 subscription : {0}", i));
            Thread.Sleep(2000);
            coldIntervalSequence.Subscribe(i => Console.WriteLine("noShare2 subscription : {0}", i));
        }

        public static IEnumerable<int> LazyEvaluation()
        {
            Console.WriteLine("About to return 1");
            yield return 1;
            //Execution stops here in this example
            Console.WriteLine("About to return 2");
            yield return 2;

            Console.WriteLine("About to return 3");
            yield return 3;
        }
    
    /// <summary>
    /// 每次订阅的时候都是从0开始-cold序列都是这样子的
    /// </summary>
    public static void ColdObservable()
        {
            var coldSource = Observable.Interval(TimeSpan.FromMilliseconds(300));
                //.Sample(TimeSpan.FromMilliseconds(500))
                coldSource.Subscribe(number => {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"subscribe1 {number}, threadid:{Thread.CurrentThread.ManagedThreadId}");
                    Console.ResetColor();
                    Thread.Sleep(10);
                });

            Thread.Sleep(3000);
            coldSource.Subscribe(number => {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"subscribe2 {number}, threadid:{Thread.CurrentThread.ManagedThreadId}");
                Console.ResetColor();
                Thread.Sleep(10);
            });
        }
    }
}
