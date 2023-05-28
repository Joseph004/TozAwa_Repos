
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tozawa.Language.Svc.Configuration;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.extension
{
    public static class DatabaseEx
    {
        public static void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app
                .ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            var appSettings = serviceScope.ServiceProvider.GetService<AppSettings>();
            var currentUserService = serviceScope.ServiceProvider.GetService<ICurrentUserService>();
            if (appSettings == null) return;
            var options = new DbContextOptionsBuilder<LanguageContext>()
                .UseSqlServer(appSettings.ConnectionStrings.MigrationSql,
                    x =>
                    {
                        x.MigrationsHistoryTable("MigrationHistory", "Tozawa.Language");
                        x.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), new List<int>());
                    });

            /* IOptions<AppSettings> optionSettings = Options.Create(appSettings); */

            using var context = new LanguageContext(options.Options, currentUserService);
            context.Database.SetCommandTimeout(600);
            context.Database.Migrate();
        }
    }

}


