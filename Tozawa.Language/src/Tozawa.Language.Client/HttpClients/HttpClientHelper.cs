using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Tozawa.Language.Client.Configuration;
using Tozawa.Language.Client.Extensions;
using Tozawa.Language.Client.Helpers;
using Tozawa.Language.Client.Models.ResponseRequests;
using Tozawa.Language.Client.Services;

namespace Tozawa.Language.Client.HttpClients
{
    public class HttpClientHelper
    {
        protected readonly ILogger<HttpClientHelper> _logger;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;
        private readonly AppSettings _appSettings;

        public HttpClientHelper(
            HttpClient client,
            ICurrentUserService currentUserService,
            AppSettings appSettings,
            ILogger<HttpClientHelper> logger)
        {
            _logger = logger;
            _client = client;
            _appSettings = appSettings;
            _currentUserService = currentUserService;
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

        public async Task<AddResponse> SendNoEntityPost<T>(string url, object value) where T : class
        {
            try
            {
                var postResponse = await Send(PostRequest(url, value));
                if (!postResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to add {name}. Status Code {StatusCode} Error message {ResponseContent}", nameof(T), postResponse.StatusCode, postResponse.Content);
                }

                return new AddResponse(postResponse.IsSuccessStatusCode, StatusTexts.GetHttpStatusText(postResponse.StatusCode), postResponse.StatusCode);
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

                return new UpdateResponse(putResponse.IsSuccessStatusCode, StatusTexts.GetHttpStatusText(putResponse.StatusCode), putResponse.StatusCode);
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

                return new DeleteResponse(deleteResponse.IsSuccessStatusCode, StatusTexts.GetHttpStatusText(deleteResponse.StatusCode), deleteResponse.StatusCode);
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

        protected async Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            var token = await GetToken();
            request.Headers.Authorization =
                   new AuthenticationHeaderValue("tzappauthentication", token);

            var currentUser = await _currentUserService.GetCurrentUser();
            request.Headers.Add("current-user", System.Text.Json.JsonSerializer.Serialize(currentUser));
            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            return response;
        }

        protected async Task<HttpResponseMessage> PostFile(string url, HttpContent request)
        {
            var token = await GetToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("tzappauthentication", token);
            var response = await _client.PostAsync(url, request).ConfigureAwait(false);

            return response;
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

            var ResourceIds = new string[] { _appSettings.LanguageSettings.ResourceId };

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