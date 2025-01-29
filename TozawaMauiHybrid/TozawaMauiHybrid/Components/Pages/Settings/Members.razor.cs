using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions.Options;
using MudBlazor.Extensions;
using Nextended.Core.Extensions;
using MudBlazor.Extensions.Core;
using TozawaMauiHybrid.Component;
using TozawaMauiHybrid.Services;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.FormModels;
using TozawaMauiHybrid.State.Member.Store;
using TozawaMauiHybrid.Pages;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Models.Enums;

namespace TozawaMauiHybrid.Components.Pages.Settings
{
    public partial class Members : BasePage
    {
        [Inject] IDialogService DialogService { get; set; }
        [Inject] MemberService memberService { get; set; }
        [Inject] private LoadingState LoadingState { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
        [Inject] IState<State.Member.Store.MemberState> MemberState { get; set; }
        [Inject] Fluxor.IDispatcher Dispatcher { get; set; }
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

            _includeDeleted = MemberState.Value.IncludeDeleted;
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
                b.WithBackgroundImage("url('/images/plain-white-background.jpg')")
               .WithBackgroundSize("cover")
               .WithBackgroundPosition("center center")
               .WithBackgroundRepeat("no-repeat")
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
        protected async Task ToggleDeleted(MemberDto item, bool hardDelete = false)
        {
            var options = new DialogOptionsEx
            {
                BackgroundClass = "tz-mud-overlay",
                BackdropClick = false,
                CloseButton = false,
                MaxWidth = MaxWidth.Small,
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
                b.WithBackgroundImage("url('/images/plain-white-background.jpg')")
                .WithBackgroundSize("cover")
                .WithBackgroundPosition("center center")
                .WithBackgroundRepeat("no-repeat")
                .WithOpacity(0.9);
            });

            var parameters = new DialogParameters
            {
                ["hardDelete"] = hardDelete,
                ["body"] = Translate(SystemTextId.AreYouSure),
                ["item"] = item,
                ["title"] = item.Deleted ? hardDelete ? Translate(SystemTextId.Delete) : Translate(SystemTextId.Restore) : Translate(SystemTextId.Delete)
            };

            var dialog = await DialogService.ShowEx<DeleteEntityDialog>(item.Deleted ? Translate(SystemTextId.Restore) : Translate(SystemTextId.Delete), parameters, options);
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
                b.WithBackgroundImage("url('/images/plain-white-background.jpg')")
              .WithBackgroundSize("cover")
              .WithBackgroundPosition("center center")
              .WithBackgroundRepeat("no-repeat")
              .WithOpacity(0.9);
            });

            var parameters = new DialogParameters
            {
                ["Entity"] = item,
                ["HasPermission"] = HasAllFunctionTypesMatching(FunctionType.ReadPresident, FunctionType.WriteVicePresident),
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
        private async Task OpenDialog()
        {
            var options = new DialogOptionsEx
            {
                BackgroundClass = "tz-mud-overlay",
                BackdropClick = false,
                CloseButton = false,
                MaxWidth = MaxWidth.ExtraLarge,
                MaximizeButton = true,
                FullHeight = true,
                FullWidth = true,
                DragMode = MudDialogDragMode.Simple,
                Animations = [AnimationType.Pulse],
                Position = DialogPosition.Center
            };

            options.SetProperties(ex => ex.Resizeable = true);
            options.DialogAppearance = MudExAppearance.FromStyle(b =>
            {
                b.WithBackgroundImage("url('/images/plain-white-background.jpg')")
              .WithBackgroundSize("cover")
              .WithBackgroundPosition("center center")
              .WithBackgroundRepeat("no-repeat")
              .WithOpacity(0.9);
            });

            var parameters = new DialogParameters
            {
                ["_activeLanguages"] = ActiveLanguages,
                ["Title"] = $"{Translate(SystemTextId.Add)} {Translate(SystemTextId.Member)}"
            };
            var dialog = await DialogService.ShowEx<AddMembersDialog>($"{Translate(SystemTextId.Add)} {Translate(SystemTextId.Member)}", parameters, options);
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
            if (member.Deleted) return;

            var options = new DialogOptionsEx
            {
                BackgroundClass = "tz-mud-overlay",
                BackdropClick = false,
                CloseButton = false,
                MaxWidth = MaxWidth.Medium,
                MaximizeButton = true,
                FullHeight = true,
                FullWidth = true,
                DragMode = MudDialogDragMode.Simple,
                Animations = [AnimationType.Pulse],
                Position = DialogPosition.Center
            };

            options.SetProperties(ex => ex.Resizeable = true);
            options.DialogAppearance = MudExAppearance.FromStyle(b =>
            {
                b.WithBackgroundImage("url('/images/plain-white-background.jpg')")
               .WithBackgroundSize("cover")
               .WithBackgroundPosition("center center")
               .WithBackgroundRepeat("no-repeat")
               .WithOpacity(0.9);
            });

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
            var dialog = await DialogService.ShowEx<EditMembersDialog>($"Edit", parameters, options);
            var result = await dialog.Result;
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