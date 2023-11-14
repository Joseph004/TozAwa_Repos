using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using TozawaNGO.Configurations;
using TozawaNGO.Extensions;
using TozawaNGO.Helpers;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.ResponseRequests;
using TozawaNGO.Services;
using TozawaNGO;
using Microsoft.JSInterop;

namespace TozawaNGO.HttpClients
{
    public class HttpClientHelper
    {
        protected readonly ILogger<HttpClientHelper> _logger;
        private readonly ITranslationService _translationService;
        private readonly AppSettings _appSettings;
        private readonly AuthenticationStateProvider _authProvider;
        private readonly ILocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;
        private IJSRuntime _jSRuntime { get; set; }
        private readonly HttpClient _client;

        public HttpClientHelper(
           HttpClient client,
            ITranslationService translationService,
            AppSettings appSettings,
            AuthenticationStateProvider authProvider,
            ILocalStorageService localStorageService,
            NavigationManager navigationManager,
            IJSRuntime jSRuntime,
            ILogger<HttpClientHelper> logger)
        {
            _logger = logger;
            _client = client;
            _appSettings = appSettings;
            _authProvider = authProvider;
            _localStorageService = localStorageService;
            _translationService = translationService;
            _navigationManager = navigationManager;
            _jSRuntime = jSRuntime;
        }
        public async Task RemoveCurrentUser()
        {
            if (await _localStorageService.ContainKeyAsync("currentUser"))
            {
                await _localStorageService.RemoveItemAsync("currentUser");
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
                return postContent;

            var result = postContent.Entity ?? new LoginResponseDto();

            await _localStorageService.SetItemAsync("authToken", result.Token);
            await _localStorageService.SetItemAsync("refreshToken", result.RefreshToken);

            return postContent;
        }
        public static HttpResponseMessage CreateHttpResonseMessage(HttpRequestMessage request, HttpResponseMessage response)
        {
            var result = response ?? new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = CreateHttpContent("Unable to connect. Please try again or contact support."),
                RequestMessage = request
            };

            if (response == null || response.IsSuccessStatusCode)
                return result;

            foreach (var (key, value) in request.Headers)
            {
                result.Headers.Add(key, value);
            }

            return result;
        }
        public async Task<GetResponse<T>> SendGet<T>(string url, Dictionary<string, string> queryParameters = null) where T : class
        {
            try
            {
                var getResponse = queryParameters != null ? await Send(GetRequest(QueryHelpers.AddQueryString(url, queryParameters))) : await Send(GetRequest(url));

                var getContent = new GetResponse<T>(getResponse.IsSuccessStatusCode, await _translationService.GetHttpStatusText(getResponse.StatusCode), getResponse.StatusCode, getResponse.IsSuccessStatusCode ? await getResponse.Content.ReadAsJsonAsync<T>() : null);

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
                     new AddResponse<T>(postResponse.IsSuccessStatusCode, await _translationService.GetHttpStatusText(postResponse.StatusCode), postResponse.StatusCode, null);

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
        public async Task<UpdateResponse> SendNoEntityPut<T>(string url, object value) where T : class
        {
            try
            {
                var putResponse = await Send(PutRequest(url, value));

                if (!putResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to update {name}. Status Code {StatusCode} Error message {ResponseContent}", nameof(T), putResponse.StatusCode, putResponse.Content);
                }

                return new UpdateResponse(putResponse.IsSuccessStatusCode, await _translationService.GetHttpStatusText(putResponse.StatusCode), putResponse.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HttpClient exception occured");
                return new UpdateResponse(false, "", null);
            }
        }
        public async Task<AddResponse> SendNoEntityPost<T>(string url, object value) where T : class
        {
            try
            {
                var postResponse = await Send(PostRequest(url, value));
                if (!postResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to add {name}. Status Code {StatusCode} Error message {ResponseContent}", nameof(T), postResponse.StatusCode, postResponse.Content);
                }

                return new AddResponse(postResponse.IsSuccessStatusCode, await _translationService.GetHttpStatusText(postResponse.StatusCode), postResponse.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HttpClient exception occured");
                return new AddResponse(false, "", null);
            }
        }

        public async Task<UpdateResponse> SendPut<T>(string url, object value) where T : class
        {
            try
            {
                var putResponse = await Send(PutRequest(url, value));
                var putContent = putResponse.IsSuccessStatusCode ? await putResponse.Content.ReadAsJsonAsync<T>() : null;

                if (!putResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to update {name}. Status Code {StatusCode} Error message {ResponseContent}", nameof(T), putResponse.StatusCode, putResponse.Content);
                }

                return new UpdateResponse(putResponse.IsSuccessStatusCode, await _translationService.GetHttpStatusText(putResponse.StatusCode), putResponse.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HttpClient exception occured");
                return new UpdateResponse(false, "", null);
            }
        }

        public async Task<DeleteResponse> SendDelete<T>(string url) where T : class
        {
            try
            {
                var deleteResponse = await Send(DeleteRequest(url));

                if (!deleteResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to delete {name}. Status Code {StatusCode} Error message {ResponseContent}", nameof(T), deleteResponse.StatusCode, deleteResponse.Content);
                }

                return new DeleteResponse(deleteResponse.IsSuccessStatusCode, await _translationService.GetHttpStatusText(deleteResponse.StatusCode), deleteResponse.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HttpClient exception occured");
                return new DeleteResponse(false, "", null);
            }
        }

        protected static HttpContent CreateMultiPartContent(IFormFile file)
        {
            if (file == null) return null;
            return new StreamContent(file.OpenReadStream())
            {
                Headers =
                {
                    ContentLength = file.Length,
                    ContentType = new MediaTypeHeaderValue(file.ContentType)
                }
            };
        }

        protected static HttpContent CreateHttpContent(object content, string mediaTypeHeader = "application/json")
        {
            if (content == null) return null;
            var ms = new MemoryStream();
            SerializeJsonIntoStream(content, ms);
            ms.Seek(0, SeekOrigin.Begin);
            HttpContent httpContent = new StreamContent(ms);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(mediaTypeHeader);
            return httpContent;
        }

        protected static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true);
            using var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None };
            var js = new JsonSerializer();
            js.Serialize(jtw, value);
            jtw.Flush();
        }

        protected HttpRequestMessage PatchRequest(string endpoint, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Content = CreateHttpContent(value)
            };
            return request;
        }

        protected HttpRequestMessage PutRequest(string endpoint, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
            {
                Content = CreateHttpContent(value)
            };
            return request;
        }

        protected HttpRequestMessage PostRequest(string endpoint, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = CreateHttpContent(value)
            };
            return request;
        }

        protected HttpRequestMessage GetRequest(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            return request;
        }

        protected HttpRequestMessage DeleteRequest(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            return request;
        }
        private async Task<string> TryRefreshToken()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
            if (string.IsNullOrEmpty(exp)) return string.Empty;
            var expTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(Convert.ToInt64(exp)));
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;

            var token = user.FindFirst(c => c.Type.Equals("authToken"))?.Value;
            var refreshToken = user.FindFirst(c => c.Type.Equals("refreshToken"))?.Value;

            var request = new RefreshTokenDto()
            {
                Token = token,
                RefreshToken = refreshToken
            };

            if (diff.TotalMinutes <= 2)
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
        protected async Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            var activeLanguage = await _translationService.GetActiveLanguage();

            var token = await TryRefreshToken();
            var context = await _authProvider.GetAuthenticationStateAsync();
            if (string.IsNullOrEmpty(token) && context.User.Identity.IsAuthenticated)
            {
                await Logout();
                token = string.Empty;
            }

            request.Headers.Add("tzuserauthentication", token);

            request.Headers.Add("toza-active-language", activeLanguage.Id.ToString());
            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            return response;
        }
        private async Task Logout()
        {
            await RemoveCurrentUser();

            var logoutUrl = $"logout{NavigateToReturnPage()}";
            await _jSRuntime.InvokeVoidAsync("open", logoutUrl, "_top");

            var returnPage = NavigateToReturnPage();
            _navigationManager.NavigateTo(returnPage);
        }
        private string NavigateToReturnPage()
        {
            var currentPath = _navigationManager.Uri.Split(_navigationManager.BaseUri)[1];

            if (string.IsNullOrEmpty(currentPath))
            {
                return "/home";
            }
            else
            {
                return $"/{currentPath}";
            }
        }
        protected async Task<HttpResponseMessage> PostFile(string url, HttpContent request)
        {
            var token = await TryRefreshToken();
            var context = await _authProvider.GetAuthenticationStateAsync();
            if (string.IsNullOrEmpty(token) && context.User.Identity.IsAuthenticated)
            {
                await Logout();
                token = string.Empty;
            }
            request.Headers.Add("tzuserauthentication", token);
            var response = await _client.PostAsync(url, request).ConfigureAwait(false);

            return response;
        }
        /*  public async Task<string> GetToken()
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

             var ResourceIds = new string[] { _appSettings.TozAwaBffApiSettings.ResourceId };

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
         } */

        /*  public async Task<string> GetToken()
         {
             var result = await RunAsync();

             if (string.IsNullOrEmpty(result.Access_token))
             {
                 throw new ArgumentNullException(nameof(result.Access_token));
             }
             return result.Access_token;
         }
         public async Task<TokenResponse> RunAsync()
         {
             var tokenUrl = $"{_appSettings.AADClient.Authority}/oauth2/v2.0/token";

             using var client = new HttpClient();

             client.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");

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
                 var req = new HttpRequestMessage(HttpMethod.Post, tokenUrl) { Content = formContent };
                 var response = await client.SendAsync(req);
                 result = await response.Content.ReadAsJsonAsync<TokenResponse>();
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "Fail to get token");
             }
             return result;
         }

         public class TokenResponse
         {
             public string Token_type { get; set; }
             public string Expires_in { get; set; }
             public string Ext_expires_in { get; set; }
             public string Access_token { get; set; }
         } */
    }
}