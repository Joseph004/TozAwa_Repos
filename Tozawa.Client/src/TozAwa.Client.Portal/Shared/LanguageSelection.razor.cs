using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Tozawa.Client.Portal.Models.Dtos;

namespace Tozawa.Client.Portal.Shared
{
    public partial class LanguageSelection : BaseComponent
    {
        [Inject] private NavigationManager _navigationManager { get; set; }

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
                ActiveLanguages = await _translationService.GetActiveLanguages();
                ActiveLanguage = await _translationService.GetActiveLanguage();

                Language = GetShortName(ActiveLanguage);
                StateHasChanged();
            }
        }
        public async Task ChangeActiveLanguage(Guid languageId)
        {
            await _translationService.ChangeActiveLanguage(languageId);
            ActiveLanguage = await _translationService.GetActiveLanguage();
            Language = GetShortName(ActiveLanguage);
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