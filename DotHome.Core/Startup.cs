using DotHome.Core.Data;
using DotHome.Core.Hubs;
using DotHome.Core.Services;
using DotHome.Core.Tools;
using DotHome.RunningModel;
using DotHome.StandardBlocks.Services;
using LettuceEncrypt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotHome.Core
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
            if (System.Runtime.InteropServices.RuntimeInformation.OSArchitecture == System.Runtime.InteropServices.Architecture.Arm)
            {
                services.AddLettuceEncrypt(c => { c.DomainNames = new[] { "zeleznicarska34.duckdns.org" }; c.AcceptTermsOfService = true; c.EmailAddress = "vojta.luk@seznam.cz"; });
            }
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Events.OnRedirectToLogin = c =>
                        {
                            if (c.Request.Path.StartsWithSegments("/config")) c.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        };
                    });
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSignalR().AddNewtonsoftJsonProtocol();
            services.AddServerSideBlazor();

            services.AddHttpContextAccessor();

            services.AddDbContext<NotificationsStoreContext>(o => o.UseSqlite("Data Source=notifications.db"), ServiceLifetime.Singleton);

            services.AddSingleton<NotificationManager>();
            services.AddSingleton<INotificationSender>(sp => sp.GetService<NotificationManager>());
            //services.AddSingleton<INotificationSender>(sp => sp.GetRequiredService<NotificationManager>());
            services.AddSingleton<PageReloader>();
            services.AddSingleton<ModelLoader>();
            services.AddSingleton<BlocksActivator>();
            services.AddSingleton<IProgramCore, BasicProgramCore>();

            //services.AddSingleton<UDPCommunicationProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub();
                endpoints.MapControllers();
                endpoints.MapHub<DebuggingHub>("debug");
                endpoints.MapFallbackToPage("/_Host");
            });

            app.ApplicationServices.GetService<IProgramCore>();
            //app.ApplicationServices.GetService<UDPCommunicationProvider>().SearchDevices();
        }
    }
}
