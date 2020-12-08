using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Text;

namespace EfCore
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseLoggerFactory(ConsoleLoggerFactory)
            .UseSqlite("Data Source=D:\\Projects\\NetCore3-1\\EfCore\\blogging.db");

        /// <summary>
        /// 添加日志
        /// </summary>
        public static readonly ILoggerFactory ConsoleLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name 
            && level == LogLevel.Information)
            .AddConsole();
        });

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().HasData(
                new Blog { BlogId = 1, Url = "http://sample.com", Title="sample" },
                new Blog { BlogId = 2, Url="htto://www.baidu.com",Title="baidu"}
                );
        }
    }






    public class Student
    {
        public int StudentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SName { get; set; }
        [Required, MaxLength(100)]
        public string SNo { get; set; }

        /// <summary>
        /// 这里表示在数据库里的数据类型是  date类型
        /// </summary>
        [Column(TypeName ="date")]
        public DateTime EntrySchoolDate { get; set; }
        public List<Subject> Subjects { get; set; }
    }

    public class Subject { 
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string NanDu { get; set; }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public List<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
