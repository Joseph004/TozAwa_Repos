using System.Net;
using System.Web;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.HttpClients
{
    public interface IAttachmentHttpClient : IHttpClientBase
    {
    }

    public class AttachmentHttpClient : HttpClientBase, IAttachmentHttpClient
    {
        public AttachmentHttpClient(HttpClient client, AppSettings appSettings, ICurrentUserService currentUserService, IUserTokenService userTokenService, ILogger<AttachmentHttpClient> logger) : base(client, appSettings, currentUserService, userTokenService, logger)
        {
        }
    }
}

