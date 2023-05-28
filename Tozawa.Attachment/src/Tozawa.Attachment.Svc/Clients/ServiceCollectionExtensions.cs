using Microsoft.Extensions.DependencyInjection;
using Tozawa.Attachment.Svc.Converters;

namespace Tozawa.Attachment.Svc.Clients
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterClients(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection RegisterConverters(this IServiceCollection services)
        {
            services.AddScoped<IFileAttachmentConverter, FileAttachmentConverter>();
            services.AddScoped<IFileAttachmentCreator, FileAttachmentCreator>();
            return services;
        }
    }
}
