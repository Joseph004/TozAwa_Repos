using System.Timers;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShareRazorClassLibrary.Services;
using TozawaNGO.Shared;
using TozawaNGO.State.Home.Store;
using TozawaNGO.StateHandler;

namespace TozawaNGO.Pages
{
    public partial class Home : BasePage
    {
        [Inject] IState<HomeState> HomeState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] ScrollTopState ScrollTopState { get; set; }
        [Inject] NavMenuTabState NavMenuTabState { get; set; }
        private string _image = "family_moving_into_new_house.jpg";
        private List<string> _images = [
            "Leonardo_Phoenix_A_joyful_child_with_bright_curious_eyes_and_a_3.jpg",
            "Leonardo_Phoenix_Vibrant_scene_of_a_joyful_black_child_with_br_3.jpg",
            "Leonardo_Phoenix_a_highly_detailed_digital_illustration_with_a_2.jpg",
            "Leonardo_Phoenix_A_cozy_vacation_house_rental_nestled_among_lu_0.jpg",
            "family_moving_into_new_house.jpg"
        ];
        private System.Timers.Timer _timer;
        private readonly int _timerInterval = 2000;
        protected override void Dispose(bool disposed)
        {
            try
            {
                ScrollTopState.OnChange -= SetScroll;
            }
            catch
            {
            }
            base.Dispose(disposed);
        }
        protected override async Task OnInitializedAsync()
        {
            NavMenuTabState.SetActiveTab(ShareRazorClassLibrary.Services.ActiveTab.Home);
            ScrollTopState.SetSource("homePage");
            ScrollTopState.OnChange += SetScroll;
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (HomeState.Value.ScrollTop != 0)
                {
                    await JSRuntime.InvokeAsync<object>("SetScroll", (-1) * HomeState.Value.ScrollTop);
                }
                _timer = new System.Timers.Timer(_timerInterval);
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleImages);
                _timer.AutoReset = false;
                _timer.Start();
                await base.OnAfterRenderAsync(firstRender);
            }
        }
        private int count = 0;
        private void HandleImages(Object source, ElapsedEventArgs e)
        {
            _image = _images.ElementAtOrDefault(count);
            if (string.IsNullOrEmpty(_image))
            {
                _image = "family_moving_into_new_house.jpg";
                count = 0;
            }
            else
            {
                count++;
            }
            InvokeAsync(() =>
          {
              StateHasChanged();
              _timer.Interval = _timerInterval;
          });
        }
        private void SetScroll()
        {
            Dispatcher.Dispatch(new ScrollTopAction(ScrollTopState.ScrollTop[ScrollTopState.Source]));
        }
    }
}

