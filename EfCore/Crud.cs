using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EfCore
{
    public static class Crud
    {
        public static void Create(BloggingContext db)
        {
            Console.WriteLine("Inserting a new blog");
            db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet", Title = "demo display" });
            db.SaveChanges();
        }

        public static void Update(BloggingContext db)
        {
            var blog = db.Blogs
                    .OrderBy(b => b.BlogId)
                    .First();
            blog.Url = "https://devblogs.microsoft.com/dotnet";
            blog.Posts.Add(
                new Post
                {
                    Title = "Hello World",
                    Content = "I wrote an app using EF Core!"
                });
            db.SaveChanges();
        }

        /// <summary>
        /// 这里只能删除被追踪的数据  所以需要先查找才可以
        /// </summary>
        /// <param name="db"></param>
        public static void Delete(BloggingContext db)
        {
            var blog = db.Blogs
                    .OrderBy(b => b.BlogId)
                    .First();
            db.Blogs.Remove(blog);
            db.SaveChanges();
        }

        public static void UpdateNotracting(BloggingContext db)
        {
            var cstu = db.Students.AsNoTracking().First();
            cstu.SName += "6688";
            db.Students.Update(cstu);
            db.SaveChanges();
        }

        //级联查询和添加
        //var student = new Student()
        //{
        //    SName = "dy",
        //    SNo = "B13040719"
        //};
        //db.Students.Add(student);
        //        db.SaveChanges();


        //        student.Subjects = new System.Collections.Generic.List<Subject>()
        //        {
        //            new Subject() { SubjectName = "Math"},
        //            new Subject() { SubjectName = "Chinese"},
        //        };

        //EF函数
        //var list = db.Students.Where(x => EF.Functions.Like(x.SName, "%enc%")).ToList();

        //查询关联数据
        //var studentNoInclude = db.Students.ToList();
        //var student = db.Students.Include(x => x.Subjects).ToList();
    }
}
