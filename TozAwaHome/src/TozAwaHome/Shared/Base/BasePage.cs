using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TozAwaHome.Models.Dtos;
using TozAwaHome.Services;

namespace TozAwaHome.Shared
{
    public partial class BasePage : ComponentBase, IDisposable
    {
        [Inject] protected ITranslationService _translationService { get; set; }
        [Inject] private ICurrentUserService _currentUserService { get; set; }

        public CurrentUserDto _currentUser { get; set; } = new();
        public List<ActiveLanguageDto> ActiveLanguages { get; set; } = new();

        public BasePage()
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

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _translationService.EnsureTranslations();

                StateHasChanged();
            }
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            _currentUser = await _currentUserService.GetCurrentUser();
            ActiveLanguages = (await _translationService.GetActiveLanguages()).OrderBy(x => x.LongName).ToList();
            await base.SetParametersAsync(ParameterView.Empty);
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

        public virtual void Dispose()
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
        }
    }
}