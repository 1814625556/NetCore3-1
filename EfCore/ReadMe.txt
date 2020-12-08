1:添加安装包
2:查看帮助信息：get-help entityframework
3:每一种数据库可以应用的数据迁移能力是不一样的
4:常用的操作：Add-Migration xxx,update-database,script-migration
5:什么是数据迁移
	当应用在生产环境中运行时，应用通常会存储需要保留的数据。 
	每当发生更改（例如添加新列）时，应用都无法在具有测试数据库的环境下启动。 
	EF Core 迁移功能通过启用 EF Core 更新数据库架构而不是创建新数据库来解决此问题。
	数据模型更改时，迁移不会删除并重新创建数据库，而是更新架构并保留现有数据。
6:每次添加迁移的时候 快照也是会发生变化的

7：通过调用 Add Update Remove Attach这些方法可以变化追踪
	attach的方式局不变化非常小

8：dbcontext 只能追踪 dbset定义的类

9: 执行sql语句查询--两种方式，可以通过查看输出来判断
	var studentsFromSql = db.Students.FromSqlRaw("select * from students").ToList();
	var studentsFromSql = db.Students.FromSqlInterpolated($"select * from students where StudentId > {studentId}").ToList();
10：执行非查询类的sql语句
    Context.Database.ExecuteSQLRaw()
	Context.Database.ExecuteSQLInterpolated()
11: 使用EF函数
		var list = db.Students.Where(x => EF.Functions.Like(x.SName, "%enc%")).ToList();

12：查询关联数据
        var studentNoInclude = db.Students.ToList();
        var student = db.Students.Include(x => x.Subjects).ToList();
	