using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Services;

namespace Tozawa.Attachment.Svc;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddCommandLine(args)
            .Build();
        return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        // Set properties and call methods on options
                    })
                    .UseStartup<Startup>();
            })
            .ConfigureAppConfiguration((context, builder) =>
            {
                var env = context.HostingEnvironment;
                builder
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureLogging((context, builder) =>
            {
                builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("", LogLevel.Information);
            });
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AttachmentContext>
    {
        public AttachmentContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(
                    new FileInfo(new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
                        .AbsolutePath).Directory?.FullName.Replace("%20", " "))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AttachmentContext>();

            optionsBuilder.UseSqlServer(configuration.GetValue<string>("AppSettings:ConnectionStrings:MigrationSql"),
                x =>
                {
                    x.MigrationsHistoryTable("MigrationHistory", "Attachment");
                    x.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), new List<int>());
                    x.CommandTimeout(300);
                });

            return new AttachmentContext(new CurrentUserService(), optionsBuilder.Options);
        }
    }
}
