using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using Tozawa.Language.Client.Helpers;
using Tozawa.Language.Client.Models.Dtos;
using Tozawa.Language.Client.Models.DTOs;
using Tozawa.Language.Client.Models.FormModels;
using Tozawa.Language.Client.Services;

namespace Tozawa.Language.Client.Shared
{
    public partial class LoginViewModal
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public string Title { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ICurrentUserService CurrentUserService { get; set; }
        [Inject] IDataProtectionProviderService _dataProtectionProviderService { get; set; }
        [Inject] AuthenticationService AuthenticationService { get; set; }
        private bool _processing = false;
        private MudButton loginButton;
        private int _attemptLoginCount = 0;
        private bool _success;
        private string[] _errors = Array.Empty<string>();
        MudForm form;
        private LoginCommandFluentValidator LoginValidator()
        {
            return new LoginCommandFluentValidator(AuthenticationService, CancellationToken.None);
        }

        LoginCommand model = new LoginCommand();

        private async Task LoginByClick(MudButton element, MouseEventArgs e)
        {
            await Login();
        }
        private async Task LoginByKeyBoard(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await Login();
            }
        }
        private bool DisabledLoginButton()
        {
            return !_success || _processing;
        }
        private async Task Login()
        {
            if (!_success) return;

            _errors = Array.Empty<string>();
            _processing = true;
            StateHasChanged();

            var request = new LoginRequest
            {
                UserName = model.UserName,
                Content = await _dataProtectionProviderService.EncryptAsync(model.Password)
            };
            var userLoginResponse = await AuthenticationService.PostLogin(request);

            if (userLoginResponse == null)
            {
                Snackbar.Add(userLoginResponse.ErrorMessage, Severity.Error);
                _processing = false;
                StateHasChanged();
            }
            else
            {
                if (!userLoginResponse.LoginSuccess)
                {
                    if (userLoginResponse.LoginAttemptCount > 0)
                    {
                        _attemptLoginCount = 3 - userLoginResponse.LoginAttemptCount;
                        _errors = _errors.Append(userLoginResponse.ErrorMessage).ToArray();
                    }
                    if (userLoginResponse.LoginAttemptCount == 3)
                    {
                        _errors = _errors.Append("You've been temporarely lockedout, please contact a technician!").ToArray();
                        await form.Validate();
                    }

                    if (userLoginResponse.ErrorMessageGuid.HasValue && userLoginResponse.ErrorMessageGuid.Value == SystemTextId.UserNamelOrPasswordWrong)
                    {
                        var errorMessage = "User name or password wrong";
                        _errors = _errors.Append("User name or password wrong").ToArray();
                    }
                    else
                    {
                        if (userLoginResponse.ErrorMessageGuid.HasValue)
                        {
                            var errorMessage = "Error";
                            _errors = _errors.Append("Error").ToArray();
                        }
                    }

                    Snackbar.Add(userLoginResponse.ErrorMessage, Severity.Error);
                    _processing = false;
                    StateHasChanged();
                }
                else
                {
                    _attemptLoginCount = 0;
                    await CurrentUserService.SetCurrentUser(userLoginResponse.User);
                    Confirm(userLoginResponse.User);
                }
            }
            _processing = false;
            StateHasChanged();
        }

        private void Confirm(CurrentUserDto user)
        {
            InvokeAsync(async () =>
          {
              MudDialog.Close(DialogResult.Ok(user));
              await Task.CompletedTask;
          });
        }

        void IDisposable.Dispose()
        {
        }
    }
}

