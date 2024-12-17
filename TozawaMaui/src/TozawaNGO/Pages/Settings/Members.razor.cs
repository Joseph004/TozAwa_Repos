using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using TozawaNGO.Services;
using TozawaNGO.Shared;
using TozawaNGO.State.Member.Store;
using TozawaNGO.StateHandler;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Services;
using ShareRazorClassLibrary.Models.FormModels;
using ShareRazorClassLibrary.Helpers;
using MudBlazor.Extensions.Options;
using MudBlazor.Extensions;
using Nextended.Core.Extensions;
using MudBlazor.Extensions.Core;

namespace TozawaNGO.Pages
{
    public partial class Members : BasePage
    {
        [Inject] IDialogService DialogService { get; set; }
        [Inject] private ISnackbar SnackBar { get; set; }
        [Inject] private ISnackBarService snackBarService { get; set; }
        [Inject] MemberService memberService { get; set; }
        [Inject] private LoadingState LoadingState { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
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
        private double scrollTop;

        protected override async Task OnInitializedAsync()
        {
            ScrollTopState.SetSource("memberPage");
            FirstloadState.OnChange += FirsLoadChanged;
            _translationService.LanguageChanged += LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;
            ScrollTopState.OnChange += SetScroll;
            LoadingState.SetRequestInProgress(true);

            ScrollTopState.ScrollTop.TryGetValue(ScrollTopState.Source, out double scroll);
            scrollTop = scroll;

            Dispatcher.Dispatch(new MemberDataAction(_page, _pageSize, _searchString, MemberState.Value.IncludeDeleted, MemberState.Value.PageOfEmail, MemberState.Value.Email, ScrollTopState.ScrollTop.TryGetValue(ScrollTopState.Source, out double value) ? value : 0, LoadingState, JSRuntime));
            await base.OnInitializedAsync();
        }
        private void FirsLoadChanged()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private string GetDescColor(MemberDto member)
        {
            return string.IsNullOrEmpty(member.Description) ? $"color: #c4c4c4;" : "";
        }
        private async Task ShowLongText(MemberDto member)
        {
            if (string.IsNullOrEmpty(member.Description)) return;
            var options = new DialogOptionsEx
            {
                BackgroundClass = "tz-mud-overlay",
                BackdropClick = false,
                CloseButton = false,
                MaxWidth = MaxWidth.Medium,
                MaximizeButton = true,
                FullHeight = false,
                FullWidth = true,
                DragMode = MudDialogDragMode.Simple,
                Animations = [AnimationType.Pulse],
                Position = DialogPosition.Center
            };

            options.SetProperties(ex => ex.Resizeable = true);
            options.DialogAppearance = MudExAppearance.FromStyle(b =>
            {
                b.WithBackgroundColor("gold")
                .WithOpacity(0.9);
            });

            var parameters = new DialogParameters
            {
                ["Entity"] = member,
                ["Title"] = member.FirstName + " " + member.LastName
            };
            var dialog = await DialogService.ShowEx<DescriptionMemberDialog>(member.FirstName + " " + member.LastName, parameters, options);
            var result = await dialog.Result;
        }
        private void SetLoading()
        {
            LoadingState.SetRequestInProgress(MemberState.Value.IsLoading);
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
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                LoadingState.SetRequestInProgress(true);
            }
            if (!MemberState.Value.IsLoading && MemberState.Value.Members.Count > 0 && FirstloadState.IsFirstLoaded)
            {
                //await SetDescriptionIcon();
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
        private void LanguageChanged(object sender, EventArgs e)
        {
            ReloadData();
        }
        private void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            ReloadData();
        }
        protected async Task ToggleFiles(MemberDto item)
        {
            await Task.FromResult(1);
            var options = new DialogOptionsEx
            {
                BackgroundClass = "tz-mud-overlay",
                BackdropClick = false,
                CloseButton = false,
                MaxWidth = MaxWidth.Large,
                MaximizeButton = true,
                FullHeight = false,
                FullWidth = true,
                DragMode = MudDialogDragMode.Simple,
                Animations = [AnimationType.Pulse],
                Position = DialogPosition.Center
            };

            options.SetProperties(ex => ex.Resizeable = true);
            options.DialogAppearance = MudExAppearance.FromStyle(b =>
            {
                b.WithBackgroundColor("gold")
                .WithOpacity(0.9);
            });

            var parameters = new DialogParameters
            {
                ["Entity"] = item,
                ["HasPermission"] = HasAtLeastOneRole(RoleDto.President.ToString()),
                ["Source"] = nameof(MemberDto)
            };
            var userName = item.Admin ? item.UserName : item.Email;
            await DialogService.ShowEx<FilesEntityDialog>($"{userName}", parameters, options);
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
        private string GetLabel(Guid labelId, string label)
        {
            return Translate(labelId, label);
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
                FirstloadState.OnChange -= FirsLoadChanged;
                _translationService.LanguageChanged -= LanguageChanged;
                _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
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