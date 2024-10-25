using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShareRazorClassLibrary.Configurations;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Shared
{
    public partial class LanguageSelection : BaseComponent
    {
        [Inject] private AppSettings _appSettings { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        public List<ShareRazorClassLibrary.Models.Dtos.ActiveLanguageDto> ActiveLanguages { get; set; }
        public ActiveLanguageDto ActiveLanguage { get; set; }
        private Dictionary<string, string> _cultures;
        [Inject] FirsloadState FirsloadState { get; set; }
        private string _dropArrowPosition = Icons.Material.Filled.KeyboardArrowDown;
        MudMenu _mudMenuRef = new();

        public string Language = "";
        private void IsOpen()
        {
            if (_mudMenuRef.IsOpen)
            {
                _dropArrowPosition = Icons.Material.Filled.KeyboardArrowUp;
            }
            else
            {
                _dropArrowPosition = Icons.Material.Filled.KeyboardArrowDown;
            }
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            FirsloadState.OnChange += FirsLoadChanged;
            _cultures = _appSettings.Languages.ToDictionary(x => x.Culture, x => x.LongName);

            base.OnInitialized();
        }
        protected override void Dispose(bool disposed)
        {
            FirsloadState.OnChange -= FirsLoadChanged;
            base.Dispose(disposed);
        }

        private void FirsLoadChanged()
        {
            StateHasChanged();
        }
        protected override async Task OnAfterRenderAsync(bool isFirstLoaded)
        {
            if (isFirstLoaded)
            {
                ActiveLanguages = await _translationService.GetActiveLanguages();
                ActiveLanguage = await _translationService.GetActiveLanguage();

                Language = GetShortName(ActiveLanguage);
            }
            StateHasChanged();
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
            StateHasChanged();
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