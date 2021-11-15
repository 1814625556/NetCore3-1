using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OtherKnowledge
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = new DirectoryInfo(@"D:\WorkSpace\guidance\src\Guidance\bin\x64\Debug\net5.0-windows");
            var files = dir.GetFiles("*.avi");

            foreach (var f in files)
            {
                Console.WriteLine(f.FullName);
                //File.Delete(f.FullName);
            }

            Console.ReadKey();
        }

        static async Task ExcuteAsync()
        {
            await MultiThreadException.AwaitTask();
        }

    }

}
