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
    public interface ISystemTypeHttpClient
    {
        Task<IEnumerable<SystemTypeDto>> Get();
        Task<TableData<SystemTypeDto>> GetPaged(Dictionary<string, string> queryParameters);
        Task SetAsDefault(Guid id);
        Task CreateSystemType(string description);
        Task DeleteSystemType(Guid id);
    }

    public class SystemTypeHttpClient : HttpClientHelper, ISystemTypeHttpClient
    {
        private string BaseControllerUri { get; set; } = "systemTypes/";
        public SystemTypeHttpClient(
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

        public async Task CreateSystemType(string description)
        {
            var response = await Send(PostRequest(BaseControllerUri, new CreateSystemTypeCommand { Description = description }));
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task DeleteSystemType(Guid id)
        {
            var response = await Send(DeleteRequest($"{BaseControllerUri}{id}"));
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<IEnumerable<SystemTypeDto>> Get()
        {
            var request = GetRequest($"{BaseControllerUri}");
            var response = await Send(request);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<IEnumerable<SystemTypeDto>>()
                : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<TableData<SystemTypeDto>> GetPaged(Dictionary<string, string> queryParameters)
        {
            var request = GetRequest(QueryHelpers.AddQueryString($"{BaseControllerUri}paged", queryParameters));
            var response = await Send(request);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<TableData<SystemTypeDto>>()
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