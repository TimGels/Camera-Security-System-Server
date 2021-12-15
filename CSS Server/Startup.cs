using CSS_Server.JsonProvider;
using CSS_Server.Models;
using CSS_Server.Models.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CSS_Server
{
    public class Startup
    {
        /// <summary>
        /// Can be used to retreive values from appsettings(.Development).json.
        /// </summary>
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add
        /// services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<CameraJsonProvider>();
            services.AddSingleton<CameraManager>();
            services.AddControllersWithViews()
                .AddNewtonsoftJson()
                .AddRazorRuntimeCompilation();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                });

            services.AddTransient<AuthenticationManager>();

            //services.AddRazorPages();

            //services.AddControllers();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure
        /// the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Error");
            //    // Default HSTS is 30 days. Change for production: https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}


            //Add to each response the Cache-Control no cache header to precent back button
            //feature on the client side after logging out succesfully.
            app.Use((context, next) =>
            {
                context.Response.Headers["Cache-Control"] = "no-store,no-cache";
                return next.Invoke();
            });

            //app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120)
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
                //endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Camera}/{action=Index}/{id?}");
            });
        }
    }
}
