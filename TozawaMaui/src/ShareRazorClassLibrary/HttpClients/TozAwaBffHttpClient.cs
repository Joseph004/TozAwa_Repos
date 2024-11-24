using ShareRazorClassLibrary.Configurations;
using ShareRazorClassLibrary.Services;
using System.Net;
using ShareRazorClassLibrary.Models.ResponseRequests;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShareRazorClassLibrary.Helpers;
using Microsoft.Extensions.Logging;

namespace ShareRazorClassLibrary.HttpClients
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
        public TozAwaBffHttpClient(HttpClient client, AppSettings appSettings, AuthenticationStateProvider authProvider,
            NavigationManager navigationManager, ILogger<TozAwaBffHttpClient> logger, ITranslationService translationService, IJSRuntime jSRuntime, AuthStateProvider authStateProvider) : base(client, translationService, appSettings, authProvider, navigationManager, jSRuntime, authStateProvider, logger)
        {
            client.BaseAddress = new Uri(appSettings.TozAwaNGOApiSettings.ApiUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
    }
}

