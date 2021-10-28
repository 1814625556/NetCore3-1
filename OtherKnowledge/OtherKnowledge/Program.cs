using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace OtherKnowledge
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTimeOffTest.Test();
            Console.ReadKey();
        }

        static async Task ExcuteAsync()
        {
            await MultiThreadException.AwaitTask();
        }

    }

}
