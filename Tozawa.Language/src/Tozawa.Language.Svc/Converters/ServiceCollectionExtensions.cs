using Microsoft.Extensions.DependencyInjection;

namespace Tozawa.Language.Svc.Converters
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterConverters(this IServiceCollection services)
        {
            services.AddScoped<ITranslatedTextConverter, TranslatedTextConverter>();
            return services;
        }
    }
}
