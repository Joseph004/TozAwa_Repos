using Microsoft.Extensions.Logging;
using Grains.Configurations;

namespace Grains.Services
{
    public interface IEmailMessageService
    {
        Task SendNewPassword(string email, string password);
    }
    public class EmailMessageService(AppSettings appSettings, ILogger<EmailMessageService> logger) : IEmailMessageService
    {
        private readonly ILogger<EmailMessageService> _logger = logger;
        private readonly AppSettings _appSettings = appSettings;

        public async Task SendNewPassword(string email, string password)
        {
            await Task.CompletedTask;
        }
    }
}