using Microsoft.AspNetCore.Components;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.Enums;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Shared
{
    public partial class BaseComponentLayout : LayoutComponentBase, IDisposable
    {
        [Inject] protected ITranslationService _translationService { get; set; }
        [Inject] protected ICountryCityService _countryCityService { get; set; }
        [Inject] protected AuthStateProvider _authStateProvider { get; set; }
        [Inject] private ICurrentUserService _currentUserService { get; set; }

        public CurrentUserDto _currentUser { get; set; } = new();

        public BaseComponentLayout()
        {

        }

        protected override void OnInitialized()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;
            base.OnInitialized();
        }
        private async void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        private async void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            await InvokeAsync(() =>
             {
                 StateHasChanged();
             });
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);
            await base.SetParametersAsync(ParameterView.Empty);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _translationService.EnsureTranslations();
                _currentUser = await _currentUserService.GetCurrentUser();
                StateHasChanged();
            }
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        public string Translate(Guid systemTextId, string fallback = null, int? limit = null, bool? toUpper = null)
        {
            return _translationService.Translate(systemTextId, fallback, limit, toUpper).Text;
        }
        public bool HasAllFeaturesMatching(params int[] features)
        {
            return features.All(f => _currentUser.Features.Contains(f));
        }
        public bool HasAtLeastOneFunctionType(params FunctionType[] functionTypes)
        {
            return _currentUser.Functions.Any(f => functionTypes.Any(x => x.Equals(f.FunctionType)));
        }
        public bool HasAllFunctionTypesMatching(params FunctionType[] functionTypes)
        {
            return functionTypes.All(f => _currentUser.Functions.Any(x => x.FunctionType.Equals(f)));
        }
        public bool HasAtLeastOneFeature(params int[] features)
        {
            return _currentUser.Features.Any(f => features.Contains(f));
        }
        public virtual void Dispose()
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
        }
    }
}