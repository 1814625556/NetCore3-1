using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace Rx
{

    class ObservableAndObserveTest
    {
        public static void Test()
        {
        }

    }

    /// <summary>
    /// 这里展示了 rx 的原理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyConsoleObserver<T> : IObserver<T>
    {
        public void OnNext(T value)
        {
            Console.WriteLine("Received value {0}", value);
        }
        public void OnError(Exception error)
        {
            Console.WriteLine("Sequence faulted with {0}", error.Message);
        }
        public void OnCompleted()
        {
            Console.WriteLine("Sequence terminated");
        }
    }
    public class MySequenceOfNumbers : IObservable<int>
    {
        public IDisposable Subscribe(IObserver<int> observer)
        {
            observer.OnNext(1);
            observer.OnNext(2);
            observer.OnNext(3);
            observer.OnCompleted();
            observer.OnNext(4);
            observer.OnNext(5);
            observer.OnError(new Exception("ex"));
            observer.OnNext(6);
            observer.OnNext(7);
            return Disposable.Empty;
        }
    }
    
}
