using Tozawa.Language.Client.HttpClients;
using Tozawa.Language.Client.Models.DTOs;
using Tozawa.Language.Client.Pages.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tozawa.Language.Client.Pages
{
    public partial class LanguagesPage : ComponentBase
    {
        [Inject] protected ILanguagesHttpClient LanguageHttpClient { get; set; }
        [Inject] IDialogService DialogService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }

        protected string _searchString = null;
        private MudTable<LanguageDto> _table;
        protected bool _loading = true;
        protected int _rowsPerPage = 8;
        protected int[] _rowsPerPageOptions = new int[] { 8, 16, 24, 32 };

        private async Task<TableData<LanguageDto>> TableReload(TableState state)
        {
            _loading = true;
            var languages = await LanguageHttpClient.GetPaged(CreateQueryParameters(state));
            _loading = false;
            return languages;
        }

        private Dictionary<string, string> CreateQueryParameters(TableState state)
        {
            return new Dictionary<string, string> {
                { "page", (state.Page + 1).ToString() },
                { "pageSize", state.PageSize.ToString() },
                { "searchString", _searchString }
            };
        }

        protected void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }

        public async void OnToggledDefault(LanguageDto languageDto)
        {
            await LanguageHttpClient.SetAsDefault(languageDto.Id);
            await _table.ReloadServerData();
        }

        public async void OnToggledDeleted(LanguageDto languageDto)
        {
            await LanguageHttpClient.DeleteLanguage(languageDto.Id);
            Snackbar.Add("Language deleted", Severity.Info);
            await _table.ReloadServerData();
        }

        private async void OpenAddDialog()
        {
            var dialog = DialogService.Show<AddLanguageDialog>("Add language");
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var newLanguage = (LanguageDto)result.Data;
                await LanguageHttpClient.AddLanguage(newLanguage);
                Snackbar.Add("Language added", Severity.Success);
                await _table.ReloadServerData();
            }
        }
    }
}
