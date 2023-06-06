using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Tozawa.Client.Portal.Models.Dtos;
using TozAwa.Client.Portal.Services;

namespace Tozawa.Client.Portal.Shared
{
    public partial class LanguageSelection : BaseComponent
    {
        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        public List<ActiveLanguageDto> ActiveLanguages { get; set; }
        public ActiveLanguageDto ActiveLanguage { get; set; }

        public string Language = "";

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                LoadingState.SetRequestInProgress(true);

                ActiveLanguages = await _translationService.GetActiveLanguages();
                ActiveLanguage = await _translationService.GetActiveLanguage();

                Language = GetShortName(ActiveLanguage);
                LoadingState.SetRequestInProgress(false);
                StateHasChanged();
            }
        }
        public async Task ChangeActiveLanguage(Guid languageId)
        {
            try
            {
                LoadingState.SetRequestInProgress(true);
                await _translationService.ChangeActiveLanguage(languageId);
                ActiveLanguage = await _translationService.GetActiveLanguage();
                Language = GetShortName(ActiveLanguage);
            }
            catch (Exception ex)
            {
                LoadingState.SetRequestInProgress(false);
            }

            LoadingState.SetRequestInProgress(false);
            StateHasChanged();
        }

        public string GetShortName(ActiveLanguageDto activeLanguage)
        {
            return _translationService.GetShortName(activeLanguage);
        }

        public string GetTitle(ActiveLanguageDto activeLanguage)
        {
            return activeLanguage == null ? "English" : GetShortName(activeLanguage).Equals("gb", StringComparison.CurrentCultureIgnoreCase) ? "English" :
                   GetShortName(activeLanguage).Equals("fr", StringComparison.CurrentCultureIgnoreCase) ? "Francais" : activeLanguage.LongName;
        }
    }
}