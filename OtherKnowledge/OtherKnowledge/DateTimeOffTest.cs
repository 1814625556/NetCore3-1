using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherKnowledge
{
    public class DateTimeOffTest
    {
        public static void Test()
        {
            DateTimeOffset dts = DateTimeOffset.Now;
            var dt = DateTime.Now;

            Console.WriteLine($"DateTimeOffset:{dts}");
            Console.WriteLine($"DateTimeOffset.LocalTime:{dts.ToLocalTime()}");
            Console.WriteLine($"DateTimeOffset.ToUniversalTime:{dts.ToUniversalTime()}");
            Console.WriteLine($"DateTimeOffset.UtcDateTime:{dts.UtcDateTime}");
            Console.WriteLine($"DateTime:{dt}");
        }
    }
}
