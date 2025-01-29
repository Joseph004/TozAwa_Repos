using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.FormModels;
using ShareRazorClassLibrary.Services;
using TozawaNGO.Helpers;
using TozawaNGO.Shared;

namespace TozawaNGO.Pages.Register
{
    public partial class RegisterMember : BasePage
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ICurrentUserService CurrentUserService { get; set; }
        [Inject] AuthenticationService AuthenticationService { get; set; }
        [Inject] ILogger<RegisterMember> Logger { get; set; }
        [Inject] NavMenuTabState NavMenuTabState { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] NavigationManager NavManager { get; set; }
        private bool _processing = false;
        private bool _currentErrorView = false;
        private bool _success;
        private string[] _errors = [];
        private string _searchCountry = "";
        private string _searchCity = "";
        private City _selectedCity = new();
        private Country _selectedCountry = new();
        private readonly MudButton RegisterButton;
        MudForm form;
        MudAutocomplete<Country> _countryAutocompleteRef = new();
        MudAutocomplete<City> _cityAutocompleteRef = new();
        private Func<Country, string> _convertedCountry => c => c != null && c.Code != null && c.Name != null ? $"{c.Name}" : "";
        private readonly Func<City, string> _convertedCity = c => c?.Name;
        public Color SelectedColor { get; set; } = Color.Default;
        public string Country = "Austra";

        protected async override Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            var auth = await _authStateProvider.GetAuthenticationStateAsync();
            if (auth.User.Identity != null && auth.User.Identity.IsAuthenticated)
            {
                var previousTab = NavMenuTabState.ActiveTab;
                NavManager.NavigateTo(NavMenuTabState.GetTabPath(previousTab));
            }
            NavMenuTabState.SetActiveTab(ShareRazorClassLibrary.Services.ActiveTab.Register);
            await base.OnInitializedAsync();
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            InvokeAsync(() =>
          {
              if (form != null && !form.IsValid && form.Errors.Length > 0)
              {
                  form?.Validate();
              }
              StateHasChanged();
          });
        }
        private async Task<IEnumerable<string>> ValidateCountry(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ["The country field is required"];
            }
            var countries = await _countryCityService.GetCountryByCode(value);
            if (countries.Count == 0)
            {
                {
                    return ["This is an incorrect value"];
                }
            }
            return [];
        }
        private async Task<IEnumerable<string>> ValidateCity(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ["The city field is required"];
            }
            var cities = await _countryCityService.GetCityByCountry(_selectedCountry.Code, value);
            if (cities.Count == 0)
            {
                {
                    return ["This is an incorrect value"];
                }
            }
            return [];
        }
        private async Task<IEnumerable<Country>> SearchCountries(string value, CancellationToken token)
        {
            _searchCountry = value;
            var response = string.IsNullOrEmpty(_searchCountry) ?
            await _countryCityService.GetCountryByCode() :
            await _countryCityService.GetCountryByCode(_searchCountry);
            _selectedCity = new();
            return response.DistinctBy(x => x.Code);
        }
        private async Task<IEnumerable<City>> SearchCities(string value, CancellationToken token)
        {
            _searchCity = value;
            var response = string.IsNullOrEmpty(_searchCity) ?
            await _countryCityService.GetCityByCountry(_selectedCountry.Code) :
            await _countryCityService.GetCityByCountry(_selectedCountry.Code, _searchCity);
            return response;
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("DisabledCopyPasteToPasswordField", Translate(SystemTextId.WriteYourPassword, "You need to write your password. No copy paste is allowed"));
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        private RegisterCommandFluentValidator RegisterValidator()
        {
            return new RegisterCommandFluentValidator(_translationService, AuthenticationService);
        }
        RegisterCommand model = new();
        private async Task RegisterByClick(MudButton element, MouseEventArgs e)
        {
            await Register();
        }
        private async Task RegisterByKeyBoard(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await Register();
            }
        }
        private bool DisabledRegisterButton()
        {
            return !_success || _processing;
        }
        private async Task Register()
        {
            if (!_success) return;

            LoadingState.SetRequestInProgress(true);

            _errors = [];
            _processing = true;
            StateHasChanged();

            model.Content = EncryptDecrypt.Encrypt(EncryptDecrypt.EncryptUsingCertificate(model.Password), "Uj=?1PowK<ai57:t%`Ro]P1~1q2&-i?b", "Rh2nvSARdZRDeYiB");
            model.Password = string.Empty;
            var userRegisterResponse = await AuthenticationService.PostRegisterMember(model);

            if (!userRegisterResponse.Success)
            {
                Snackbar.Add(Translate(SystemTextId.ErrorOccursPleaseContactSupport, "Error, contact support if this still happens."), Severity.Error);
                LoadingState.SetRequestInProgress(false);
                _processing = false;
                StateHasChanged();
            }
            else
            {
                var entity = userRegisterResponse.Entity ?? new RegisterResponseDto();
                if (!entity.RegisterSuccess)
                {
                    if (entity.ErrorMessageGuid.HasValue && entity.ErrorMessageGuid.Value == SystemTextId.EmailOrPasswordWrong)
                    {
                        var errorMessage = Translate(entity.ErrorMessageGuid.Value, entity.ErrorMessage);
                        _errors = [.. _errors, Translate(SystemTextId.EmailOrPasswordWrong, "Email or password wrong")];
                    }
                    else if (entity.ErrorMessageGuid.HasValue && entity.ErrorMessageGuid.Value == SystemTextId.UserNamelOrPasswordWrong)
                    {
                        var errorMessage = Translate(entity.ErrorMessageGuid.Value, entity.ErrorMessage);
                        _errors = [.. _errors, Translate(SystemTextId.UserNamelOrPasswordWrong, "User name or password wrong")];
                    }
                    else
                    {
                        if (entity.ErrorMessageGuid.HasValue)
                        {
                            var errorMessage = Translate(entity.ErrorMessageGuid.Value, entity.ErrorMessage);
                            _errors = _errors.Append(Translate(entity.ErrorMessageGuid.Value, "Error")).ToArray();
                        }
                    }

                    StateHasChanged();
                    await form.Validate();
                    Snackbar.Add(entity.ErrorMessageGuid.HasValue ? Translate(entity.ErrorMessageGuid.Value, entity.ErrorMessage) : entity.ErrorMessage, Severity.Error);
                    LoadingState.SetRequestInProgress(false);
                    _processing = false;
                    StateHasChanged();
                }
                else
                {
                    //Confirm(entity);
                }
            }
            LoadingState.SetRequestInProgress(false);
            _processing = false;
            StateHasChanged();
        }
        protected override void Dispose(bool disposed)
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            base.Dispose(disposed);
        }
    }
}

