using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using TozawaNGO.Configurations;
using TozawaNGO.Extensions;
using TozawaNGO.Helpers;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.ResponseRequests;
using TozawaNGO.Services;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

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
        private readonly AuthStateProvider _authStateProvider;
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
            AuthStateProvider authStateProvider,
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
            _authStateProvider = authStateProvider;
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

            var user = await _authStateProvider.GetUserFromToken(result.Token);
            await _authStateProvider.SetCurrentUser(user);

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
        public async Task<UpdateResponse> SendNoEntityPatch<T>(string url, object value) where T : class
        {
            try
            {
                var putResponse = await Send(PatchRequest(url, value));

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

        public async Task<UpdateResponse<T>> SendPut<T>(string url, object value) where T : class
        {
            try
            {
                var putResponse = await Send(PutRequest(url, value));
                var putContent = putResponse.IsSuccessStatusCode ? await putResponse.Content.ReadAsJsonAsync<T>() : null;

                if (!putResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to update {name}. Status Code {StatusCode} Error message {ResponseContent}", nameof(T), putResponse.StatusCode, putResponse.Content);
                }

                return new UpdateResponse<T>(putResponse.IsSuccessStatusCode, await _translationService.GetHttpStatusText(putResponse.StatusCode), putResponse.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HttpClient exception occured");
                return new UpdateResponse<T>(false, "HttpClient exception occured", null, null);
            }
        }

        public async Task<UpdateResponse<T>> SendPatch<T>(string url, object value) where T : class
        {
            try
            {
                var putResponse = await Send(PatchRequest(url, value));
                var putContent = putResponse.IsSuccessStatusCode ? await putResponse.Content.ReadAsJsonAsync<T>() : null;

                if (!putResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to update {name}. Status Code {StatusCode} Error message {ResponseContent}", nameof(T), putResponse.StatusCode, putResponse.Content);
                }

                return new UpdateResponse<T>(putResponse.IsSuccessStatusCode, await _translationService.GetHttpStatusText(putResponse.StatusCode), putResponse.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HttpClient exception occured");
                return new UpdateResponse<T>(false, "HttpClient exception occured", null, null);
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
            /*  var authState = await _authProvider.GetAuthenticationStateAsync();
             var user = authState.User; */

            var token = await _localStorageService.GetItemAsync<string>("authToken");
            var refreshToken = await _localStorageService.GetItemAsync<string>("refreshToken");
            /* var token = user.FindFirst(c => c.Type.Equals("authToken"))?.Value;
            var refreshToken = user.FindFirst(c => c.Type.Equals("refreshToken"))?.Value; */

            var request = new RefreshTokenDto()
            {
                Token = token,
                RefreshToken = refreshToken
            };

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken) && !ValidateCurrentToken(token))
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
        private bool ValidateCurrentToken(string token)
        {
            var myIssuer = _appSettings.JWTSettings.ValidIssuer;
            var myAudience = _appSettings.JWTSettings.ValidAudience;

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWTSettings.SecurityKey))
                }, out SecurityToken validatedToken);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
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

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content) && content == "Oops, you are not authorized please contact support!!!")
                {
                    await Logout();
                }
            }

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
    }
}