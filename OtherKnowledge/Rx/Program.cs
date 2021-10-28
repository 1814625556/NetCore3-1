using System;
using System.Threading;
using System.Reactive.Linq;

namespace Rx
{
    class Program
    {
        static void Main(string[] args)
        {
            HardOperatesTest.Test();
            //ColdAndHotObservable.Test();
            //ObservableAndObserveTest.Test();
            //SubjectTest.Test();
            //ReduceSequenceTest.Test();
            //InspectionTest.Test();
            //TransformationTest.Test();
            //Chapter3SideTest.Test();
            //ErrorHandlerTest.Test();
            //ComBiningTest.Test();
            //TimeShiftedTest.Test();

            Console.ReadKey();
        }

    }
}
