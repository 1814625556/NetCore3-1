using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherKnowledge
{
    public class PathSplit
    {
        public static void SplitOne()
        {
            var pt = @"D:\RuiDao-bak\ruidao\HostControl\HostAppDemo\HostAppControl.xaml.cs";

            var arr = pt.Split("\\");

            var fileName = arr[arr.Length - 1];

            pt = @"HostAppControl.xaml.cs";

            arr = pt.Split("\\");

            fileName = arr[arr.Length - 1];
        }
    }
}
