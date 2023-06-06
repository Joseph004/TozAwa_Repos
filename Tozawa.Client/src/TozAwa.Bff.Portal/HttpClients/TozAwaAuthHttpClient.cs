using System.Net;
using System.Web;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.HttpClients
{
    public interface ITozAwaAuthHttpClient : IHttpClientBase
    {
    }

    public class TozAwaAuthHttpClient : HttpClientBase, ITozAwaAuthHttpClient
    {
        public TozAwaAuthHttpClient(HttpClient client, AppSettings appSettings, ICurrentUserService currentUserService, IUserTokenService userTokenService, ILogger<TozAwaAuthHttpClient> logger) : base(client, appSettings, currentUserService, userTokenService, logger)
        {
        }
    }
}

