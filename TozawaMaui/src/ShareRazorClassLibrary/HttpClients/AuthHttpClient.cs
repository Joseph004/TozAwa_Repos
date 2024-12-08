using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShareRazorClassLibrary.Configurations;
using ShareRazorClassLibrary.Extensions;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.ResponseRequests;
using ShareRazorClassLibrary.Services;

namespace ShareRazorClassLibrary.HttpClients
{
    public interface IAuthHttpClient
    {
        Task<GetResponse<T>> SendGet<T>(string url, Dictionary<string, string> queryParameters = null) where T : class;
        Task<AddResponse<T>> SendPost<T>(string url, object value) where T : class;
    }
    public class AuthHttpClient : IAuthHttpClient
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<AuthHttpClient> _logger;
        private readonly AuthenticationStateProvider _authProvider;
        private readonly NavigationManager _navigationManager;
        private readonly HttpClient _client;
        private readonly FirstloadState _firstloadState;
        private readonly AuthStateProvider _authStateProvider;
        private readonly ISessionStorageService _sessionStorageService;

        public AuthHttpClient(HttpClient client, AppSettings appSettings, ISessionStorageService sessionStorageService, AuthStateProvider authStateProvider, ILogger<AuthHttpClient> logger, AuthenticationStateProvider authProvider, NavigationManager navigationManager, FirstloadState firstloadState)
        {
            _client = client;
            _logger = logger;
            _appSettings = appSettings;
            _authProvider = authProvider;
            _firstloadState = firstloadState;
            _authStateProvider = authStateProvider;
            _sessionStorageService = sessionStorageService;

            client.BaseAddress = new Uri(appSettings.TozAwaNGOApiSettings.ApiUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            _navigationManager = navigationManager;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
        private async Task<string> TryRefreshToken()
        {
            var token = ((AuthStateProvider)_authStateProvider).UserLoginStateDto.JWTToken;
            var refreshToken = ((AuthStateProvider)_authStateProvider).UserLoginStateDto.JWTRefreshToken;

            var request = new RefreshTokenDto()
            {
                Token = token,
                RefreshToken = refreshToken
            };
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken) && !_authStateProvider.ValidateCurrentToken(token))
            {
                var response = await PostRefresh("token/refresh/", request);
                if (response.Success)
                {
                    var result = response.Entity ?? new LoginResponseDto();
                    return result.Token;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(request.Token))
                {
                    return request.Token;
                }
            }
            return string.Empty;
        }
        private async Task Logout()
        {
            var token = ((AuthStateProvider)_authStateProvider).UserLoginStateDto.JWTToken;
            var refreshToken = ((AuthStateProvider)_authStateProvider).UserLoginStateDto.JWTRefreshToken;
            var user = await ((AuthStateProvider)_authStateProvider).GetUserFromToken();
            await PostLogout($"token/logout/{user.Id.ToString()}", token, refreshToken);
            await ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        }
        private async Task PostLogout(string url, string token, string refreshToken)
        {
            var request = PostRequest(url, null);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("tzuserauthentication", token);
            }
            await _client.SendAsync(request);
        }
        public virtual async Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            var token = await TryRefreshToken();
            var context = await _authProvider.GetAuthenticationStateAsync();
            if (string.IsNullOrEmpty(token) && context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                await Logout();
                token = string.Empty;
            }
            request.Headers.Add("tzuserauthentication", token);
            var activeLanguage = await _sessionStorageService.GetItemAsync<ActiveLanguageDto>($"ActiveLanguageKey_activeLanguage");

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
        private async Task<AddResponse<LoginResponseDto>> PostRefresh(string url, RefreshTokenDto value)
        {
            var request = PostRequest(url, value);

            if (!string.IsNullOrEmpty(value.Token))
            {
                request.Headers.Add("tzuserauthentication", value.Token);
            }

            var postResponse = await _client.SendAsync(request);

            var postContent = postResponse.IsSuccessStatusCode ? new AddResponse<LoginResponseDto>(postResponse.IsSuccessStatusCode, StatusTexts.GetHttpStatusText(postResponse.StatusCode), postResponse.StatusCode, await postResponse.Content.ReadAsJsonAsync<LoginResponseDto>()) :
                     new AddResponse<LoginResponseDto>(postResponse.IsSuccessStatusCode, StatusTexts.GetHttpStatusText(postResponse.StatusCode), postResponse.StatusCode, null);

            if (!postResponse.IsSuccessStatusCode || !postContent.Success)
            {
                ((AuthStateProvider)_authStateProvider).UserLoginStateDto.Set(false, null, null);
                return postContent;
            }

            var result = postContent.Entity ?? new LoginResponseDto();

            ((AuthStateProvider)_authStateProvider).UserLoginStateDto.Set(true, result.Token, result.RefreshToken);
            await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token, result.RefreshToken);

            return postContent;
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
    }
    public class TokenResponse
    {
        public string Token_type { get; set; }
        public string Expires_in { get; set; }
        public string Ext_expires_in { get; set; }
        public string Access_token { get; set; }
    }
}

