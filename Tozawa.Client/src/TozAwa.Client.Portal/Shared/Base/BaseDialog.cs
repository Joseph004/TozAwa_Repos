using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Tozawa.Client.Portal.Models.Dtos;
using Tozawa.Client.Portal.Services;

namespace Tozawa.Client.Portal.Shared
{
    public partial class BaseDialog : ComponentBase, IDisposable
    {
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
            StateHasChanged();
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

        public bool HasAllFeaturesMatching(params int[] features)
        {
            return _currentUser.Features.All(f => features.Contains(f));
        }
        public bool HasAtLeastOneFunctionType(params string[] functionTypes)
        {
            return _currentUser.Functions.Any(f => functionTypes.Any(x => x.Equals(f.FunctionType, StringComparison.OrdinalIgnoreCase)));
        }
        public bool HasAllFunctionTypesMatching(params string[] functionTypes)
        {
            return _currentUser.Functions.All(f => functionTypes.All(x => x.Equals(f.FunctionType, StringComparison.OrdinalIgnoreCase)));
        }
        public bool HasAtLeastOneFeature(params int[] features)
        {
            return _currentUser.Features.Any(f => features.Contains(f));
        }

        public void Dispose()
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
        }
    }
}