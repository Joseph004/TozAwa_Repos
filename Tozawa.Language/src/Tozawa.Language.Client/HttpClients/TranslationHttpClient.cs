using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tozawa.Language.Client.AuthenticationServices;
using Tozawa.Language.Client.Configuration;
using Tozawa.Language.Client.Models.DTOs;
using Tozawa.Language.Client.Models.FormModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Tozawa.Language.Client.Extensions;
using Tozawa.Language.Client.Services;

namespace Tozawa.Language.Client.HttpClients
{
    public interface ITranslationHttpClient
    {
        Task<PagedDto<TranslatedTextDto>> Get(GetTranslationSummaryQuery request);
        Task<TableData<TranslatedTextDto>> GetPaged(GetTranslationSummaryQuery request);
        Task<Guid> AddTextTranslation(AddTextCommand request);
        Task<string> UpdateTextCommand(UpdateTextCommand request);
        Task Delete(Guid translationId);
    }

    public class TranslationHttpClient : HttpClientHelper, ITranslationHttpClient
    {
        private string BaseControllerUri { get; set; } = "translationSummary/";

        public TranslationHttpClient(
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

        public async Task<PagedDto<TranslatedTextDto>> Get(GetTranslationSummaryQuery request)
        {
            

            var response = await Send(PostRequest(BaseControllerUri, request));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<PagedDto<TranslatedTextDto>>()
                : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<TableData<TranslatedTextDto>> GetPaged(GetTranslationSummaryQuery query)
        {
            

            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            queryParameters.Add(nameof(query.SourceLanguageId), query.SourceLanguageId.ToString());
            queryParameters.Add(nameof(query.TargetLanguageId), query.TargetLanguageId.ToString());
            queryParameters.Add(nameof(query.SystemTypeId), query.SystemTypeId.ToString());
            if (!string.IsNullOrEmpty(query.FilterText))
            {
                queryParameters.Add(nameof(query.FilterText), query.FilterText);
            }
            if (query.XliffState.HasValue)
            {
                queryParameters.Add(nameof(query.XliffState), query.XliffState.Value.ToString());
            }
            queryParameters.Add(nameof(query.Page), query.Page.ToString());
            queryParameters.Add(nameof(query.PageSize), query.PageSize.ToString());

            var request = GetRequest(QueryHelpers.AddQueryString($"{BaseControllerUri}paged", queryParameters));
            var response = await Send(request);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<TableData<TranslatedTextDto>>()
                : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<string> UpdateTextCommand(UpdateTextCommand request)
        {
           

            var response = await Send(PutRequest("Translation/update", request));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<string>()
                : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<Guid> AddTextTranslation(AddTextCommand request)
        {
           

            var response = await Send(PostRequest("translation/add/", request));
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<Guid>()
                : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task Delete(Guid translationId)
        {
           

            var response = await Send(DeleteRequest("Translation/" + translationId));
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
    }
}