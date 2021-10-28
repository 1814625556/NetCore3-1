using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;

namespace Rx
{
    //Rx 只是一种将给定通知的回调链接在一起的方式
    //wpf中ui上面的调度器 DispatcherScheduler.Instance

    //NewThreadScheduler：即在新线程上执行
    //ThreadPoolScheduler：即在线程池中执行--已经废弃
    //TaskPoolScheduler：与ThreadPoolScheduler类似
    //CurrentThreadScheduler：在当前线程执行
    //ImmediateScheduler：在当前线程立即执行
    //EventLoopScheduler：创建一个后台线程按序执行所有操作

    //wpf
    //.ObserveOnDispatcher()
    //.ObserveOn(DispatcherScheduler.Current)


    public class SchedulerTest
    {
        public static void UsingScheduler()
        {
            Console.WriteLine("Starting on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
            var source = Observable.Create<int>(
            o =>
            {
                Console.WriteLine("Invoked on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                o.OnNext(1);
                o.OnNext(2);
                o.OnNext(3);
                o.OnCompleted();
                Console.WriteLine("Finished on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                return Disposable.Empty;
            });
            source
            //.SubscribeOn(NewThreadScheduler.Default) == .SubscribeOn(Scheduler.NewThread)->过期
            //.SubscribeOn(ThreadPoolScheduler.Instance)
            //.SubscribeOn(Scheduler.Default)
            .Subscribe(
            o => Console.WriteLine("Received {1} on threadId:{0}", Thread.CurrentThread.ManagedThreadId, o),
            () => Console.WriteLine("OnCompleted on threadId:{0}", Thread.CurrentThread.ManagedThreadId));
            Console.WriteLine("Subscribed on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
        }

        public static void DifferenceBetweenSubscribeOnAndObserveOn()
        {
            Thread.CurrentThread.Name = "Main";

            IScheduler thread1 = new NewThreadScheduler(x => new Thread(x)
            { Name = "Thread1" });
            IScheduler thread2 = new NewThreadScheduler(x => new Thread(x)
            { Name = "Thread2" });

            Observable.Create<int>(o =>
            {
                Console.WriteLine("Subscribing on " + Thread.CurrentThread.Name);
                o.OnNext(1);
                return Disposable.Create(() => { });
            })
            .SubscribeOn(thread1)
            .ObserveOn(thread2)
            .Subscribe(x => Console.WriteLine("Observing '" + x + "' on " + Thread.CurrentThread.Name));
        }

        //创建一个主题 然后在各个线程上调用，发现
        //每个OnNext都在通知它的同一线程上被回调
        public static void SchedulerDemo1()
        {
            Console.WriteLine("Starting on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
            var subject = new Subject<object>();
            subject.Subscribe(
                o => Console.WriteLine("Received {1} on threadId:{0}",
                    Thread.CurrentThread.ManagedThreadId,
                    o));
            ParameterizedThreadStart notify = obj =>
            {
                Console.WriteLine("OnNext({1}) on threadId:{0}",
                    Thread.CurrentThread.ManagedThreadId,
                    obj);
                subject.OnNext(obj);
            };
            notify(1);
            new Thread(notify).Start(2);
            new Thread(notify).Start(3);
        }
        /// <summary>
        /// 多种调度执行差异
        /// </summary>
        public static void VariesSchedulers()
        {
            Console.WriteLine($"Main threadId: {Thread.CurrentThread.ManagedThreadId}");
            Scheduler.Default.Schedule(() => 
            {
                Console.WriteLine($"Default Schedule: {Thread.CurrentThread.ManagedThreadId}");
            });
            NewThreadScheduler.Default.Schedule(() => {
                Console.WriteLine($"NewThreadScheduler: {Thread.CurrentThread.ManagedThreadId}");
            });
            Scheduler.Immediate.Schedule(() => {
                Console.WriteLine($"Immediate Schedule: {Thread.CurrentThread.ManagedThreadId}");
            });
            Scheduler.CurrentThread.Schedule(() => {
                Console.WriteLine($"CurrentThread Schedule: {Thread.CurrentThread.ManagedThreadId}");
            });
            ThreadPoolScheduler.Instance.Schedule(() => {
                Console.WriteLine($"ThreadPoolScheduler: {Thread.CurrentThread.ManagedThreadId}");
            });
            TaskPoolScheduler.Default.Schedule(() => {
                Console.WriteLine($"TaskPoolScheduler: {Thread.CurrentThread.ManagedThreadId}");
            });

            //  Main threadId: 1
            //  Default Schedule: 4
            //  Immediate Schedule: 1
            //  CurrentThread Schedule: 1
            //  ThreadPoolScheduler: 4
            //  TaskPoolScheduler: 4
            //  NewThreadScheduler: 9
        }

        /// <summary>
        /// 延迟执行
        /// </summary>
        public static void DelayExcuteScheduler()
        {
            var delay = TimeSpan.FromSeconds(1);
            Console.WriteLine("Before schedule at {0:o}", DateTime.Now);
            var token = Scheduler.Default.Schedule(delay,
            () => Console.WriteLine("Inside schedule at {0:o}", DateTime.Now));
            Console.WriteLine("After schedule at  {0:o}", DateTime.Now);
            //token.Dispose(); 取消操作
        }

        #region 
        //ImmediateScheduler 对比 CurrentThreadScheduler,ImmediateScheduler和 
        //CurrentThreadScheduler都不会切换线程
        public void CurrentThreadExample()
        {
            ScheduleTasks(Scheduler.CurrentThread);
            /*Output: 
            outer start. 
            outer end. 
            --innerAction start. 
            --innerAction end. 
            ----leafAction. 
            */
        }

        //立即执行函数 独一份
        public void ImmediateExample()
        {
            ScheduleTasks(Scheduler.Immediate);
            /*Output: 
            outer start. 
            --innerAction start. 
            ----leafAction. 
            --innerAction end. 
            outer end. 
            */
        }

        private static void ScheduleTasks(IScheduler scheduler)
        {
            Action leafAction = () => Console.WriteLine("----leafAction.");
            Action innerAction = () =>
            {
                Console.WriteLine("--innerAction start.");
                scheduler.Schedule(leafAction);
                Console.WriteLine("--innerAction end.");
            };
            Action outerAction = () =>
            {
                Console.WriteLine("outer start.");
                scheduler.Schedule(innerAction);
                Console.WriteLine("outer end.");
            };
            scheduler.Schedule(outerAction);
        }
        #endregion

        //eventLoopTest
        public static void EventLoopTest()
        {
            Console.WriteLine($"MainThreadId:{Thread.CurrentThread.ManagedThreadId}");
            new EventLoopScheduler().Schedule(
                ()=> 
                { 
                    Console.WriteLine($"eventLoopThreadId:{Thread.CurrentThread.ManagedThreadId}"); 
                });

            new EventLoopScheduler( action =>new Thread(action) { Name="eventLoop1"}).Schedule(
                () =>
                {
                    Console.WriteLine($"eventLoopThreadId:{Thread.CurrentThread.ManagedThreadId},name:{Thread.CurrentThread.Name}");
                });
        }



        #region NewThread，ThreadPool，TaskPool 测试,
        //newthread后续的运行都是在同一个线程上，threadpool 
        //和 taskpool有可能是无序的，而且后续的线程也不一定运行在同一个线程上
        public static void NewThreadTest()
        {
            Console.WriteLine("Starting on thread :{0}",
            Thread.CurrentThread.ManagedThreadId);

            //Scheduler.NewThread.Schedule("A", OuterAction);方法已经过期--这里好好看一下 跟eventLoop里面差不多
            NewThreadScheduler.Default.Schedule("A", OuterAction);
        }

        public static void ThreadPoolTest()
        {
            Console.WriteLine("Starting on thread :{0}",
                Thread.CurrentThread.ManagedThreadId);
            //Scheduler.ThreadPool.Schedule("A", OuterAction);
            //Scheduler.ThreadPool.Schedule("B", OuterAction);--已经过时-消除平台api问题 建议将 Scheduler.ThreadPool 转化为 Scheduler.Default
            Scheduler.Default.Schedule("A", OuterAction);
            Scheduler.Default.Schedule("B", OuterAction);
        }

        public static void TaskPoolTest()
        {
            Console.WriteLine("Starting on thread :{0}",
                Thread.CurrentThread.ManagedThreadId);
            //Scheduler.TaskPool.Schedule("A", OuterAction);
            //Scheduler.TaskPool.Schedule("B", OuterAction);
            TaskPoolScheduler.Default.Schedule("A", OuterAction);
            TaskPoolScheduler.Default.Schedule("B", OuterAction);
        }


        private static IDisposable OuterAction(IScheduler scheduler, string state)
        {
            Console.WriteLine("{0} start. ThreadId:{1}",
            state,
            Thread.CurrentThread.ManagedThreadId);
            scheduler.Schedule(state + ".inner", InnerAction);
            Console.WriteLine("{0} end. ThreadId:{1}",
            state,
            Thread.CurrentThread.ManagedThreadId);
            return Disposable.Empty;
        }
        private static IDisposable InnerAction(IScheduler scheduler, string state)
        {
            Console.WriteLine("{0} start. ThreadId:{1}",
            state,
            Thread.CurrentThread.ManagedThreadId);
            scheduler.Schedule(state + ".Leaf", LeafAction);
            Console.WriteLine("{0} end. ThreadId:{1}",
            state,
            Thread.CurrentThread.ManagedThreadId);
            return Disposable.Empty;
        }
        private static IDisposable LeafAction(IScheduler scheduler, string state)
        {
            Console.WriteLine("{0}. ThreadId:{1}",
            state,
            Thread.CurrentThread.ManagedThreadId);
            return Disposable.Empty;
        }
        #endregion
    }
}
