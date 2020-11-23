using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using ThreeApi.Data;
using ThreeApi.Entities;
using ThreeApi.Services;

namespace ThreeApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //����Э��--Header ����� Accept
            services.AddControllers(setup=> {
                setup.ReturnHttpNotAcceptable = true;
            })
            //����json,���������� addxml ����֧�� xml���͵����ݷ���
            .AddNewtonsoftJson(setup =>{
                setup.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()
            //��� 422 ������Ӧ
            .ConfigureApiBehaviorOptions(setup =>
            {
                setup.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "http://www.baidu.com",
                        Title = "�д��󣡣���",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "�뿴��ϸ��Ϣ",
                        Instance = context.HttpContext.Request.Path
                    };

                    problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            }); ;


            services.AddDbContext<RoutineDbContext>(option =>
            {
                option.UseSqlite("Data Source=routine.db");
            });

            //���AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

            //ÿ�� http���󶼻��½�һ��ʵ��
            services.AddScoped<ICompanyRepository, CompanyRepository>();

            //��ȡ�����ļ�
            var nickName = Configuration.GetSection("NickName").Value;
            var students = Configuration.GetSection("Students").GetChildren();
            var num = Configuration.GetSection("Students").GetSection("Number").Value;
            num = Configuration["Students:Number"];

            //�����ļ�������ע�� ע����� CompaniesController ������
            services.Configure<StudentOptions>(Configuration.GetSection("Students"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            
            //�����������error��ʱ����Լ�¼��־
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("UseExceptionHandler Unexpected Error!");
                });
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}
