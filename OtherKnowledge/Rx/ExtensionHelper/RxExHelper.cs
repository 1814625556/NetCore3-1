using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace Rx.ExtensionHelper
{
    public static class RxExHelper
    {
        public static void Dump<T>(this IObservable<T> source, string name)
        {
            source.Subscribe(
                i => Console.WriteLine("{0}-->{1}", name, i),
                ex => Console.WriteLine("{0} failed-->{1}", name, ex.Message),
                () => Console.WriteLine("{0} completed", name));
        }

        public static IDisposable SubscribeX<T>(this IObservable<T> source)
        {
            return source.Subscribe(
                i => { Console.WriteLine($"OnNext:{i}"); },
                ex => Console.WriteLine($"OnError msg:{ex?.Message}"),
                () => Console.WriteLine("OnCompleted"));
        }

        public static IDisposable SubscribeTime<T>(this IObservable<T> source)
        {
            return source.Subscribe(
                i => { Console.WriteLine($"{DateTime.Now.TimeOfDay} - OnNext:{i}"); },
                ex => Console.WriteLine($"{DateTime.Now.TimeOfDay} - OnError msg:{ex?.Message}"),
                () => Console.WriteLine($"{DateTime.Now.TimeOfDay} - OnCompleted"));
        }

        public static IDisposable SubscribeNoError<T>(this IObservable<T> source)
        {
            return source.Subscribe(
                i => { Console.WriteLine($"OnNext:{i}"); },
                //ex => Console.WriteLine($"OnError msg:{ex?.Message}"),
                () => Console.WriteLine("OnCompleted"));
        }

        public static IObservable<T> MyFinally<T>(
            this IObservable<T> source, Action finallyAction)
        {
            return Observable.Create<T>(o =>
            {
                var finallyOnce = Disposable.Create(finallyAction);
                var subscription = source.Subscribe(
                o.OnNext,
                ex =>
                {
                    try { o.OnError(ex); }
                    finally { finallyOnce.Dispose(); }
                },
                () =>
                {
                    try { o.OnCompleted(); }
                    finally { finallyOnce.Dispose(); }
                });
                return new CompositeDisposable(subscription, finallyOnce);
            });
        }

    }
}
