using AdminApp.Helper.Models;
using AdminApp.Helper.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdminApp.Permission
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
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            AddAuthentication(services);
            AddSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json","权限服务器接口文档");
                c.DocumentTitle="权限服务器接口文档";
            });
            app.UseStatusCodePages();
            app.UseMvc();
        }

        #region SerivceAdd


        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Events=new JwtBearerEvents
                {
                    OnMessageReceived=context =>
                    {
                        context.Token=context.Request.Query ["access_token"];
                        return Task.CompletedTask;
                    }
                };
                o.TokenValidationParameters=new TokenValidationParameters
                {
                    //验证SymmetricSecurityKey与生成Token需一至
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey=
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration ["JwtSettings:Key"])),

                    ValidateActor=false,
                    //验证Audience与生成Token需一至
                    ValidateAudience=false,
                    ValidAudience=Configuration ["JwtSettings:Audience"],
                    //验证Issuer与生成Token需一至
                    ValidateIssuer=true,
                    ValidIssuer=Configuration ["JwtSettings:Issuer"],
                    // 是否要求Token的Claims中必须包含Expires
                    RequireExpirationTime=true,
                    // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                    ValidateLifetime=false,
                    ValidateTokenReplay=false
                };
            });
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1",new Info
                {
                    Version="v1",
                    Title="权限服务器接口文档"
                });
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                x.AddSecurityRequirement(new Dictionary<string,IEnumerable<string>> { { "Bearer",new string [ ] { } } });
                x.AddSecurityDefinition("Bearer",new ApiKeyScheme
                {
                    Name="Authorization", //jwt默认的参数名称
                    In="header", //jwt默认存放Authorization信息的位置(请求头中)
                    Type="apiKey"
                });
                x.OperationFilter<SwaggerHeaderFilter>();
            });
        }
        #endregion
    }
}
