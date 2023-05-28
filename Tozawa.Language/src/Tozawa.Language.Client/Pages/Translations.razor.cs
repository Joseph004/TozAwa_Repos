using Tozawa.Language.Client.HttpClients;
using Tozawa.Language.Client.Models.DTOs;
using Tozawa.Language.Client.Models.Enum;
using Tozawa.Language.Client.Models.FormModels;
using Tozawa.Language.Client.Pages.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Tozawa.Language.Client.Pages
{
    public partial class Translations : ComponentBase
    {
        [Inject] protected ILanguagesHttpClient LanguageHttpClient { get; set; }
        [Inject] protected ISystemTypeHttpClient SystemTypeClient { get; set; }
        [Inject] protected ITranslationHttpClient TranslationHttpClient { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }

        public List<LanguageDto> Languages { get; set; } = new List<LanguageDto>();
        public List<SystemTypeDto> SystemTypes { get; set; } = new List<SystemTypeDto>();

        private LanguageDto sourceLanguage = new();
        private LanguageDto targetLanguage = new();
        private SystemTypeDto selectedSystemType = new();
        private List<TranslatedTextDto> CopyTranslations { get; set; } = new();
        protected string _searchString = null;
        protected string xliffStateFilter;
        private MudTable<TranslatedTextDto> _table = new MudTable<TranslatedTextDto>();
        protected bool _loading = true;
        protected int _rowsPerPage = 20;
        protected int[] _rowsPerPageOptions = new int[] { 4, 8, 12, 16, 20, 24 };
        private string sourceLanguageText;
        private string targetLanguageText;
        private string systemTypeText;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await populateDate();
                StateHasChanged();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
        /*  protected override async Task OnInitializedAsync()
         {
         } */
        public async Task populateDate()
        {
            Languages.AddRange(await LanguageHttpClient.Get(false));
            SystemTypes.AddRange(await SystemTypeClient.Get());

            sourceLanguage = Languages.FirstOrDefault(x => x.IsDefault);
            sourceLanguageText = sourceLanguage.LongName;
            selectedSystemType = SystemTypes.FirstOrDefault(x => x.IsDefault);
            systemTypeText = selectedSystemType.Description;
        }
        private async Task<TableData<TranslatedTextDto>> TableReload(TableState state)
        {
            var translations = new TableData<TranslatedTextDto>() { TotalItems = 0, Items = Enumerable.Empty<TranslatedTextDto>() };
            if (!string.IsNullOrEmpty(sourceLanguageText) && !string.IsNullOrEmpty(targetLanguageText) && !string.IsNullOrEmpty(systemTypeText))
            {
                _loading = true;
                StateHasChanged();
                translations = await TranslationHttpClient.GetPaged(CreateQueryParameters(state));
                CopyTranslations = translations.Items != null
                        ? translations.Items.Select(x => (TranslatedTextDto)x.Clone()).ToList()
                        : new List<TranslatedTextDto>();
                _loading = false;
            }
            return translations;
        }
        private bool HasValidSearchParameters()
        {
            if (!string.IsNullOrEmpty(sourceLanguageText))
            {
                sourceLanguage = Languages.Where(x => x.LongName.Equals(sourceLanguageText, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(targetLanguageText))
            {
                targetLanguage = Languages.Where(x => x.LongName.Equals(targetLanguageText, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(systemTypeText))
            {
                selectedSystemType = SystemTypes.Where(x => x.Description.Equals(systemTypeText, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }
            return sourceLanguage != null && targetLanguage != null && selectedSystemType != null;
        }

        private GetTranslationSummaryQuery CreateQueryParameters(TableState state)
        {
            return new GetTranslationSummaryQuery()
            {
                Page = state.Page + 1,
                PageSize = state.PageSize,
                SourceLanguageId = sourceLanguage.Id,
                TargetLanguageId = targetLanguage.Id,
                SystemTypeId = selectedSystemType.Id,
                FilterText = _searchString,
                XliffState = string.IsNullOrEmpty(xliffStateFilter) ? null : (XliffState)Convert.ToInt32(xliffStateFilter)
            };
        }

        protected async void OnSearch(string text)
        {
            if (text != _searchString)
            {
                Snackbar.Add("Searching...", Severity.Info);
                _loading = true;
                xliffStateFilter = "";
                _searchString = text;
                await _table.ReloadServerData();
                _loading = false;
                Snackbar.Clear();
            }
        }

        private async Task UpdateTranslation(TranslatedTextDto translatedTextDto)
        {
            var existingValue = CopyTranslations.SingleOrDefault(x => x.Id == translatedTextDto.Id).Target;
            if (!string.IsNullOrEmpty(translatedTextDto.Target) && existingValue != translatedTextDto.Target)
            {
                CopyTranslations.SingleOrDefault(x => x.Id == translatedTextDto.Id).Target = translatedTextDto.Target;
                Snackbar.Add("Updating translation...", Severity.Info);
                _loading = true;
                var command = new UpdateTextCommand
                {
                    Text = translatedTextDto.Target,
                    Id = translatedTextDto.Id,
                    LanguageId = targetLanguage.Id,
                    SystemTypeId = selectedSystemType.Id,
                    XliffState = string.IsNullOrEmpty(translatedTextDto.Target) ? XliffState.NeedsTranslation : XliffState.Translated
                };

                await TranslationHttpClient.UpdateTextCommand(command);
                await _table.ReloadServerData();
                _loading = false;
                Snackbar.Clear();
            }
        }

        private async void Delete(TranslatedTextDto translatedTextDto)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var dialog = DialogService.Show<ConfirmationDialog>("Are you sure", options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                Snackbar.Add("Deleting translation...", Severity.Info);
                _loading = true;
                await TranslationHttpClient.Delete(translatedTextDto.Id);
                await _table.ReloadServerData();
                _loading = false;
                Snackbar.Clear();
            }
        }

        private async void SwitchLanguages()
        {
            var tempLanguage = targetLanguage;
            targetLanguage = sourceLanguage;
            sourceLanguage = tempLanguage;

            sourceLanguageText = sourceLanguage.LongName;
            targetLanguageText = targetLanguage.LongName;
            StateHasChanged();
            if (HasValidSearchParameters())
            {
                Snackbar.Add("Loading", Severity.Info);
                _loading = true;
                await _table.ReloadServerData();
                _loading = false;
                Snackbar.Clear();
            }
        }

        private async void OnChange(ChangeEventArgs args, TranslatedTextDto translatedTextDto)
        {
            translatedTextDto.XliffState = translatedTextDto.XliffState;
            var updateCommand = new UpdateTextCommand()
            {
                Id = translatedTextDto.Id,
                SystemTypeId = translatedTextDto.SystemTypeId,
                Text = translatedTextDto.Target,
                LanguageId = targetLanguage.Id,
                XliffState = translatedTextDto.XliffState
            };
            Snackbar.Add("Updating text", Severity.Info);
            _loading = true;
            await TranslationHttpClient.UpdateTextCommand(updateCommand);
            await _table.ReloadServerData();
            _loading = false;
            Snackbar.Clear();
        }

        private async void Search()
        {
            Snackbar.Add("Searching", Severity.Info);
            _loading = true;
            if (xliffStateFilter != "")
            {
                _searchString = string.Empty;
            }
            await _table.ReloadServerData();
            _loading = false;
            Snackbar.Clear();
        }

        private async void OpenAddDialog()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var dialog = DialogService.Show<AddTranslationDialog>($"Add translation on {sourceLanguage.Name}", options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                if (result.Data is not string)
                    return;
                var text = result.Data as string;
                var command = new AddTextCommand()
                {
                    LanguageId = sourceLanguage.Id,
                    SystemTypeId = selectedSystemType.Id,
                    Text = text
                };
                Snackbar.Add("Translation added", Severity.Success);
                _loading = true;
                await TranslationHttpClient.AddTextTranslation(command);
                await _table.ReloadServerData();
                _loading = false;
                Snackbar.Clear();
            }
        }
    }
}