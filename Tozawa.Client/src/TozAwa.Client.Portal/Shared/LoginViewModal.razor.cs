using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using Tozawa.Client.Portal.HttpClients.Helpers;
using Tozawa.Client.Portal.Models.Dtos;
using Tozawa.Client.Portal.Models.FormModels;
using Tozawa.Client.Portal.Services;
using TozAwa.Client.Portal.Services;

namespace Tozawa.Client.Portal.Shared
{
    public partial class LoginViewModal : BaseDialog
    {
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
        [Inject] IDataProtectionProviderService _dataProtectionProviderService { get; set; }
        private bool _processing = false;
        private bool _currentErrorView = false;
        private bool _success;
        private string[] _errors = Array.Empty<string>();
        private bool _loginAsRoot = false;
        private int _attemptLoginCount = 0;
        private MudButton loginButton;
        private string ToggleAdminIcon = Icons.Material.Filled.ToggleOff;
        private bool isShow;
        MudForm form;
        string _message = string.Empty;
        bool _is_server_authenticated = false;
        private List<Action> actionsToRunAfterRender = new List<Action>();

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("DisabledCopyPasteToPasswordField", Translate(SystemTextId.WriteYourPassword, "You need to write your password. No copy paste is allowed"));
                await JSRuntime.InvokeVoidAsync("AddTitleToInnerIconLoginAsAdmin", Translate(SystemTextId.LoginAsAdmin, "Login as administrator"));
                await JSRuntime.InvokeVoidAsync("AddTitleToInnerIconLoginAsNotAdmin", Translate(SystemTextId.NotLoginAsAdmin, "Not a administrator"));
            }
            foreach (var actionToRun in actionsToRunAfterRender)
            {
                actionToRun();
            }
            actionsToRunAfterRender.Clear();
            await base.OnAfterRenderAsync(firstRender);
        }
        void ButtonAdminclick()
        {
            if (isShow)
            {
                isShow = false;
                ToggleAdminIcon = Icons.Material.Filled.ToggleOff;
                model.LoginAsRoot = false;
            }
            else
            {
                isShow = true;
                ToggleAdminIcon = Icons.Material.Filled.ToggleOn;
                model.LoginAsRoot = true;
            }
            StateHasChanged();
            actionsToRunAfterRender.Add(async () => await ActionsToRunAfterRender());
        }
        private LoginCommandFluentValidator LoginValidator()
        {
            return new LoginCommandFluentValidator(_translationService, AuthenticationService);
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

                _errors = Array.Empty<string>();
                _processing = true;
                _currentErrorView = model.LoginAsRoot;
                StateHasChanged();

                var request = new LoginRequest
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    LoginAsRoot = model.LoginAsRoot,
                    Content = await _dataProtectionProviderService.EncryptAsync(model.Password)
                };
                var userLoginResponse = await AuthenticationService.PostLogin(request);

                if (!userLoginResponse.Success)
                {
                    Snackbar.Add(Translate(SystemTextId.Error, "Error"), Severity.Error);
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
                            _errors = _errors.Append(Translate(SystemTextId.TemporarlyLockout, "You've been temporarely lockedout, please contact a technician!")).ToArray();
                        }

                        if (entity.ErrorMessageGuid.HasValue && entity.ErrorMessageGuid.Value == SystemTextId.EmailOrPasswordWrong)
                        {
                            var errorMessage = Translate(entity.ErrorMessageGuid.Value, entity.ErrorMessage);
                            _errors = _errors.Append(Translate(SystemTextId.EmailOrPasswordWrong, "Email or password wrong")).ToArray();
                        }
                        else if (entity.ErrorMessageGuid.HasValue && entity.ErrorMessageGuid.Value == SystemTextId.UserNamelOrPasswordWrong)
                        {
                            var errorMessage = Translate(entity.ErrorMessageGuid.Value, entity.ErrorMessage);
                            _errors = _errors.Append(Translate(SystemTextId.UserNamelOrPasswordWrong, "User name or password wrong")).ToArray();
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
                        _processing = false;
                        StateHasChanged();
                    }
                    else
                    {
                        _attemptLoginCount = 0;
                        await CurrentUserService.SetCurrentUser(entity.User);
                        Confirm(entity.User);
                    }
                }

                _processing = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ErrorHandling?.ProcessError(ex, Translate(SystemTextId.LoginError, "Login error"), Translate(SystemTextId.ErrorOccursWhenLogIn, "Error occurs when login"));

                _processing = false;
                StateHasChanged();
            }
        }
        private void Cancel()
        {
            MudDialog.Cancel();
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

