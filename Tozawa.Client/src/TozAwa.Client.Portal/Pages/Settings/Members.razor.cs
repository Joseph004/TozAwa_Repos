using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tozawa.Client.Portal.HttpClients.Helpers;
using Tozawa.Client.Portal.Models.Dtos;
using Tozawa.Client.Portal.Models.Enums;
using Tozawa.Client.Portal.Models.FormModels;
using Tozawa.Client.Portal.Models.ResponseRequests;
using Tozawa.Client.Portal.Services;
using Tozawa.Client.Portal.Shared;

namespace Tozawa.Client.Portal.Pages
{
    public partial class Members : BasePage
    {
        [Inject] protected ObjectTextService objectTextService { get; set; }
        [Inject] protected MemberService memberService { get; set; }
        [Inject] IDialogService DialogService { get; set; }
        [Inject] private ISnackbar SnackBar { get; set; }
        [Inject] private ISnackBarService snackBarService { get; set; }
        protected IEnumerable<MemberDto> _pagedData;
        private MudTable<MemberDto> _table;
        private MemberDto _addedItem;
        protected bool _includeDeleted;
        protected bool _loading = true;

        protected int _totalItems;
        protected string _searchString = null;
        private MemberDto _backupItem;
        protected MemberDto _selectedItem = null;
        protected MemberDto _previousSelectedItem = new();
        private string _pageOfEmail = null;
        public int ThumbnailSize = 24;
        public string _disabledPointer = "";
        protected int[] _pageSizeOptions = new[] { 20, 50, 100 };

        protected override async Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += LanguageChanged;

            await base.OnInitializedAsync();
        }
        private bool DisabledEditRow()
        {
            var entity = _selectedItem ?? new MemberDto();
            return entity.Deleted;
        }
        private async void LanguageChanged(object sender, EventArgs e)
        {
            if (_table != null)
            {
                await _table.ReloadServerData();
            }
            StateHasChanged();
        }
        private void BackupItem(object element)
        {
            _backupItem = new()
            {
                FirstName = ((MemberDto)element).FirstName,
                LastName = ((MemberDto)element).LastName,
                Email = ((MemberDto)element).Email,
                Description = ((MemberDto)element).Description,
            };
        }
        protected async Task ToggleDeleted(MemberDto item, bool hardDelete = false)
        {
            var options = new DialogOptions
            {
                MaxWidth = MaxWidth.Large
            };

            var parameters = new DialogParameters
            {
                ["hardDelete"] = hardDelete,
                ["body"] = Translate(SystemTextId.AreYouSure),
                ["item"] = item,
                ["title"] = item.Deleted ? hardDelete ? Translate(SystemTextId.Delete) : Translate(SystemTextId.Restore) : Translate(SystemTextId.Delete)
            };

            var dialog = DialogService.Show<DeleteEntityDialog>(item.Deleted ? Translate(SystemTextId.Restore) : Translate(SystemTextId.Delete), parameters, options);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                _loading = true;
                StateHasChanged();
                SnackBar.Add(Translate(SystemTextId.Processing), Severity.Info);

                var modalResponse = (DeleteRequest)result.Data;
                var patchRequest = new PatchMemberRequest();

                if (modalResponse.SoftDeleted)
                {
                    patchRequest = new PatchMemberRequest { SoftDeleted = modalResponse.SoftDeleted };
                }

                if (modalResponse.HardDeleted)
                {
                    patchRequest = new PatchMemberRequest { HardDeleted = modalResponse.HardDeleted };
                }

                var updateResponse = await memberService.PatchMember(item.Id, patchRequest);

                _pageOfEmail = null;
                snackBarService.Add(updateResponse);

                await _table.ReloadServerData();
            }
        }
        protected async Task ToggleFiles(MemberDto item)
        {
            var options = new DialogOptions
            {
                DisableBackdropClick = true,
                MaxWidth = MaxWidth.Large,
                CloseButton = true
            };
            var parameters = new DialogParameters
            {
                ["Entity"] = item,
                ["HasPermission"] = HasAtLeastOneFunctionType(FunctionsType.WriteMember)
            };
            var userName = item.RootUser ? item.UserName : item.Email;
            DialogService.Show<FilesEntityDialog>($"{userName}", parameters, options);
        }
        protected async Task ToggleIncludeDeleted()
        {
            _includeDeleted = !_includeDeleted;
            await _table.ReloadServerData();
        }
        private void ResetItemToOriginalValues(object element)
        {
            ((MemberDto)element).FirstName = _backupItem.FirstName;
            ((MemberDto)element).Description = _backupItem.Description;
            ((MemberDto)element).LastName = _backupItem.LastName;
            ((MemberDto)element).Email = _backupItem.Email;
        }

        private bool AnyTextIsUpdated()
        {
            return !_backupItem.Description.Equals(_selectedItem.Description);
        }
        private bool AnyPropertyIsUpdated()
        {
            return !_backupItem.FirstName.Equals(_selectedItem.FirstName) || !_backupItem.LastName.Equals(_selectedItem.LastName)
                || !_backupItem.Email.Equals(_selectedItem.Email);
        }
        protected async Task<TableData<MemberDto>> ServerReload(TableState state)
        {
            _loading = true;

            var data = await memberService.GetItems(state, _includeDeleted, _searchString, _pageOfEmail);
            var entity = data.Entity ?? new TableData<MemberDto>();
            _pageOfEmail = null;

            var items = entity.Items ?? Enumerable.Empty<MemberDto>();

            if (items.Any())
            {
                if (_addedItem != null)
                {
                    if (items.Any(x => x.Id == _addedItem.Id))
                    {
                        _selectedItem = items.SingleOrDefault(x => x.Id == _addedItem.Id);
                        _addedItem = null;
                    }
                }
                else
                {
                    if (_selectedItem != null && !items.Any(x => x.Id == _selectedItem.Id))
                    {
                        _selectedItem = items.FirstOrDefault();
                        _previousSelectedItem = _selectedItem;
                    }
                }
            }
            else
            {
                _selectedItem = null;
                _previousSelectedItem = new();
            }

            _loading = false;
            StateHasChanged();
            return entity;
        }
        public string GetDescription(MemberDto context)
        {
            if (!string.IsNullOrEmpty(context.Description) && !context.Description.Equals("Not Translated")) return context.Description;

            context.Description = "";
            return "";
        }
        public string GetHelpText(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Equals("Not Translated")) return Translate(SystemTextId.DescritionMissing, "The description is missing");

            return "";
        }
        protected void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }
        protected void RowClickEvent(TableRowClickEventArgs<MemberDto> tableRowClickEventArgs)
        {
            if (_previousSelectedItem != null && _previousSelectedItem.Id != tableRowClickEventArgs.Item.Id)
            {
                _previousSelectedItem = tableRowClickEventArgs.Item;
                _addedItem = null;
            }
        }
        private async Task OpenDialog()
        {
            var options = new DialogOptions
            {
                MaxWidth = MaxWidth.ExtraLarge
            };
            var parameters = new DialogParameters
            {
                ["_activeLanguages"] = ActiveLanguages
            };
            var dialog = DialogService.Show<AddMembersDialog>($"{Translate(SystemTextId.Add)} {Translate(SystemTextId.Member)}", parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                if (result.Data is AddResponse<MemberDto> data)
                {
                    _searchString = null;
                    _pageOfEmail = data.Entity.Email;
                    _addedItem = data.Entity;
                    await _table.ReloadServerData();
                }
            }
        }
        private async Task ItemHasBeenCommitted()
        {
            var item = _selectedItem;
            if (!item.Deleted)
            {
                if (AnyTextIsUpdated())
                {
                    var updateTextsRequest = new UpdateObjectTextCommand(UpdateEntityType.Member, item.Id);

                    if (!_backupItem.Description.Equals(_selectedItem.Description))
                    {
                        updateTextsRequest.Description = _selectedItem.Description;
                        updateTextsRequest.DescriptionTextId = _selectedItem.DescriptionTextId;
                    }
                    var updateTextResponse = await objectTextService.UpdateObjectText(updateTextsRequest);
                    snackBarService.Add(updateTextResponse);
                }
                if (AnyPropertyIsUpdated())
                {
                    var patchRequest = new PatchMemberRequest { FirstName = item.FirstName, LastName = item.LastName, Email = item.Email };
                    var updateResponse = await memberService.PatchMember(_selectedItem.Id, patchRequest);
                    snackBarService.Add(updateResponse);

                    if (!updateResponse.Success)
                    {
                        _selectedItem.FirstName = _backupItem.FirstName;
                        _selectedItem.LastName = _backupItem.LastName;
                        _selectedItem.Email = _backupItem.Email;
                        return;
                    }
                }
                await _table.ReloadServerData();
            }
        }
        protected string SelectedRowClassFunc(MemberDto element, int rowNumber)
        {
            if (_selectedItem != null && _selectedItem.Id == element.Id)
            {
                return "selected";
            }
            if (_addedItem != null && _addedItem.Id == element.Id)
            {
                return "created";
            }
            return string.Empty;
        }

        public override void Dispose()
        {
            _translationService.LanguageChanged -= LanguageChanged;
        }
    }
}