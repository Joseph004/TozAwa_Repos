using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TozAwa.Client.Portal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
             {
                 // Set properties and call methods on options
             })
#if (DEBUG)
                        .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
#endif
            .UseStartup<Startup>();
                });
    }
}
