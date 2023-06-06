using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.HttpClients
{
    public abstract partial class HttpClientBase : IHttpClientBase
    {
        internal readonly HttpClient _httpClient;
        private readonly ICurrentUserService _currentUserService;
        private readonly AppSettings _appSettings;
        protected readonly ILogger<HttpClientBase> _logger;
        private readonly IUserTokenService _userTokenService;

        public HttpClientBase(HttpClient httpClient,
        AppSettings appSettings,
            ICurrentUserService currentUserService,
            IUserTokenService userTokenService,
             ILogger<HttpClientBase> logger)
        {
            _httpClient = httpClient;
            _currentUserService = currentUserService;
            _appSettings = appSettings;
            _logger = logger;
            _userTokenService = userTokenService;
        }

        public async Task<T> Get<T>(string uri)
            => await Get<T>(uri, CancellationToken.None);

        public async Task<T> Get<T>(string uri, CancellationToken cancellationToken)
        {
            var getRequest = HttpClientHelperBase.GetRequest(uri);
            return await SendAndReceive<T>(getRequest, cancellationToken);
        }

        public async Task Put(string uri, object value)
            => await Put(uri, value, CancellationToken.None);

        public async Task<T> Put<T>(string uri, object value, CancellationToken cancellationToken)
        {
            var putRequest = HttpClientHelperBase.PutRequest(uri, value);
            return await SendAndReceive<T>(putRequest, cancellationToken);
        }

        public async Task Put(string uri, object value, CancellationToken cancellationToken)
        {
            var putRequest = HttpClientHelperBase.PutRequest(uri, value);
            await Send(putRequest, cancellationToken);
        }

        public async Task<T> Patch<T>(string uri, object value)
            => await Patch<T>(uri, value, CancellationToken.None);
        public async Task<T> Patch<T>(string uri, object value, CancellationToken cancellationToken)
        {
            var postRequest = HttpClientHelperBase.PatchRequest(uri, value);
            return await SendAndReceive<T>(postRequest, cancellationToken);
        }

        public async Task<T> Post<T>(string uri, object value, bool multiContent = false)
            => await Post<T>(uri, value, CancellationToken.None, multiContent);

        public async Task<T> Post<T>(string uri, object value, CancellationToken cancellationToken, bool multiContent = false)
        {
            var postRequest = HttpClientHelperBase.PostRequest(uri, value, multiContent);
            return await SendAndReceive<T>(postRequest, cancellationToken);
        }

        public async Task<T> Delete<T>(string uri, CancellationToken cancellationToken)
        {
            var deleteRequest = HttpClientHelperBase.DeleteRequest(uri);
            return await SendAndReceive<T>(deleteRequest, cancellationToken);
        }

        public async Task Delete(string uri, CancellationToken cancellationToken)
        {
            var deleteRequest = HttpClientHelperBase.DeleteRequest(uri);
            await Send(deleteRequest, cancellationToken);
        }

        private async Task<T> SendAndReceive<T>(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await Send(request, cancellationToken);
            var res = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync(cancellationToken));
            if (res == null)
            {
                throw new Exception("Send and recieve returned null");
            }
            return res;
        }

        private async Task<HttpResponseMessage> Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await SetupHeaders(request);
            var result = await _httpClient.SendAsync(request, cancellationToken);

            result.EnsureSuccessStatusCode();
            return result;
        }

        private async Task SetupHeaders(HttpRequestMessage request)
        {
            var token = await GetToken();
            request.Headers.Authorization =
                   new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

            var currentUser = _currentUserService.User;
            if (!string.IsNullOrEmpty(currentUser.AccessToken))
            {
                var userToken = new JwtSecurityTokenHandler().WriteToken(_userTokenService.GenerateTokenOptionsForAthService(currentUser.AccessToken));
                request.Headers.Add("tzuserauthentication", userToken);
            }
            request.Headers.Add("current-user", System.Text.Json.JsonSerializer.Serialize(currentUser));
            request.Headers.Add("toza-active-language", _currentUserService.LanguageId.ToString());
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
            AuthenticationResult result = null;
            try
            {
                IConfidentialClientApplication app;

                app = ConfidentialClientApplicationBuilder.Create(_appSettings.AADClient.ClientId)
                    .WithClientSecret(_appSettings.AADClient.ClientSecret)
                    .WithAuthority(new Uri(_appSettings.AADClient.Authority))
                    .Build();

                var resourceId = _httpClient.DefaultRequestHeaders.First(x => x.Key.Equals("tozawa-resourceId")).Value.ToList().First();

                var ResourceIds = new string[] { resourceId };

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