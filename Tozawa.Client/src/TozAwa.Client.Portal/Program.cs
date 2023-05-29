using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TozAwa.Client.Portal
{
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
                .AddJsonFile("CustomerSettings/appsettings.json", false)
                .AddJsonFile($"CustomerSettings/appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args)
                .Build();
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                        {
                            // Set properties and call methods on options
                        })
#if (DEBUG)
                        .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
#endif
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
                    //builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("", LogLevel.Information);
                });
        }
    }
}
