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
using TozawaMauiHybrid.State.Feature.Store;
using TozawaMauiHybrid.Pages;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Models.Enums;

namespace TozawaMauiHybrid.Components.Pages.Settings
{
    public partial class Features : BasePage
    {
        [Inject] IDialogService DialogService { get; set; }
        [Inject] FeatureService FeatureService { get; set; }
        [Inject] private LoadingState LoadingState { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
        [Inject] IState<State.Feature.Store.FeatureState> FeatureState { get; set; }
        [Inject] Fluxor.IDispatcher Dispatcher { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ScrollTopState ScrollTopState { get; set; }

        protected IEnumerable<FeatureDto> _pagedData = [];
        private MudTable<FeatureDto> _table;
        protected bool _includeDeleted;
        private Dictionary<Guid, string> DescriptionIcon = [];

        protected int _totalItems;
        protected string _searchString = null;
        protected string _page = "0";
        protected string _pageSize = "20";
        private FeatureDto _selectedItem;
        protected PatchFeatureRequest _patchFeatureRequest = new();
        public int ThumbnailSize = 24;
        protected int[] _pageSizeOptions = [20, 50, 100];
        private double scrollTop;

        protected override async Task OnInitializedAsync()
        {
            ScrollTopState.SetSource("FeaturePage");
            FirstloadState.OnChange += FirsLoadChanged;
            _translationService.LanguageChanged += LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;
            ScrollTopState.OnChange += SetScroll;
            LoadingState.SetRequestInProgress(true);

            _includeDeleted = FeatureState.Value.IncludeDeleted;
            ScrollTopState.ScrollTop.TryGetValue(ScrollTopState.Source, out double scroll);
            scrollTop = scroll;

            Dispatcher.Dispatch(new FeatureDataAction(_page, _pageSize, _searchString, FeatureState.Value.IncludeDeleted, ScrollTopState.ScrollTop.TryGetValue(ScrollTopState.Source, out double value) ? value : 0, LoadingState, JSRuntime, FeatureState.Value.Id));
            await base.OnInitializedAsync();
        }
        private void FirsLoadChanged()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private async Task ShowLongText(FeatureDto feature, TypeOfText textType)
        {
            if (textType == TypeOfText.Description && string.IsNullOrEmpty(feature.Description)) return;
            if (textType == TypeOfText.Text && string.IsNullOrEmpty(feature.Text)) return;
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
                ["Entity"] = feature,
                ["TypeOfText"] = textType,
                ["Title"] = feature.Id.ToString()
            };
            var dialog = await DialogService.ShowEx<EntityTextDialog>(feature.Id.ToString(), parameters, options);
            var result = await dialog.Result;
        }
        private string GetTextColor(string text)
        {
            return string.IsNullOrEmpty(text) ? $"color: #c4c4c4;" : "";
        }
        private void SetLoading()
        {
            LoadingState.SetRequestInProgress(FeatureState.Value.IsLoading);
        }
        private int Count = 0;
        private async Task SetScrollJS()
        {
            if (Count != 0) return;
            if (scrollTop != 0)
            {
                Count++;
                _selectedItem = FeatureState.Value.Features.First();
                await JSRuntime.InvokeAsync<object>("SetScroll", (-1) * scrollTop);
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                LoadingState.SetRequestInProgress(true);
            }
            if (!FeatureState.Value.IsLoading && FeatureState.Value.Features.Count > 0 && FirstloadState.IsFirstLoaded)
            {
                LoadingState.SetRequestInProgress(false);
                await Task.Delay(new TimeSpan(0, 0, Convert.ToInt32(0.5))).ContinueWith(async o => { await SetScrollJS(); });
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        private void ReloadData()
        {
            SetLoading();
            ScrollTopState.ScrollTop.TryGetValue(ScrollTopState.Source, out double scroll);
            scrollTop = scroll;
            Dispatcher.Dispatch(new FeatureDataAction(_page, _pageSize, _searchString, _includeDeleted, scrollTop, LoadingState, JSRuntime, FeatureState.Value.Id));
        }
        private void LanguageChanged(object sender, EventArgs e)
        {
            ReloadData();
        }
        private void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            ReloadData();
        }
        protected async Task ToggleDeleted(FeatureDto item, bool hardDelete = false)
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
                ["item"] = new BaseDto { Deleted = item.Deleted },
                ["title"] = item.Deleted ? hardDelete ? Translate(SystemTextId.Delete) : Translate(SystemTextId.Restore) : Translate(SystemTextId.Delete)
            };

            var dialog = await DialogService.ShowEx<DeleteEntityDialog>(item.Deleted ? Translate(SystemTextId.Restore) : Translate(SystemTextId.Delete), parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var modalResponse = (DeleteRequest)result.Data;
                var patchRequest = new PatchFeatureRequest
                {
                    Deleted = modalResponse.SoftDeleted
                };

                LoadingState.SetRequestInProgress(true);
                Dispatcher.Dispatch(new FeaturePatchAction(item.Id, patchRequest, item));
            }
        }
        protected async Task ToggleIncludeDeleted()
        {
            _includeDeleted = !_includeDeleted;
            ReloadData();
            await Task.CompletedTask;
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
        protected void RowClickEvent(TableRowClickEventArgs<FeatureDto> tableRowClickEventArgs)
        {
            Dispatcher.Dispatch(new FeatureSelectedAction(tableRowClickEventArgs.Item));
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
                ["Title"] = $"{Translate(SystemTextId.Add)}"
            };
            var dialog = await DialogService.ShowEx<AddFeaturesDialog>($"{Translate(SystemTextId.Add)}", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                if (result.Data is AddFeatureRequest model)
                {
                    var added = await FeatureService.AddFeature(model);
                }
            }
        }
        private async Task ToggleEdit(FeatureDto Feature)
        {
            if (Feature.Deleted) return;

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

            var data = new FeatureDto
            {
                Id = Feature.Id,
                Deleted = Feature.Deleted,
                Text = Feature.Text,
                Description = Feature.Description,
            };
            var parameters = new DialogParameters
            {
                ["Feature"] = data
            };
            var dialog = await DialogService.ShowEx<EditFeaturesDialog>($"Edit", parameters, options);
            var result = await dialog.Result;
        }
        private string GetLabel(Guid labelId, string label)
        {
            return Translate(labelId, label);
        }
        protected string SelectedRowClassFunc(FeatureDto element, int rowNumber)
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