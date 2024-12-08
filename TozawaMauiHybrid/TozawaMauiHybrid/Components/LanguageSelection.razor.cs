using Microsoft.AspNetCore.Components;
using MudBlazor;
using TozawaMauiHybrid.Configurations;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Component
{
    public partial class LanguageSelection : BaseComponent<LanguageSelection>
    {
        [Inject] private AppSettings _appSettings { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        public List<TozawaMauiHybrid.Models.Dtos.ActiveLanguageDto> ActiveLanguages { get; set; }
        public ActiveLanguageDto ActiveLanguage { get; set; }
        private Dictionary<string, string> _cultures;
        [Inject] FirstloadState FirstloadState { get; set; }
        private string _dropArrowPosition = Icons.Material.Filled.KeyboardArrowDown;
        MudMenu _mudMenuRef = new();
        protected override void Dispose(bool disposed)
        {
            FirstloadState.OnChange -= FirsLoadChanged;
            base.Dispose(disposed);
        }
        private void FirsLoadChanged()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        public string Language = "";
        private void IsOpen()
        {
            if (_mudMenuRef.Open)
            {
                _dropArrowPosition = Icons.Material.Filled.KeyboardArrowUp;
            }
            else
            {
                _dropArrowPosition = Icons.Material.Filled.KeyboardArrowDown;
            }
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected override void OnInitialized()
        {
            FirstloadState.OnChange += FirsLoadChanged;
            _cultures = _appSettings.Languages.ToDictionary(x => x.Culture, x => x.LongName);

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool isFirstLoaded)
        {
            if (isFirstLoaded)
            {
                ActiveLanguages = await _translationService.GetActiveLanguages();
                ActiveLanguage = await _translationService.GetActiveLanguage();

                Language = GetShortName(ActiveLanguage);
                await InvokeAsync(() =>
         {
             StateHasChanged();
         });
            }
        }

        public async Task ChangeActiveLanguage(Guid languageId)
        {
            try
            {
                await _translationService.ChangeActiveLanguage(languageId);
                ActiveLanguage = await _translationService.GetActiveLanguage();
                Language = GetShortName(ActiveLanguage);
            }
            catch (Exception)
            {
                Snackbar.Add("Could not change language", Severity.Error);
            }
            await InvokeAsync(() =>
         {
             StateHasChanged();
         });
        }

        public string GetShortName(ActiveLanguageDto activeLanguage)
        {
            return _translationService.GetShortName(activeLanguage);
        }

        public string GetTitle(ActiveLanguageDto activeLanguage)
        {
            return activeLanguage == null ? "English" : GetShortName(activeLanguage).Equals("gb", StringComparison.CurrentCultureIgnoreCase) ? "English" :
                   GetShortName(activeLanguage).Equals("fr", StringComparison.CurrentCultureIgnoreCase) ? "Francais" :
                   GetShortName(activeLanguage).Equals("se", StringComparison.CurrentCultureIgnoreCase) ? "Svenska" : activeLanguage.LongName;
        }
    }
}