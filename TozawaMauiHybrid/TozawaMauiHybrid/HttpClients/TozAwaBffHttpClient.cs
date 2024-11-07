using TozawaMauiHybrid.Configurations;
using TozawaMauiHybrid.Services;
using System.Net;
using TozawaMauiHybrid.Models.ResponseRequests;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TozawaMauiHybrid.Helpers;
using Microsoft.Extensions.Logging;

namespace TozawaMauiHybrid.HttpClients
{
    public interface ITozAwaBffHttpClient
    {
        Task<GetResponse<T>> SendGet<T>(string url, Dictionary<string, string> queryParameters = null) where T : class;
        Task<AddResponse<T>> SendPost<T>(string url, object value) where T : class;
        Task<AddResponse<T>> SendPost02<T>(string url, object value) where T : class;
        Task<AddResponse> SendNoEntityPost<T>(string url, object value) where T : class;
        Task<UpdateResponse> SendNoEntityPut<T>(string url, object value) where T : class;
        Task<UpdateResponse<T>> SendPut<T>(string url, object value) where T : class;
        Task<UpdateResponse> SendNoEntityPatch<T>(string url, object value) where T : class;
        Task<UpdateResponse<T>> SendPatch<T>(string url, object value) where T : class;
        Task<DeleteResponse> SendDelete<T>(string url) where T : class;
    }

    public class TozAwaBffHttpClient : HttpClientHelper, ITozAwaBffHttpClient
    {
        public TozAwaBffHttpClient(HttpClient client, AppSettings appSettings, AuthenticationStateProvider authProvider, PreferencesStoreClone storage, ILogger<TozAwaBffHttpClient> logger, ITranslationService translationService, IJSRuntime jSRuntime, AuthStateProvider authStateProvider, FirstloadState firstloadState) : base(client, translationService, appSettings, authProvider, storage, jSRuntime, authStateProvider, firstloadState, logger)
        {
#if !DEBUG
           ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
#endif
        }
    }
}

