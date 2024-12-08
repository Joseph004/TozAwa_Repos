using Microsoft.AspNetCore.Components;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Shared
{
    public partial class BasePage : Fluxor.Blazor.Web.Components.FluxorComponent, IDisposable
    {
        [Inject] protected ITranslationService _translationService { get; set; }
        [Inject] protected AuthStateProvider _authStateProvider { get; set; }
        [Inject] private ICurrentUserService _currentUserService { get; set; }

        public CurrentUserDto _currentUser { get; set; } = new();
        public List<ActiveLanguageDto> ActiveLanguages { get; set; } = [];

        public BasePage()
        {

        }

        protected override void OnInitialized()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;
            base.OnInitialized();
        }

        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            InvokeAsync(() =>
           {
               StateHasChanged();
           });
        }
        private void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            InvokeAsync(() =>
           {
               StateHasChanged();
           });
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            //await _translationService.EnsureTranslations();
            _currentUser = await _currentUserService.GetCurrentUser();
            ActiveLanguages = (await _translationService.GetActiveLanguages()).OrderBy(x => x.LongName).ToList();
            await base.SetParametersAsync(ParameterView.Empty);
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
        protected override void Dispose(bool disposed)
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
            base.Dispose(disposed);
        }
    }
}