using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherKnowledge
{
    public class OutInTest
    {
        public static void InTest()
        {
            //IBar<object> barObj = new Bar();
            //IBar<string> barStr = barObj;

            IBar<string> barStr = new Bar();
            barStr.Print("nihao");
        }
    }

    #region in
    interface IBar<in T>
    {
        void Print(T content);
    }

    class Bar : IBar<object>
    {
        public void Print(object content)
        {
            Console.WriteLine(content);
        }
    }
    #endregion
}
