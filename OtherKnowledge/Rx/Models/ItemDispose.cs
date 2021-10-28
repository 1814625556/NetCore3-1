using System;
using System.Collections.Generic;
using System.Text;

namespace Rx.Models
{
    public class ItemDispose : IDisposable
    {
        public string Msg { get; set; }
        public ItemDispose(string msg)
        {
            Msg = msg;
        }
        public void Dispose()
        {
            Console.WriteLine($"ItemDispose...");
        }
    }
}
