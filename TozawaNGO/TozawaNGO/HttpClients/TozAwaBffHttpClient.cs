using TozawaNGO.Configurations;
using TozawaNGO.Services;
using System.Net;
using TozawaNGO.Models.ResponseRequests;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Blazored.SessionStorage;
using Microsoft.JSInterop;
using TozawaNGO.Auth.Models;

namespace TozawaNGO.HttpClients
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
        public TozAwaBffHttpClient(HttpClient client, AppSettings appSettings, AuthenticationStateProvider authProvider, ILocalStorageService localStorageService,
            NavigationManager navigationManager, IJSRuntime jSRuntime, ILogger<TozAwaBffHttpClient> logger, ITranslationService translationService) : base(client, translationService, appSettings, authProvider, localStorageService, navigationManager, jSRuntime, logger)
        {
            client.BaseAddress = new Uri(appSettings.TozAwaNGOApiSettings.ApiUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
    }
}

