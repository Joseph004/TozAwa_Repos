using Microsoft.Extensions.DependencyInjection;

namespace Tozawa.Language.Svc.XliffConverter
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterXliffConveter(this IServiceCollection services)
        {
            services.AddScoped<IImportResultService, ImportResultService>();
            services.AddScoped<IXDocumentToXliff, DocumentToXliff>();
            services.AddScoped<IXliffImporter, XliffImporter>();
            services.AddScoped<IXliffImportTransaction, XliffImportTransaction>();
            services.AddScoped<IXliffExportTransaction, XliffExportTransaction>();
            services.AddScoped<IExporter, Exporter>();
            services.AddScoped<IXliffConverter, XliffConverter>();
            return services;
        }
    }
}
