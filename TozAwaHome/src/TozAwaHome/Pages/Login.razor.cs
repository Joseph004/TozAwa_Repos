using Microsoft.AspNetCore.Components;
using MudBlazor;
using TozAwaHome.HttpClients.Helpers;
using TozAwaHome.Models.Dtos;
using TozAwaHome.Models.FormModels;
using TozAwaHome.Services;
using TozAwaHome.Shared;

namespace TozAwaHome.Pages
{
    public partial class Login : BasePage
    {
        [CascadingParameter]
        public ErrorHandling ErrorHandling { get; set; } = null;
        private string[] _errors = Array.Empty<string>();
        MudForm form;
        private bool _success;
        private bool _processing = false;
        [Inject] AuthenticationService AuthenticationService { get; set; }
        [Inject] IDataProtectionProviderService _dataProtectionProviderService { get; set; }
        [Inject] ICurrentUserService CurrentUserService { get; set; }
        [Inject] NavigationManager NavManager {get;set;}

        protected async override Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;

            await base.OnInitializedAsync();
        }

        private LoginCommandFluentValidator LoginValidator()
        {
            return new LoginCommandFluentValidator(_translationService, AuthenticationService);
        }

        LoginCommand model = new LoginCommand();

        //private string _spinnerClass = "";

        private bool DisabledLoginButton()
        {
            return !_success || _processing;
        }
        private async void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            if(!_success)
            {
              await form.Validate();
            }

            StateHasChanged();
        }
        private async Task Submit()
        {
            try
            {
				if (!_success) return;
				_errors = Array.Empty<string>();
                _processing = true;
                //_spinnerClass = "spinner-border spinner-border-sm";

                var request = new LoginRequest
                {
                    Email = model.Email,
                    Content = await _dataProtectionProviderService.EncryptAsync(model.Password)
                };
                var userLoginResponse = await AuthenticationService.PostLogin(request);

                if (!userLoginResponse.Success)
                {
                    await App.Current.MainPage.DisplayAlert("Oops", Translate(SystemTextId.Error, "Error"), "OK");
                    _processing = false;
                    //_spinnerClass = "";
                    this.StateHasChanged();
                }
                else
                {
                    var entity = userLoginResponse.Entity ?? new LoginResponseDto();
                    if (!entity.LoginSuccess)
                    {
                        if (entity.ErrorMessageGuid.HasValue && entity.ErrorMessageGuid.Value == SystemTextId.EmailOrPasswordWrong)
                        {
                            var errorMessage = Translate(entity.ErrorMessageGuid.Value, entity.ErrorMessage);
                            _errors = _errors.Append(Translate(SystemTextId.EmailOrPasswordWrong, "Email or password wrong")).ToArray();
                        }
                        else
                        {
                            if (entity.ErrorMessageGuid.HasValue)
                            {
                                var errorMessage = Translate(entity.ErrorMessageGuid.Value, entity.ErrorMessage);
                                _errors = _errors.Append(Translate(entity.ErrorMessageGuid.Value, "Error")).ToArray();
                            }
                        }

                        this.StateHasChanged();
                        await form.Validate();
                        var errMessage = entity.ErrorMessageGuid.HasValue ? Translate(entity.ErrorMessageGuid.Value, entity.ErrorMessage) : entity.ErrorMessage;
                        await App.Current.MainPage.DisplayAlert("Oops", errMessage, "OK");
                        _processing = false;
                        //_spinnerClass = "";
                        this.StateHasChanged();
                    }
                    else
                    {
                        /*var tokenResponse = JsonConvert.DeserializeObject<AuthenticateRequestAndResponse>(response.Content.ToString());

                        var handler = new JwtSecurityTokenHandler();
                        var jsontoken = handler.ReadToken(tokenResponse.AccessToken) as JwtSecurityToken;

                        string userID = jsontoken.Claims.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.NameId).Value;
                        string name = jsontoken.Claims.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.Name).Value;
                        string userAvatar = jsontoken.Claims.FirstOrDefault(f => f.Type == "UserAvatar").Value;

                        string email = loginModel.UserName;

                        var userBasicDetail = new UserBasicDetail
                        {
                            Email = email,
                            Name = name,
                            AccessToken = tokenResponse.AccessToken,
                            RefreshToken = tokenResponse.RefreshToken,
                            UserAvatar = !string.IsNullOrWhiteSpace(userAvatar) ? $"{Setting.BaseUrl}/{userAvatar}" : "",
                            UserID = userID
                        };*/

                        await CurrentUserService.SetCurrentUser(entity.User);

                        NavManager.NavigateTo("/dashboard");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling?.ProcessError(ex, Translate(SystemTextId.LoginError, "Login error"), Translate(SystemTextId.ErrorOccursWhenLogIn, "Error occurs when login"));

                _processing = false;
                //_spinnerClass = "";
                this.StateHasChanged();
            } 
        }

        public override void Dispose()
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
        }
    }
}
