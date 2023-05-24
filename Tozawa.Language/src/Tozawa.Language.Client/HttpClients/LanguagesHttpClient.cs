using System.Net;
using Tozawa.Language.Client.Configuration;
using Tozawa.Language.Client.Models.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using Tozawa.Language.Client.Extensions;
using Tozawa.Language.Client.Services;

namespace Tozawa.Language.Client.HttpClients
{
    public interface ILanguagesHttpClient
    {
        Task<IEnumerable<LanguageDto>> Get(bool includeDeleted = false);
        Task<TableData<LanguageDto>> GetPaged(Dictionary<string, string> queryParameters);
        Task<LanguageDto> AddLanguage(LanguageDto language);
        Task<LanguageDto> DeleteLanguage(Guid languageId);
        Task<LanguageDto> UpdateLanguage(LanguageDto language);
        Task SetAsDefault(Guid id);
    }

    public class LanguagesHttpClient : HttpClientHelper, ILanguagesHttpClient
    {
        private string BaseControllerUri { get; set; } = "activelanguages/";
        public LanguagesHttpClient(
            HttpClient client,
            ICurrentUserService currentUserService,
            AppSettings appSettings,
            ILogger<LanguagesHttpClient> logger,
            AuthenticationStateProvider authState) : base(client, currentUserService, appSettings, logger)
        {
            client.BaseAddress = new Uri(appSettings.LanguageSettings.ApiUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        public async Task<IEnumerable<LanguageDto>> Get(bool includeDeleted)
        {
            var url = BaseControllerUri + (includeDeleted ? "includeDeleted/" : "");

            var request = GetRequest(url);

            var response = await Send(request);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<IEnumerable<LanguageDto>>()
                : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<TableData<LanguageDto>> GetPaged(Dictionary<string, string> queryParameters)
        {
            var request = GetRequest(QueryHelpers.AddQueryString($"{BaseControllerUri}paged", queryParameters));
            var response = await Send(request);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<TableData<LanguageDto>>()
                : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<LanguageDto> AddLanguage(LanguageDto language)
        {
            var response = await Send(PostRequest(BaseControllerUri, language));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<LanguageDto>()
                : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<LanguageDto> UpdateLanguage(LanguageDto language)
        {
            var response = await Send(PutRequest(BaseControllerUri, language));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<LanguageDto>()
                : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<LanguageDto> DeleteLanguage(Guid languageId)
        {
            var response = await Send(DeleteRequest($"{BaseControllerUri}{languageId}"));
            return response.IsSuccessStatusCode
                 ? await response.Content.ReadAsJsonAsync<LanguageDto>()
                 : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task SetAsDefault(Guid id)
        {
            var response = await Send(PutRequest($"{BaseControllerUri}default", new { Id = id }));
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
    }
}