using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EfCore
{
    class Program
    {
        
        static void Main(string[] args)
        {
            using (var db = new BloggingContext())
            {
                //这里如果数据库还没有创建，会报错找不到table的，所以需要添加 openConnection,或者执行 EnsureCreated
                //db.Database.OpenConnection();
                var isCreate = db.Database.EnsureCreated();
                var studentId = 0;
                var studentsFromSql = db.Students.FromSqlInterpolated($"select * from students where StudentId > {studentId}").ToList();
            }

            Console.ReadKey();
        }
    }
}
