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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using IfcCreator.Interface;
using IfcCreator.ExceptionHandling;
using IfcCreator.HTTP;

namespace IfcCreator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<ApiBehaviorOptions>(options =>
                {
                    options.InvalidModelStateResponseFactory = ctx => new ValidationErrorActionResult();
                });
            services.AddCors(options => 
                {
                    options.AddPolicy(MyAllowSpecificOrigins,
                                      builder => 
                                      {
                                          builder.AllowAnyOrigin();
                                          builder.AllowAnyMethod();
                                          builder.AllowAnyHeader();
                                      });
                });
            services.AddSingleton<IProductIfcCreator, ProductIfcCreator>();
            services.AddSingleton<IResponseHelper, ResponseHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseCors(MyAllowSpecificOrigins);
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.ConfigureGlobalExceptionMiddleware();
            app.UseMvc();
        }
    }
}
