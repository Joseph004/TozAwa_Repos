using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Component
{
    public partial class BaseDialog<T> : ComponentBase, IDisposable
    {
        [Inject] ILogger<T> _logger { get; set; }
        [Inject] protected ITranslationService _translationService { get; set; }
        [Inject] private ICurrentUserService _currentUserService { get; set; }

        public CurrentUserDto _currentUser { get; set; } = new();

        public BaseDialog()
        {

        }
        protected override void OnInitialized()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            base.OnInitialized();
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            InvokeAsync(() =>
          {
              StateHasChanged();
          });
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);
            await _translationService.EnsureTranslations();
            _currentUser = await _currentUserService.GetCurrentUser();
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