using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GMAPI.Models;
using Microsoft.EntityFrameworkCore;
using GMAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.CodeAnalysis.FlowAnalysis;
using GMAPI.Dtos;
using AutoMapper;
using GMAPI.Other;

namespace GMAPI
{


    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            

            //configuration["hosturl"] = "https://192.168.178.50:5000";
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAutoMapper(typeof(AccountProfile));

            var dbConfig = 


            services.AddEntityFrameworkNpgsql().AddDbContext<PostgresDatabaseContext>(opt =>
            {
                opt.UseNpgsql(Configuration.GetConnectionString("MyWebApiConnection"));
            });
            /*services.AddDbContext<TodoContext>(opt =>
            opt.UseInMemoryDatabase("TodoList"));*/
            services.AddControllers();
            //services.AddAuthorization();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
            });

            services.AddMvc().AddControllersAsServices();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("Appsettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           

            app.UseForwardedHeaders();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

          
            //TODO Enable
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseAuthorization();
            
            app.UseMiddleware<RequestLoggingMiddleware>();


            //TODO herzien cors
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
