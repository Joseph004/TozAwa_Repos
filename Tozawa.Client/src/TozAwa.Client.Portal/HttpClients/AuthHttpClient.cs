

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Tozawa.Client.Portal.Configurations;
using Tozawa.Client.Portal.Extensions;
using Tozawa.Client.Portal.HttpClients.Helpers;
using Tozawa.Client.Portal.Models.Dtos;
using Tozawa.Client.Portal.Models.ResponseRequests;

namespace Tozawa.Client.Portal.HttpClients
{
    public interface IAuthHttpClient
    {
        Task<GetResponse<T>> SendGet<T>(string url, Dictionary<string, string> queryParameters = null) where T : class;
        Task<AddResponse<T>> SendPost<T>(string url, object value) where T : class;
    }
    public class AuthHttpClient : IAuthHttpClient
    {
        private readonly AppSettings _appSettings;
        private readonly HttpClient _client;
        private readonly ILogger<AuthHttpClient> _logger;
        private readonly ILocalStorageService _localStorageService;
        public AuthHttpClient(HttpClient client, AppSettings appSettings, ILocalStorageService localStorageService, ILogger<AuthHttpClient> logger, AuthenticationStateProvider authState)
        {
            client.BaseAddress = new Uri(appSettings.TozAwaBffApiSettings.ApiUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            _client = client;
            _logger = logger;
            _appSettings = appSettings;
            _localStorageService = localStorageService;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        public virtual async Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            var token = await GetToken();
            request.Headers.Authorization =
                   new AuthenticationHeaderValue("bearer", token);

            var activeLanguage = await _localStorageService.GetItemAsync<ActiveLanguageDto>($"ActiveLanguageKey_activeLanguage");

            if (activeLanguage != null && activeLanguage.Id != Guid.Empty)
            {
                request.Headers.Add("toza-active-language", activeLanguage.Id.ToString());
            }

            var result = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Server error");
            }
            return result;
        }
        public HttpRequestMessage GetRequest(string endpoint)
           => new(HttpMethod.Get, endpoint);
        public HttpRequestMessage PostRequest(string endpoint, object value)
        => new(HttpMethod.Post, endpoint) { Content = CreateHttpContent(value) };
        public static HttpContent CreateHttpContent(object content, string mediaTypeHeader = "application/json")
        {
            if (content == null) return null;
            var ms = new MemoryStream();
            SerializeJsonIntoStream(content, ms);
            ms.Seek(0, SeekOrigin.Begin);
            HttpContent httpContent = new StreamContent(ms);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(mediaTypeHeader);
            return httpContent;
        }

        private static void SerializeJsonIntoStream(object content, MemoryStream ms)
        {
            using var sw = new StreamWriter(ms, new UTF8Encoding(false), 1024, true);
            using var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None };
            var js = new JsonSerializer();
            js.Serialize(jtw, content);
            jtw.Flush();
        }

        public async Task<GetResponse<T>> SendGet<T>(string url, Dictionary<string, string> queryParameters = null) where T : class
        {
            try
            {
                var getResponse = queryParameters != null ? await Send(GetRequest(QueryHelpers.AddQueryString(url, queryParameters))) : await Send(GetRequest(url));

                var getContent = new GetResponse<T>(getResponse.IsSuccessStatusCode, StatusTexts.GetHttpStatusText(getResponse.StatusCode), getResponse.StatusCode, getResponse.IsSuccessStatusCode ? await getResponse.Content.ReadAsJsonAsync<T>() : null);

                if (!getResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to get {name}. Status Code {StatusCode} Error message {ResponseContent}", nameof(T), getResponse.StatusCode, getResponse.Content);
                }

                return getContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HttpClient exception occured");
                return new GetResponse<T>(false, "", null, null);
            }

        }

        public async Task<AddResponse<T>> SendPost<T>(string url, object value) where T : class
        {
            try
            {
                var postResponse = await Send(PostRequest(url, value));

                var postContent = postResponse.IsSuccessStatusCode ? await postResponse.Content.ReadAsJsonAsync<AddResponse<T>>() :
                     new AddResponse<T>(postResponse.IsSuccessStatusCode, StatusTexts.GetHttpStatusText(postResponse.StatusCode), postResponse.StatusCode, null);

                if (!postResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to add {name}. Status Code {StatusCode} Error message {ResponseContent}", nameof(T), postResponse.StatusCode, postResponse.Content);
                }

                return postContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HttpClient exception occured");
                return new AddResponse<T>(false, "HttpClient exception occured", null, null);
            }
        }

        public async Task<string> GetToken()
        {
            var result = await RunAsync();

            if (string.IsNullOrEmpty(result.AccessToken))
            {
                throw new ArgumentNullException(nameof(result.AccessToken));
            }
            return result.AccessToken;
        }
        public async Task<AuthenticationResult> RunAsync()
        {
            IConfidentialClientApplication app;

            app = ConfidentialClientApplicationBuilder.Create(_appSettings.AADClient.ClientId)
                .WithClientSecret(_appSettings.AADClient.ClientSecret)
                .WithAuthority(new Uri(_appSettings.AADClient.Authority))
                .Build();

            string[] ResourceIds = new string[] { _appSettings.TozAwaBffApiSettings.ResourceId };

            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(ResourceIds).ExecuteAsync();
            }
            catch (MsalClientException ex)
            {
                _logger.LogError(ex, "Fail to get token");
            }
            return result;
        }
    }

}

