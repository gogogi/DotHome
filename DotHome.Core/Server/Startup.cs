using DotHome.Core.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                 options =>
                 {
                     options.Events.OnRedirectToLogin = c =>
                     {
                         if (c.Request.Path.StartsWithSegments("/config")) c.Response.StatusCode = 401;
                         return Task.CompletedTask;
                     };
                     options.LoginPath = new PathString("/login");
                     options.AccessDeniedPath = new PathString("/config/download");
                 });
            services.AddControllersWithViews();
            services.AddRazorPages();
            

            //services.AddSession();

            services.AddSingleton<ProgrammingModelLoader>();
            services.AddSingleton<IProgramCore, BasicProgramCore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseSession();
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                //endpoints.MapFallbackToFile("index.html");
            });

            app.ApplicationServices.GetService<IProgramCore>();  // To force the service provider to create an instance
        }
    }
}
