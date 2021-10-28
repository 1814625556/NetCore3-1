using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherKnowledge
{
    class StringCommonSearch
    {
        public static void Test()
        {
            var str1 = "111";
            var str2 = str1;

            //这里其实是new了一个新对象
            str1 = "222";

            Console.WriteLine($"str1:{str1},str2:{str2}");


            var stu1 = new Student() { Name = "zhangsan", Age = 23 };

            Student stu2 = stu1;

            stu1.Name = "lisi";
            stu1.Age = 26;

            Console.WriteLine($"stu1.name:{stu1.Name}-stu1.age:{stu1.Age},stu2.name:{stu2.Name}-stu2.age:{stu2.Age}");

            var stu3 = new Student() { Name = "ZhangSan", Age = 23 };

            //这种写法相当于string了
            Student stu4 = stu3;
            stu3 = new Student() { Age = 35, Name = "LiSi" };

            Console.WriteLine($"stu3:{stu3.Name} {stu3.Age},stu4:{stu4.Name} {stu4.Age}");
        }
    }
    class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
