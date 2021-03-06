using CSS_Server.Controllers;
using CSS_Server.Models.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SQLitePCL;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CSS_Server.Models.Database
{
    public class CSSContext : IdentityDbContext<User>
    {
        public static readonly string connectionString = GetConnectionString();
        public CSSContext(DbContextOptions<CSSContext> options) : base(options)
        {

        }

        public DbSet<Log> Logs { get; set; }
        public DbSet<Camera> Cameras { get; set; }

        public static CSSContext GetContext()
        {
            DbContextOptionsBuilder<CSSContext> optionsBuilder = new();
            optionsBuilder.UseSqlite(connectionString);
            return new CSSContext(optionsBuilder.Options);
        }

        private static string GetConnectionString()
        {
            //The database file path is set to the same folder as the executable + "database.db"
            string databaseFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "database.db");

            //read the key from the configuration. This configuration is set in Program.cs and is initialized after the IHostBuilder build the app.
            string key = Startup.Configuration["DATABASE_KEY"];

            //Check if key is not set. If so, stop further execution of the app.
            if (key == null)
            {
                StopApplication("No DATABASE_KEY set, set it with the environment vars or with secrets.json");
            }

            SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder()
            {
                DataSource = databaseFilePath,
                Password = key,
            };

            return builder.ToString();
        }

        private static void StopApplication(string message)
        {
            if (message != null && message != string.Empty)
            {
                Debug.WriteLine(message);
                Console.WriteLine(message);
            }
            Environment.Exit(1);
        }

        public static void InitializeDatabase(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<CSSContext>();
                    context.Database.EnsureCreated();
                    //Set the needStupAccount prop for the account controller.
                    AccountController.needSetupAccount = !context.Users.Any();
                }
                catch (SqliteException ex) when (ex.SqliteErrorCode == 26) //SQLite Error 26: 'file is not a database'.
                {
                    //Happens when the given encryption key is incorrect.
                    StopApplication("Given file is not a database. This is probably due a wrong encryption key.");
                }
            }
        }
    }
}
