using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Microsoft.JSInterop;
using System;
using Tozawa.Language.Client.HttpClients;
using System.Linq;
using Tozawa.Language.Client.Models.DTOs;
using Tozawa.Language.Client.Models.FormModels;
using System.Threading.Tasks;

namespace Tozawa.Language.Client.Pages.Components
{
    public partial class XliffExportView : ComponentBase
    {
        [Inject] protected ISystemTypeHttpClient SystemTypeClient { get; set; }
        [Inject] protected ILanguagesHttpClient LanguageClient { get; set; }
        [Inject] protected IXliffHttpClient XliffHttpClient { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }

        public List<LanguageDto> Languages { get; set; } = new List<LanguageDto>();
        public List<SystemTypeDto> SystemTypes { get; set; } = new List<SystemTypeDto>();

        private LanguageDto sourceLanguage;
        private LanguageDto targetLanguage;
        private SystemTypeDto selectedSystemType;

        protected override async Task OnInitializedAsync()
        {
            Languages.AddRange(await LanguageClient.Get(false));
            SystemTypes.AddRange(await SystemTypeClient.Get());

            sourceLanguage = Languages.FirstOrDefault(x => x.IsDefault);
            targetLanguage = Languages.FirstOrDefault();
            selectedSystemType = SystemTypes.FirstOrDefault(x => x.IsDefault);

            StateHasChanged();
        }

        private async void Export()
        {
            var query = new GetXliffFileQuery
            {
                OriginalLanguageId = sourceLanguage.Id,
                TargetLanguageId = targetLanguage.Id,
                SystemTypeId = selectedSystemType.Id
            };

            var fileName = GenerateFileName(query);
            var response = await XliffHttpClient.CreateAndGetExportFile(query, fileName);
            var decoded = System.Text.Encoding.UTF8.GetString(response);
            await JSRuntime.InvokeVoidAsync("download", new
            {
                ByteArray = decoded.Replace("\"", ""),
                FileName = fileName
            });
        }

        private string GenerateFileName(GetXliffFileQuery query)
        {
            var datePart = string.Format("{0:yyyyMMdd}", DateTime.Now);
            var timePart = string.Format("{0:HHmmss}", DateTime.Now);
            var sourceLanguageShortName = Languages.SingleOrDefault(x => x.Id == query.OriginalLanguageId).ShortName;
            var targetLanguageShortName = Languages.SingleOrDefault(x => x.Id == query.TargetLanguageId).ShortName;
            var fileName = "CabLa_" + sourceLanguageShortName + "_" + targetLanguageShortName + "_" + datePart + "_" + timePart + ".xlf";
            return fileName;
        }
    }
}