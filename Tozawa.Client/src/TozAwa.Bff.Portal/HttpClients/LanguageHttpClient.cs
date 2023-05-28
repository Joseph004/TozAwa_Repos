using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.extension;
using Tozawa.Bff.Portal.Models;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Client.Portal.HttpClients
{
    public interface ILanguageHttpClient
    {
        Task<List<TranslationDto>> GetTranslations(Guid languageId, Guid? systemTypeId = null);
        Task<List<FullTranslationDto>> GetTranslationsFull(Guid languageId);
        Task<SystemTypeDto> GetCacheTimeStamp(Guid systemTypeId);
        Task<ImportTranslationResultDto> Import(ImportTranslationDto request);
        Task<List<ActiveLanguageDto>> GetActiveLanguages();
        Task<string> UpdateText(TranslationUpdateDto request);
        Task<string> UpdateTextBySystemType(TranslationUpdateDto request);
        Task<Guid> AddTranslation(TranslationAddDto request);
        Task<Guid> AddSystemType(Guid id, string description);
    }

    public class LanguageHttpClient : ILanguageHttpClient
    {
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<LanguageHttpClient> _logger;
        private readonly AppSettings _appSettings;

        public LanguageHttpClient(HttpClient client, ICurrentUserService currentUserService, AppSettings appSettings, ILogger<LanguageHttpClient> logger)
        {
            _client = client;
            _currentUserService = currentUserService;
            _logger = logger;
            _appSettings = appSettings;

            client.BaseAddress = new Uri(appSettings.LanguageSettings.ApiUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        protected async Task<HttpRequestMessage> PostRequest(string endpoint, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = CreateHttpContent(value)
            };

            return await Task.FromResult(request);
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

        protected async Task SetHeaders(HttpRequestMessage request)
        {
            var token = await GetToken();
            request.Headers.Authorization =
                   new AuthenticationHeaderValue("bearer", token);

            var currentUser = _currentUserService.User;
            request.Headers.Add("current-user", System.Text.Json.JsonSerializer.Serialize(currentUser));
            request.Headers.Add("toza-active-language", _currentUserService.LanguageId.ToString());
        }

        protected async Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            try
            {
                await SetHeaders(request);
                var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                return CreateHttpResonseMessage(request, response);
            }
            catch (Exception ex)
            {
                return CreateHttpResonseMessage(request, null);
            }
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

        public async Task<ImportTranslationResultDto> Import(ImportTranslationDto request)
        {
            var response = await Send(await PostRequest("import/", request));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<ImportTranslationResultDto>()
                : throw await HandleError(response);
        }

        public async Task<SystemTypeDto> GetCacheTimeStamp(Guid systemTypeId)
        {
            var response = await Send(await GetRequest($"systemtypes/{systemTypeId}"));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<SystemTypeDto>()
                : throw await HandleError(response);
        }

        public async Task<List<TranslationDto>> GetTranslations(Guid languageId, Guid? systemTypeId = null)
        {
            var response = await Send(await GetRequest($"translation/GetBySystemTypeLanguage/{systemTypeId ?? _appSettings.SystemTextGuid}/" + languageId));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<List<TranslationDto>>()
                : throw await HandleError(response);
        }

        public async Task<Guid> AddTranslation(TranslationAddDto request)
        {
            request.SystemTypeId = _appSettings.SystemTextGuid;
            var response = await Send(await PostRequest("translation/add/", request));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<Guid>()
                : throw await HandleError(response);
        }

        public async Task<List<FullTranslationDto>> GetTranslationsFull(Guid languageId)
        {
            var response = await Send(await GetRequest($"translation/GetBySystemTypeLanguage/{_appSettings.SystemTextGuid}/{languageId}"));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<List<FullTranslationDto>>()
                : throw await HandleError(response);
        }

        public async Task<string> UpdateText(TranslationUpdateDto request)
        {
            request.SystemTypeId = _appSettings.SystemTextGuid;
            var response = await Send(await PutRequest("translation/update/", request));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : throw await HandleError(response);
        }

        public async Task<string> UpdateTextBySystemType(TranslationUpdateDto request)
        {
            var response = await Send(await PutRequest("translation/update/", request));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : throw await HandleError(response);
        }

        public async Task<Guid> AddSystemType(Guid id, string description)
        {
            var response = await Send(await PostRequest("systemtypes", new AddSystemTypeRequest { Id = id, Description = description }));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<Guid>()
                : throw await HandleError(response);
        }

        public async Task<List<ActiveLanguageDto>> GetActiveLanguages()
        {
            var response = await Send(await GetRequest("activelanguages"));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<List<ActiveLanguageDto>>()
                : throw await HandleError(response);
        }

        protected async Task<HttpRequestMessage> PutRequest(string endpoint, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
            {
                Content = CreateHttpContent(value)
            };
            return await Task.FromResult(request);
        }

        protected async Task<HttpRequestMessage> GetRequest(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            return await Task.FromResult(request);
        }

        public async Task<Exception> HandleError(HttpResponseMessage response)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError($"{response.StatusCode} {response.RequestMessage.RequestUri.AbsoluteUri} {error}");
            throw new Exception(error);
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

