using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Tozawa.Attachment.Svc.Validation
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterValidationService(this IServiceCollection services)
        {
            services.AddScoped<IValidatorFactory, FluentValidatorFactory>();
            services.AddScoped<IValidationService, FluentValidationService>();

            return services;
        }
    }
}
