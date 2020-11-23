using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThreeApi.Data;

namespace ThreeApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    var dbContext = scope.ServiceProvider.GetService<RoutineDbContext>();
                    bool flag = dbContext.Database.EnsureCreated();
                    dbContext.Database.Migrate();
                    logger.LogWarning($"Database Migration Success! {flag}");
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Database Migration Error!");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((context,configBuilder)=> {
                configBuilder.Sources.Clear();
                //添加自定义json配置文件
                configBuilder.AddJsonFile("settings/cc.json");
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
