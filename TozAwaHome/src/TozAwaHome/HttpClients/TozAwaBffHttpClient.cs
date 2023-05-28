using System;
using System.Threading.Tasks;
using TozAwaHome.Models.FormModels;
using System.Net.Http;
using TozAwaHome.Configurations;
using Microsoft.Extensions.Logging;
using TozAwaHome.Services;
using System.Net;
using System.Web;
using TozAwaHome.Models.Dtos;
using Microsoft.AspNetCore.Http;
using TozAwaHome.Models.ResponseRequests;
using System.Collections.Generic;
using TozAwaHome.HttpClientService;

namespace TozAwaHome.HttpClients
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
		public TozAwaBffHttpClient(AppSettings appSettings, HttpContextAccessor httpContextAccessor, ILogger<TozAwaBffHttpClient> logger, ICurrentUserService currentUserService, ITranslationService translationService) : base(currentUserService, translationService, appSettings, logger)
        {
        }
    }
}

