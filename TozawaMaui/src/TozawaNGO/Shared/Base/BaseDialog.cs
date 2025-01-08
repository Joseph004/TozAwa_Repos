using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Nextended.Core.Extensions;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.Enums;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Shared
{
    public partial class BaseDialog<T> : ComponentBase, IDisposable
    {
        [Inject] ILogger<T> _logger { get; set; }
        [Inject] IDialogService DialogService { get; set; }
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
        }
    }
}