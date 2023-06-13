using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tozawa.Client.Portal.Configurations;
using Tozawa.Client.Portal.Extensions;
using Tozawa.Client.Portal.HttpClients.Helpers;
using Tozawa.Client.Portal.Models.Dtos;
using Tozawa.Client.Portal.Models.ResponseRequests;
using Tozawa.Client.Portal.Services;
using TozAwa.Client.Portal;

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
        private readonly ILogger<AuthHttpClient> _logger;
        private readonly AuthenticationStateProvider _authProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly ISessionStorageService _sessionStorageService;
        private readonly HttpClient _client;

        public BrowserRequestCache DefaultBrowserRequestCache { get; set; }
        public BrowserRequestCredentials DefaultBrowserRequestCredentials { get; set; }
        public BrowserRequestMode DefaultBrowserRequestMode { get; set; }
        public AuthHttpClient(HttpClient client, AppSettings appSettings, ILogger<AuthHttpClient> logger, AuthenticationStateProvider authProvider, ILocalStorageService localStorage, NavigationManager navigationManager,
           ISessionStorageService sessionStorageService, AuthenticationStateProvider authState)
        {
            _client = client;
            _logger = logger;
            _appSettings = appSettings;
            _localStorage = localStorage;
            _authProvider = authProvider;
            _navigationManager = navigationManager;
            _sessionStorageService = sessionStorageService;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
        private async Task<string> TryRefreshToken()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
            if (string.IsNullOrEmpty(exp)) return string.Empty;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;

            var token = await _localStorage.GetItemAsync<string>("authToken");
            var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

            var request = new RefreshTokenDto()
            {
                Token = token,
                RefreshToken = refreshToken
            };

            if (diff.TotalMinutes <= 2)
            {
                var response = await PostRefresh("token/refresh", request);
                if (response.Success)
                {
                    var result = response.Entity ?? new LoginResponseDto();
                    return result.Token;
                }
            }
            return string.Empty;
        }
        private async Task Logout()
        {
            await RemoveCurrentUser();
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("refreshToken");
            ((AuthStateProvider)_authProvider).NotifyUserLogout();

            NavigateToReturnPage();
        }
        public async Task RemoveCurrentUser()
        {
            if (await _sessionStorageService.ContainKeyAsync("currentUser"))
            {
                await _sessionStorageService.RemoveItemAsync("currentUser");
            }
        }
        private void NavigateToReturnPage()
        {
            var currentPath = _navigationManager.Uri.Split(_navigationManager.BaseUri)[1];

            if (string.IsNullOrEmpty(currentPath))
            {
                _navigationManager.NavigateTo("/home");
            }
            else
            {
                _navigationManager.NavigateTo($"/{currentPath}");
            }
        }
        public virtual async Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            var token = await TryRefreshToken();
            var context = await _authProvider.GetAuthenticationStateAsync();
            if (string.IsNullOrEmpty(token) && context.User.Identity.IsAuthenticated)
            {
               await Logout();
                token = string.Empty;
            }
            request.Headers.Add("tzuserauthentication", token);
            var activeLanguage = await _localStorage.GetItemAsync<ActiveLanguageDto>($"ActiveLanguageKey_activeLanguage");

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

            var activeLanguage = await _localStorage.GetItemAsync<ActiveLanguageDto>($"ActiveLanguageKey_activeLanguage");

            if (activeLanguage != null && activeLanguage.Id != Guid.Empty)
            {
                request.Headers.Add("toza-active-language", activeLanguage.Id.ToString());
            }

            var postResponse = await _client.SendAsync(request);

            var postContent = postResponse.IsSuccessStatusCode ? await postResponse.Content.ReadAsJsonAsync<AddResponse<LoginResponseDto>>() :
                     new AddResponse<LoginResponseDto>(postResponse.IsSuccessStatusCode, StatusTexts.GetHttpStatusText(postResponse.StatusCode), postResponse.StatusCode, null);

            if (!postResponse.IsSuccessStatusCode || !postContent.Success)
                return postContent;

            var result = postContent.Entity ?? new LoginResponseDto();

            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

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

        /* private async Task<string> GetToken()
        {
            var result = await RunAsync();

            if (string.IsNullOrEmpty(result.Access_token))
            {
                throw new ArgumentNullException(nameof(result.Access_token));
            }
            return result.Access_token;
        }
        private async Task<TokenResponse> RunAsync()
        {
            var tokenUrl = $"/oauth2/v2.0/token";

            using var client = _httpClientFactory.CreateClient("TzDefault");
    
            var postObject = new
            {
                client_id = _appSettings.AADClient.ClientId,
                scope = _appSettings.TozAwaBffApiSettings.ResourceId,
                client_secret = _appSettings.AADClient.ClientSecret,
                grant_type = "client_credentials"
            };
           
            var formContent = new FormUrlEncodedContent(new[]
           {
              new KeyValuePair<string, string>("client_id", _appSettings.AADClient.ClientId),
              new KeyValuePair<string, string>("scope", _appSettings.TozAwaBffApiSettings.ResourceId),
              new KeyValuePair<string, string>("client_secret", _appSettings.AADClient.ClientSecret),
              new KeyValuePair<string, string>("grant_type", "client_credentials"),
            });
            TokenResponse result = null;
            try
            {
                var req = PostRequest(tokenUrl, postObject);
                var response = await client.SendAsync(req, CancellationToken.None).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsJsonAsync<TokenResponse>();
                }
                else
                {
                    var message = response.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail to get token");
            }
            return result;
        }*/
    }
    public class TokenResponse
    {
        public string Token_type { get; set; }
        public string Expires_in { get; set; }
        public string Ext_expires_in { get; set; }
        public string Access_token { get; set; }
    }
}

