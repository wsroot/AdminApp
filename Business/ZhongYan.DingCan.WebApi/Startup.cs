using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AdminApp.Helper.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using ZhongYan.DingCan.WebApi.Models;

namespace ZhongYan.DingCan.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration=configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<List<DingCanSettings>>(Configuration.GetSection("DingCanSetting"));
            AddSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json","燕山+订餐接口文档");
                c.DocumentTitle="燕山+订餐接口文档";
            });
            app.UseStatusCodePages();
            app.UseMvc();
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1",new Info
                {
                    Version="v1",
                    Title="燕山+订餐接口文档"
                });
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                x.AddSecurityRequirement(new Dictionary<string,IEnumerable<string>> { { "工作证号",new string [ ] { } } });
                x.OperationFilter<SwaggerHeaderFilter>();
                x.AddSecurityDefinition("工作证号",new ApiKeyScheme
                {
                    Name="Authorization",
                    In="header",
                    Type="apiKey"
                });
            });
        }
    }
}
