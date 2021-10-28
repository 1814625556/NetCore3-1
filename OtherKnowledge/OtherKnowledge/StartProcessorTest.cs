using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherKnowledge
{
    public class StartProcessorTest
    {
        public static void Test()
        {
            Process.Start(@"D:\Files\Qt-vtk\VTKServer.exe",
               new[] { "{\"Method\":\"SetWindow\",\"Parameter\":{\"Height\":495,\"Width\":712,\"Left\":772,\"Top\":555,\"Hidden\":0}}" });
            Console.ReadKey();
        }
    }
}
