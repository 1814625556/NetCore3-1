using System;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace RxWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var loadStream = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(handler => this.MainWin.Loaded += handler,
                handler => this.MainWin.Loaded -= handler);
            loadStream.Subscribe(WhenLoaded);
        }

        /// <summary>
        /// 当页面加载完毕之后执行的操作
        /// </summary>
        /// <param name="args"></param>
        public void WhenLoaded(EventPattern<RoutedEventArgs> args)
        {
            //鼠标移动事件
            var mouseStream = Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => this.MainWin.MouseMove += h,
                h => this.MainWin.MouseMove -= h);
            mouseStream.Sample(TimeSpan.FromMilliseconds(500)).ObserveOnDispatcher().Do(args =>
            {
                var pt = args.EventArgs.GetPosition(this);
                this.Txtblock.Text = $"({pt.X},{pt.Y})";
            }).Publish().Connect();

            //test Throttle method
            //mouseStream.Throttle(TimeSpan.FromMilliseconds(500)).ObserveOnDispatcher().Do(args =>
            //{
            //    var pt = args.EventArgs.GetPosition(this);
            //    this.Txtblock.Text = $"({pt.X},{pt.Y})";
            //}).Publish().Connect();


            #region 三种转换事件成Observable流的方式
            //得到了Button的Click事件流。
            var clickedStream = Observable.FromEventPattern<EventArgs>(this.rxClick, "Click");

            //这种方式比较高效
            var clickedStream2 = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                h => rxClick.Click += h, h => rxClick.Click -= h);

            //这个方法其实 是做了一个监听 每次用户触发click事件的时候 就会 执行h方法 然后h方法里面执行 onnext方法
            var cliekedStream3 = Observable.FromEvent<RoutedEventHandler, string>(
                h => (object sender, RoutedEventArgs args) =>
                {
                    MessageBox.Show($"fromEvent threadId:{Thread.CurrentThread.ManagedThreadId}");
                    //只有执行了这段代码才能继续执行subscribe里面的OnNext方法
                    h(args.OriginalSource.ToString());
                    //也可以这么调用
                    //h.Invoke(args.OriginalSource.ToString());
                },
                h => rxClick.Click += h,
                h => rxClick.Click -= h);
            #endregion

            //在事件流上注册了一个观察者。 
            //clickedStream.Subscribe(e => MessageBox.Show("Hello world"))；
            var clickedStreamDispose = clickedStream2.Subscribe(e =>
            {
                MessageBox.Show("cliekedStream2");
            });
            cliekedStream3
                .SubscribeOn(NewThreadScheduler.Default)
                .Subscribe(str =>
                {
                    MessageBox.Show($"clickedStream3-{str}");
                });

            //unsubscribe
            Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => this.DisposeClick.Click += h,
                h => this.DisposeClick.Click -= h).Subscribe(args => clickedStreamDispose.Dispose());

            //放到外面就可以获取到了 ， 在Scheduler.Default里面获取不到
            var dScheduler = DispatcherScheduler.Current;
            Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => this.DiffThreadGetUiDispater.Click += h,
                h => this.DiffThreadGetUiDispater.Click -= h).Subscribe(args => {
                    var fileLines = Observable.Create<string>(
                    o =>
                    {
                        //var dScheduler = DispatcherScheduler.Current;
                        //var dScheduler = Scheduler.Default;
                        var lines = File.ReadAllLines(@"D:\WorkSpace\ruidao\OpenCV\demo01\chapter2.py");
                        foreach (var line in lines)
                        {
                            var localLine = line;
                            dScheduler.Schedule(
                            () => o.OnNext(localLine));
                        }
                        return Disposable.Empty;
                    });
                    fileLines
                        .SubscribeOn(Scheduler.Default)
                        //获取UI线程 dispatcher的方法
                        //.ObserveOnDispatcher()
                        //.ObserveOn(DispatcherScheduler.Current)
                        .Subscribe(line => {
                            Txtblock.Text += line;
                        });
                });


            //hot observable
            var hotObservable = Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
                h => (object sender, RoutedEventArgs args) =>
                {
                    MessageBox.Show($"fromEvent threadId:{Thread.CurrentThread.ManagedThreadId}");
                    h(args);
                },
            h => HotButton.Click += h,
                h => HotButton.Click -= h);
            //这里如果只是写Do函数的话，没有订阅 就不会有Do函数的执行
            //hotObservable.Do(args => { MessageBox.Show("hot obsevable"); }).Subscribe(args => { });

            //先转化成 IConnecteableObservable 然后再Connect 可以执行 Do方法
            var publicObservable = hotObservable.Do(args =>
            {
                MessageBox.Show($"Do method threadId:{Thread.CurrentThread.ManagedThreadId}");
            }).ObserveOn(Scheduler.Default).ObserveOn(Scheduler.Default).Publish();
            publicObservable.Subscribe(args =>
            {
                MessageBox.Show($"Subscribe threadId:{Thread.CurrentThread.ManagedThreadId}");
                
            });

            //执行这行代码的时候就才会发射序列，使用public方法之后 序列的发送不再依靠 subscribe 方法，而是依靠Connect方法。
            var connectDispose = publicObservable.Connect();
        }
    }
}
