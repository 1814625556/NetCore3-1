using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherKnowledge
{
     /// <summary>
     /// Const和Readonly的最大区别(除语法外)
　　 ///Const的变量是嵌入在IL代码中，编译时就加载好，不依赖外部dll（这也是为什么不能在构造方法中赋值）。
   ///Const在程序集更新时容易产生版本不一致的情况。
     ///Readonly的变量是在运行时加载，需请求加载dll，每次都获取最新的值。Readonly赋值引用类型以后，
     ///引用本身不可以改变，但是引用所指向的实例的值是可以改变的。在构造方法中，我们可以多次对Readonly赋值。
     /// </summary>
    public class ReadOnlyAndConstTest
    {
        /// <summary>
        /// 静态常量 属于类，不属于对象
        /// </summary>
        public const int Age=0;
        public readonly int Cars = 0;
        public ReadOnlyAndConstTest()
        {
            //Age = 10;
            Cars = 10;
        }
    }
}
