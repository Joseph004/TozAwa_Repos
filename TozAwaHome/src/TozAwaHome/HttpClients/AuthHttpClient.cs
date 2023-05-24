

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using TozAwaHome.Configurations;
using TozAwaHome.Extensions;
using TozAwaHome.HttpClients.Helpers;
using TozAwaHome.HttpClientService;
using TozAwaHome.Models.Dtos;
using TozAwaHome.Models.ResponseRequests;

namespace TozAwaHome.HttpClients
{
    public interface IAuthHttpClient
    {
        Task<GetResponse<T>> SendGet<T>(string url, Dictionary<string, string> queryParameters = null) where T : class;
        Task<AddResponse<T>> SendPost<T>(string url, object value) where T : class;
    }
    public class AuthHttpClient : IAuthHttpClient
    {
        private readonly AppSettings _appSettings;
        //private readonly HttpClient _client;
        private readonly ILogger<AuthHttpClient> _logger;

		public AuthHttpClient(AppSettings appSettings, ILogger<AuthHttpClient> logger)
        {
			_logger = logger;
			_appSettings = appSettings;

			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
		}

        public virtual async Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            HttpResponseMessage result = null;
			try
			{
				var token = await GetToken();
            request.Headers.Authorization =
                   new AuthenticationHeaderValue("bearer", token);

            var activeLanguage = new ActiveLanguageDto();

			var activeLanguageStr = await SecureStorage.GetAsync($"ActiveLanguageKey_activeLanguage");
            if(!string.IsNullOrEmpty(activeLanguageStr))
            {
              activeLanguage = JsonConvert.DeserializeObject<ActiveLanguageDto>(activeLanguageStr);
            }

            if (activeLanguage != null && activeLanguage.Id != Guid.Empty)
            {
                request.Headers.Add("toza-active-language", activeLanguage.Id.ToString());
            }
				HttpClient client = new HttpClient();

#if DEBUG
				if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
				{
					HttpsClientHandlerService handler = new HttpsClientHandlerService();
					client = new HttpClient(handler.GetPlatformMessageHandler());
                    //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				}
				else
				{
					//HttpClientHandler clientHandler = new HttpClientHandler();
					//clientHandler.SslProtocols = System.Security.Authentication.SslProtocols.Tls13 | System.Security.Authentication.SslProtocols.Tls12;
					//ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
					//client = new HttpClient(clientHandler);
				}
#else
       client.DefaultRequestHeaders.Add("Accept", "application/json");
       ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
#endif

				client.BaseAddress = new Uri(_appSettings.TozAwaBffApiSettings.ApiUrl);
				client.DefaultRequestHeaders.Add("Accept", "application/json");

                using var httpClient = client;
                
                result = await client.SendAsync(request);

				if (!result.IsSuccessStatusCode)
				{
					throw new Exception("Server error");
				}
				return result;
              
			}
			catch (HttpRequestException e)
			{
				throw new Exception("Server error");
			} 
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
            catch (MsalClientException ex)
            {
                _logger.LogError(ex, "Fail to get token");
            }
            return result;
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

