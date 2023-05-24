using System.Net;
using Tozawa.Language.Client.Configuration;
using Tozawa.Language.Client.Models.DTOs;
using Tozawa.Language.Client.Models.FormModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using Tozawa.Language.Client.Extensions;
using Tozawa.Language.Client.Services;

namespace Tozawa.Language.Client.HttpClients
{
    public interface IXliffHttpClient
    {
        Task<string> UploadFile(MultipartFormDataContent file);
        Task<TableData<XliffDistributionFile>> GetAllXliffDistributionFilesLogsPaged(Dictionary<string, string> queryParameters);
        Task<Byte[]> DownloadExportedFile(string fileName);
        Task<Byte[]> DownloadImportedFile(string fileName);
        Task<Byte[]> CreateAndGetExportFile(GetXliffFileQuery query, string fileName);
    }

    public class XliffHttpClient : HttpClientHelper, IXliffHttpClient
    {
        private string BaseControllerUri { get; set; } = "translationSummary/";
        public XliffHttpClient(
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

        public async Task<string> UploadFile(MultipartFormDataContent file)
        {
            var url = "xliffimport/";
            var response = await PostFile(url, file);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<string>()
                : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<TableData<XliffDistributionFile>> GetAllXliffDistributionFilesLogsPaged(Dictionary<string, string> queryParameters)
        {
            var request = GetRequest(QueryHelpers.AddQueryString($"Xliffdistributionfileslogs/paged", queryParameters));
            var response = await Send(request);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsJsonAsync<TableData<XliffDistributionFile>>()
                : throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<Byte[]> DownloadExportedFile(string fileName)
        {
            var url = $"Xliffexport/{fileName}";
            var request = GetRequest(url);
            var response = await Send(request);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsByteArrayAsync()
                : throw new Exception("Error downloading exported xliff-file.");
        }

        public async Task<Byte[]> DownloadImportedFile(string fileName)
        {
            var url = $"xliffimport/{fileName}";
            var request = GetRequest(url);
            var response = await Send(request);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsByteArrayAsync()
                : throw new Exception("Error downloading imported xliff-file.");
        }

        public async Task<Byte[]> CreateAndGetExportFile(GetXliffFileQuery query, string fileName)
        {
            var url = $"Xliffexport/{query.OriginalLanguageId}/{query.TargetLanguageId}/{query.SystemTypeId}/{fileName}";
            var request = GetRequest(url);
            var response = await Send(request);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsByteArrayAsync()
                : throw new Exception("Error downloading exported xliff-file.");
        }
    }
}