using System.Collections.Generic;
using System.Threading.Tasks;
using Tozawa.Language.Client.HttpClients;
using Tozawa.Language.Client.Models.DTOs;
using Tozawa.Language.Client.Pages.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Tozawa.Language.Client.Pages
{
    public partial class SystemTypesPage : ComponentBase
    {
        [Inject] protected ISystemTypeHttpClient SystemTypeClient { get; set; }
        [Inject] IDialogService DialogService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }

        protected string _searchString = null;
        private MudTable<SystemTypeDto> _table;
        protected bool _loading = true;
        protected int _rowsPerPage = 8;
        protected int[] _rowsPerPageOptions = new int[] { 8, 16, 24, 32 };

        private async Task<TableData<SystemTypeDto>> TableReload(TableState state)
        {
            _loading = true;
            var systemTypes = await SystemTypeClient.GetPaged(CreateQueryParameters(state));
            _loading = false;
            return systemTypes;
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

        public async void OnToggledChanged(SystemTypeDto systemTypeDto)
        {
            await SystemTypeClient.SetAsDefault(systemTypeDto.Id);
            await _table.ReloadServerData();
        }

        private async void OpenAddDialog()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var dialog = DialogService.Show<AddSystemTypeDialog>("Add system type", options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {                
                await SystemTypeClient.CreateSystemType((string)result.Data);
                Snackbar.Add("Systemtype added", Severity.Success);
                await _table.ReloadServerData();
            }
        }

        private async void Delete(SystemTypeDto systemTypeDto)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var dialog = DialogService.Show<ConfirmationDialog>("Are you sure", options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await SystemTypeClient.DeleteSystemType(systemTypeDto.Id);
                Snackbar.Add("Systemtype deleted", Severity.Info);
                await _table.ReloadServerData();
            }
        }
    }
}