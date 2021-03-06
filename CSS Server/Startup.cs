using CSS_Server.JsonProvider;
using CSS_Server.Models;
using CSS_Server.Models.Authentication;
using CSS_Server.Models.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

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

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddDbContext<CSSContext>(options => 
                options.UseSqlite(CSSContext.connectionString));

            services.AddDefaultIdentity<User>(options => 
            {
                    options.SignIn.RequireConfirmedAccount = false;
            }).AddEntityFrameworkStores<CSSContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;

                // Email confirmed not required setting.
                options.SignIn.RequireConfirmedEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Login";
                options.SlidingExpiration = true;
            });

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
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Content-Security-Policy"] = "default-src 'none'; connect-src 'self'; style-src-elem 'self' https://fonts.gstatic.com https://fonts.googleapis.com; style-src 'self' https://fonts.googleapis.com; img-src 'self'; font-src 'self' https://fonts.gstatic.com; script-src 'self'; frame-ancestors 'none'; form-action 'self'";
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                return next.Invoke();
            });

            app.UseCookiePolicy(new CookiePolicyOptions {
                Secure = CookieSecurePolicy.Always
            });

            //app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(30)
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
