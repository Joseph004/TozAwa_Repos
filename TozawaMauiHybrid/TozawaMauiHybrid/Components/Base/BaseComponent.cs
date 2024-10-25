using Microsoft.AspNetCore.Components;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Services;
namespace TozawaMauiHybrid.Component
{
    public partial class BaseComponent : ComponentBase, IDisposable
    {
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
            StateHasChanged();
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            StateHasChanged();
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
        public bool HasAtLeastOneRole(params string[] roles)
        {
            return _currentUser.Roles.Any(r => roles.Any(x => GetRole(x) == r)) || _currentUser.Admin;
        }
        public bool HasAllRolesMatching(params string[] roles)
        {
            return _currentUser.Roles.All(r => roles.All(x => GetRole(x) == r)) || _currentUser.Admin;
        }
        private static RoleDto GetRole(string role)
        {
            Enum.TryParse(role, out RoleDto myRole);

            return myRole;
        }
        public virtual void Dispose()
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
        }
    }
}