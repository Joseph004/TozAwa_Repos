using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using TozawaNGO.Helpers;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.FormModels;
using TozawaNGO.Services;
using TozawaNGO.Shared;
using TozawaNGO.State.Member.Store;
using TozawaNGO.StateHandler;

namespace TozawaNGO.Pages
{
    public partial class Members : BasePage
    {
        [Inject] IDialogService DialogService { get; set; }
        [Inject] private ISnackbar SnackBar { get; set; }
        [Inject] private AttachmentService AttachmentService { get; set; }
        [Inject] private ISnackBarService snackBarService { get; set; }
        [Inject] MemberService memberService { get; set; }
        [Inject] private LoadingState LoadingState { get; set; }
        [Inject] IState<TozawaNGO.State.Member.Store.MemberState> MemberState { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ScrollTopState ScrollTopState { get; set; }

        protected IEnumerable<MemberDto> _pagedData = [];
        private MudTable<MemberDto> _table;
        protected bool _includeDeleted;
        private Dictionary<Guid, string> DescriptionIcon = [];

        protected int _totalItems;
        protected string _searchString = null;
        protected string _page = "0";
        protected string _pageSize = "20";
        private MemberDto _selectedItem;
        protected PatchMemberRequest _patchMemberRequest = new();
        public int ThumbnailSize = 24;
        protected int[] _pageSizeOptions = [20, 50, 100];
        private bool firstLoaded;
        private double scrollTop;

        protected override async Task OnInitializedAsync()
        {
            ScrollTopState.SetSource("memberPage");
            _translationService.LanguageChanged += LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;
            AttachmentService.OnChange += UpdateMemberAttachments;
            ScrollTopState.OnChange += SetScroll;
            LoadingState.SetRequestInProgress(true);

            ScrollTopState.ScrollTop.TryGetValue(ScrollTopState.Source, out double scroll);
            scrollTop = scroll;

            Dispatcher.Dispatch(new MemberDataAction(_page, _pageSize, _searchString, MemberState.Value.IncludeDeleted, MemberState.Value.PageOfEmail, MemberState.Value.Email, ScrollTopState.ScrollTop.TryGetValue(ScrollTopState.Source, out double value) ? value : 0, LoadingState, JSRuntime));
            await base.OnInitializedAsync();
        }
        private string GetDescColor(MemberDto member)
        {
            return string.IsNullOrEmpty(member.Description) ? $"color: #c4c4c4;" : "";
        }
        private async Task ShowLongText(MemberDto member)
        {
            if (string.IsNullOrEmpty(member.Description)) return;
            var options = new DialogOptions
            {
                DisableBackdropClick = true,
                MaxWidth = MaxWidth.Medium,
                CloseButton = true
            };
            var parameters = new DialogParameters
            {
                ["Entity"] = member
            };
            var dialog = DialogService.Show<DescriptionMemberDialog>(member.FirstName + " " + member.LastName, parameters, options);
            var result = await dialog.Result;
        }
        private void SetLoading()
        {
            LoadingState.SetRequestInProgress(true);
        }
        private int Count = 0;
        private async Task SetScrollJS()
        {
            if (Count != 0) return;
            if (scrollTop != 0)
            {
                Count++;
                _selectedItem = MemberState.Value.Members.First();
                await JSRuntime.InvokeAsync<object>("SetScroll", (-1) * scrollTop);
            }
        }
        /* private async Task SetDescriptionIcon()
        {
            foreach (var textField in MemberState.Value.MudTextField)
            {
                DescriptionIcon.TryAdd(textField.Key, "");
                var dotNetReference = DotNetObjectReference.Create(this);
                var isOverflowed = await JSRuntime.InvokeAsync<bool>("checkOverflow", textField.Key, dotNetReference);
                if (isOverflowed)
                {
                    DescriptionIcon[textField.Key] = Icons.Material.Outlined.Info;
                }
            }
            StateHasChanged();
        } 
        [JSInvokable("AddDescIcon")]
        public void SetDescriptionIcon(string id, bool isRemoved = false)
        {
            Guid.TryParse(id, out Guid guid);
            if (guid == Guid.Empty) return;
            var member = MemberState.Value.Members.First(x => x.Id == guid);
            if (DescriptionIcon.ContainsKey(guid))
            {
                if (isRemoved)
                {
                    DescriptionIcon[guid] = "";
                    StateHasChanged();
                    return;
                }

                if (!string.IsNullOrEmpty(member.Description) && member.Description.Length > 5)
                {
                    DescriptionIcon[guid] = Icons.Material.Outlined.Info;

                    StateHasChanged();
                }
            }
        }*/
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                firstLoaded = true;
                LoadingState.SetRequestInProgress(true);
            }
            if (!MemberState.Value.IsLoading && MemberState.Value.Members.Count > 0 && firstLoaded)
            {
                //await SetDescriptionIcon();
                firstLoaded = false;
                LoadingState.SetRequestInProgress(false);
                await Task.Delay(new TimeSpan(0, 0, Convert.ToInt32(0.5))).ContinueWith(async o => { await SetScrollJS(); });
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        private string GetTextFieldClassNames(MemberDto member)
        {
            return "memberDescription" + " " + member.Id.ToString();
        }
        private void ReloadData()
        {
            SetLoading();
            ScrollTopState.ScrollTop.TryGetValue(ScrollTopState.Source, out double scroll);
            scrollTop = scroll;
            Dispatcher.Dispatch(new MemberDataAction(_page, _pageSize, _searchString, _includeDeleted, MemberState.Value.PageOfEmail, MemberState.Value.Email, scrollTop, LoadingState, JSRuntime));
        }
        private bool DisabledEditRow()
        {
            var entity = _selectedItem ?? new MemberDto();
            return entity.Deleted;
        }
        private void UpdateMemberAttachments()
        {
            ReloadData();
        }
        private void LanguageChanged(object sender, EventArgs e)
        {
            ReloadData();
        }
        private void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            ReloadData();
        }
        protected async Task ToggleDeleted(MemberDto item, bool hardDelete = false)
        {
            var options = new DialogOptions
            {
                MaxWidth = MaxWidth.Small,
                CloseButton = false
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

            if (!result.Canceled)
            {
                var modalResponse = (DeleteRequest)result.Data;
                var patchRequest = new PatchMemberRequest
                {
                    Deleted = modalResponse.SoftDeleted
                };

                if (modalResponse.HardDeleted)
                {
                    patchRequest.Deleted = null;
                    patchRequest.DeleteForever = modalResponse.HardDeleted;
                }

                LoadingState.SetRequestInProgress(true);
                Dispatcher.Dispatch(new MemberPatchAction(item.Id, patchRequest, item));
            }
        }
        protected async Task ToggleFiles(MemberDto item)
        {
            await Task.FromResult(1);
            var options = new DialogOptions
            {
                DisableBackdropClick = true,
                MaxWidth = MaxWidth.Large,
                CloseButton = false
            };
            var parameters = new DialogParameters
            {
                ["Entity"] = item,
                ["HasPermission"] = HasAtLeastOneRole(RoleDto.President.ToString()),
                ["Source"] = nameof(MemberDto)
            };
            var userName = item.Admin ? item.UserName : item.Email;
            DialogService.Show<FilesEntityDialog>($"{userName}", parameters, options);
        }
        protected async Task ToggleIncludeDeleted()
        {
            _includeDeleted = !_includeDeleted;
            ReloadData();
            await Task.CompletedTask;
        }
        public string GetDescription(MemberDto context)
        {
            if (!string.IsNullOrEmpty(context.Description) && !context.Description.Equals("Not Translated"))
            {
                /* if (context.Description.Length > 20)
                {
                    return context.Description[..19];
                }
                else
                { } */
                return context.Description;
            }

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
            ReloadData();
        }
        protected void RowClickEvent(TableRowClickEventArgs<MemberDto> tableRowClickEventArgs)
        {
            Dispatcher.Dispatch(new MemberSelectedAction(tableRowClickEventArgs.Item));
        }
        private async Task OpenDialog()
        {
            var options = new DialogOptions
            {
                DisableBackdropClick = true,
                MaxWidth = MaxWidth.ExtraLarge,
                CloseButton = false
            };
            var parameters = new DialogParameters
            {
                ["_activeLanguages"] = ActiveLanguages
            };
            var dialog = DialogService.Show<AddMembersDialog>($"{Translate(SystemTextId.Add)} {Translate(SystemTextId.Member)}", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                if (result.Data is AddMemberRequest model)
                {
                    var added = await memberService.AddMember(model);
                }
            }
        }
        private async Task ToggleEdit(MemberDto member)
        {
            var options = new DialogOptions
            {
                DisableBackdropClick = true,
                MaxWidth = MaxWidth.Medium,
                CloseButton = false
            };
            var data = new MemberDto
            {
                Id = member.Id,
                Deleted = member.Deleted,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email,
                Description = member.Description,
            };
            var parameters = new DialogParameters
            {
                ["Member"] = data
            };
            var dialog = DialogService.Show<EditMembersDialog>($"Edit", parameters, options);
            var result = await dialog.Result;
        }
        protected string SelectedRowClassFunc(MemberDto element, int rowNumber)
        {
            if (_selectedItem != null && _selectedItem.Id == element.Id)
            {
                return "selected";
            }
            return string.Empty;
        }
        private void SetScroll()
        {
            Dispatcher.Dispatch(new ScrollTopAction(ScrollTopState.ScrollTop[ScrollTopState.Source]));
        }
        protected override void Dispose(bool disposed)
        {
            try
            {
                _translationService.LanguageChanged -= LanguageChanged;
                _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
                AttachmentService.OnChange -= UpdateMemberAttachments;
                ScrollTopState.OnChange -= SetScroll;
                Dispatcher.Dispatch(new UnSubscribeAction());
            }
            catch
            {
            }
            base.Dispose(disposed);
        }
    }
}