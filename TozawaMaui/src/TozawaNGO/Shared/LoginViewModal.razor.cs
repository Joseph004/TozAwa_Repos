using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.FormModels;
using ShareRazorClassLibrary.Services;
using TozawaNGO.Helpers;

namespace TozawaNGO.Shared
{
    public partial class LoginViewModal : BaseDialog
    {
#pragma warning disable CS0649
        [CascadingParameter]
        public ErrorHandling ErrorHandling { get; set; } = null;
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public string Title { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ICurrentUserService CurrentUserService { get; set; }
        [Inject] AuthenticationService AuthenticationService { get; set; }
        [Inject] ILogger<LoginViewModal> Logger { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        private bool _processing = false;
        private bool _currentErrorView = false;
        private bool _success;
        private string[] _errors = [];
        private readonly int _attemptLoginCount = 0;
        private readonly MudButton loginButton;
        MudForm form;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("DisabledCopyPasteToPasswordField", Translate(SystemTextId.WriteYourPassword, "You need to write your password. No copy paste is allowed"));
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private LoginCommandFluentValidator LoginValidator()
        {
            return new LoginCommandFluentValidator(_translationService, AuthenticationService);
        }

        LoginCommand model = new();

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
            return !_success || _processing || _attemptLoginCount == 3;
        }
        private async Task ActionsToRunAfterRender()
        {
            await JSRuntime.InvokeVoidAsync("AddTitleToInnerIconLoginAsAdmin", Translate(SystemTextId.LoginAsAdmin, "Login as administrator"));
            await JSRuntime.InvokeVoidAsync("AddTitleToInnerIconLoginAsNotAdmin", Translate(SystemTextId.NotLoginAsAdmin, "Not a administrator"));

            if (model.LoginAsRoot && !string.IsNullOrEmpty(model.UserName))
            {
                if (!string.IsNullOrEmpty(model.Password))
                {
                    await form.Validate();
                    StateHasChanged();
                }
            }
            else if (!model.LoginAsRoot && !string.IsNullOrEmpty(model.Email))
            {
                if (!string.IsNullOrEmpty(model.Password))
                {
                    await form.Validate();
                    StateHasChanged();
                }
            }
        }
        private async Task Login()
        {
            try
            {
                if (!_success) return;

                LoadingState.SetRequestInProgress(true);

                _errors = [];
                _processing = true;
                _currentErrorView = model.LoginAsRoot;
                StateHasChanged();

                var request = new LoginRequest
                {
                    Email = model.Email,
                    Content = EncryptDecrypt.Encrypt(EncryptDecrypt.EncryptUsingCertificate(model.Password), "Uj=?1PowK<ai57:t%`Ro]P1~1q2&-i?b", "Rh2nvSARdZRDeYiB")
                };

                var userLoginResponse = await AuthenticationService.PostLogin(request);

                if (!userLoginResponse.Success)
                {
                    Snackbar.Add(Translate(SystemTextId.ErrorOccursPleaseContactSupport, "Error, contact support if this still happens."), Severity.Error);
                    LoadingState.SetRequestInProgress(false);
                    _processing = false;
                    StateHasChanged();
                }
                else
                {
                    var entity = userLoginResponse.Entity ?? new LoginResponseDto();
                    if (!entity.LoginSuccess)
                    {
                        if (entity.LoginAttemptCount == 3)
                        {
                            _errors = [.. _errors, Translate(SystemTextId.TemporarlyLockout, "You've been temporarely lockedout, please contact a technician!")];
                        }

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
                        Confirm(entity);
                    }
                }

                _processing = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                LoadingState.SetRequestInProgress(false);
                ErrorHandling?.ProcessError(ex, Translate(SystemTextId.LoginError, "Login error"), Translate(SystemTextId.ErrorOccursWhenLogIn, "Error occurs when login"));

                _processing = false;
                StateHasChanged();
            }
            LoadingState.SetRequestInProgress(false);
            StateHasChanged();
        }
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private void Confirm(LoginResponseDto loginResponse)
        {
            InvokeAsync(async () =>
          {
              MudDialog.Close(DialogResult.Ok(loginResponse));
              await Task.CompletedTask;
          });
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public override void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
        }
    }
}

