using Microsoft.AspNetCore.Components;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Services;

namespace TozawaNGO.Shared
{
    public partial class BaseComponent : ComponentBase, IDisposable
    {
        [Inject] protected ITranslationService _translationService { get; set; }
        public bool IsFirstLoaded { get; set; }
        [Inject] public ICurrentUserService _currentUserService { get; set; }

        public CurrentUserDto _currentUser { get; set; } = new();

        public BaseComponent()
        {

        }
        protected override void OnInitialized()
        {
            IsFirstLoaded = false;
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            base.OnInitialized();
        }

        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                IsFirstLoaded = true;
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
        }
    }
}