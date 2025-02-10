using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.Enums;
using TozawaMauiHybrid.Services;
namespace TozawaMauiHybrid.Component
{
    public partial class BaseComponent<T> : Fluxor.Blazor.Web.Components.FluxorComponent, IDisposable
    {
        [Inject] ILogger<T> _logger { get; set; }
        [Inject] protected ITranslationService _translationService { get; set; }
        [Inject] protected AuthStateProvider _authStateProvider { get; set; }
        [Inject] public ICurrentUserService _currentUserService { get; set; }

        public CurrentUserDto _currentUser { get; set; } = new();

        public BaseComponent()
        {

        }
        protected override void OnInitialized()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;
            base.OnInitialized();
        }
        private void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            InvokeAsync(() =>
          {
              StateHasChanged();
          });
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            InvokeAsync(() =>
          {
              StateHasChanged();
          });
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _currentUser = await _currentUserService.GetCurrentUser();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);
            await base.SetParametersAsync(ParameterView.Empty);
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
        protected override void Dispose(bool disposed)
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
            base.Dispose(disposed);
        }
    }
}