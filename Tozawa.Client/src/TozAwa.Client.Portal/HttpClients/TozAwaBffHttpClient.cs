using System;
using System.Threading.Tasks;
using Tozawa.Client.Portal.Models.FormModels;
using System.Net.Http;
using Tozawa.Client.Portal.Configurations;
using Microsoft.Extensions.Logging;
using Tozawa.Client.Portal.Services;
using System.Net;
using System.Web;
using Tozawa.Client.Portal.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Tozawa.Client.Portal.Models.ResponseRequests;
using System.Collections.Generic;
using Blazored.LocalStorage;

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
        private readonly HttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;
        public TozAwaBffHttpClient(HttpClient client, AppSettings appSettings, ILocalStorageService localStorage, HttpContextAccessor httpContextAccessor, ILogger<TozAwaBffHttpClient> logger, ICurrentUserService currentUserService, ITranslationService translationService) : base(client, currentUserService, translationService, appSettings, localStorage, logger)
        {
            _appSettings = appSettings;
            if (client.BaseAddress == null)
            {
                client.BaseAddress = new Uri(appSettings.TozAwaBffApiSettings.ApiUrl);
            }
            _httpContextAccessor = httpContextAccessor;
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
    }
}

