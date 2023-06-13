using Tozawa.Client.Portal.Configurations;
using Tozawa.Client.Portal.Services;
using System.Net;
using Tozawa.Client.Portal.Models.ResponseRequests;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Blazored.SessionStorage;

namespace Tozawa.Client.Portal.HttpClients
{
    public interface ITozAwaBffHttpClient
    {
        Task<GetResponse<T>> SendGet<T>(string url, Dictionary<string, string> queryParameters = null) where T : class;
        Task<AddResponse<T>> SendPost<T>(string url, object value) where T : class;
        Task<AddResponse> SendNoEntityPost<T>(string url, object value) where T : class;
        Task<UpdateResponse> SendNoEntityPut<T>(string url, object value) where T : class;
        Task<UpdateResponse> SendPut<T>(string url, object value) where T : class;
        Task<DeleteResponse> SendDelete<T>(string url) where T : class;
    }

    public class TozAwaBffHttpClient : HttpClientHelper, ITozAwaBffHttpClient
    {
        private readonly AppSettings _appSettings;
        public TozAwaBffHttpClient(HttpClient client, AppSettings appSettings, AuthenticationStateProvider authProvider, ILocalStorageService localStorage,
            NavigationManager navigationManager,
            ISessionStorageService sessionStorageService, ILogger<TozAwaBffHttpClient> logger, ITranslationService translationService) : base(client, translationService, appSettings, authProvider, localStorage, navigationManager, sessionStorageService, logger)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
    }
}

