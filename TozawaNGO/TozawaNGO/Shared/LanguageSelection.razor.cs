using Microsoft.AspNetCore.Components;
using MudBlazor;
using TozawaNGO.Configurations;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Services;

namespace TozawaNGO.Shared
{
    public partial class LanguageSelection : BaseComponent
    {
        [Inject] private AppSettings _appSettings { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        public List<ActiveLanguageDto> ActiveLanguages { get; set; }
        public ActiveLanguageDto ActiveLanguage { get; set; }
        private Dictionary<string, string> _cultures;
        private string _dropArrowPosition = Icons.Material.Filled.KeyboardArrowDown;
        MudMenu _mudMenuRef = new();
        EventCallback<bool> _onMenuChange;
        private bool _isFirstLoaded { get; set; } = false;
        public EventCallback<bool> OnMenuChange
        {
            get => _onMenuChange;
            set
            {
                _onMenuChange = value;
                MudMenuRefchanged();
            }
        }

        public string Language = "";
        public void MudMenuRefchanged()
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
            OnMenuChange.InvokeAsync(_mudMenuRef.IsOpen);

            _cultures = _appSettings.Languages.ToDictionary(x => x.Culture, x => x.LongName);

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool isFirstLoaded)
        {
            if (isFirstLoaded)
            {
                _isFirstLoaded = true;
                LoadingState.SetRequestInProgress(true);

                ActiveLanguages = await _translationService.GetActiveLanguages();
                ActiveLanguage = await _translationService.GetActiveLanguage();

                Language = GetShortName(ActiveLanguage);
                LoadingState.SetRequestInProgress(false);
            }
            StateHasChanged();
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
                   GetShortName(activeLanguage).Equals("fr", StringComparison.CurrentCultureIgnoreCase) ? "Francais" :
                   GetShortName(activeLanguage).Equals("se", StringComparison.CurrentCultureIgnoreCase) ? "Svenska" : activeLanguage.LongName;
        }
    }
}