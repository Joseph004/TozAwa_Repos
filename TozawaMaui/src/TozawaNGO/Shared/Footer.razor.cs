using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Shared
{
    public partial class Footer : BaseComponent
    {
        [Inject] IJSRuntime JS { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        public string _email { get; set; }
        SubscribeCommand model = new();
        MudForm form;
        private bool _success;
        private string[] _errors = [];
        bool requestInProgress;
        string EmailInputIcon = Icons.Material.Filled.Send;

        private SubscribeCommandFluentValidator SubscribeValidator()
        {
            return new SubscribeCommandFluentValidator(_translationService);
        }
        public string GetHelperText()
        {
            if (_errors.Length != 0)
            {
                return _errors[0];
            }
            return "";
        }
        public async Task SendEmailClick()
        {
            _errors = [];
            if (requestInProgress) return;
            var validator = SubscribeValidator();

            var requestValidate = await validator.ValidateAsync(model);

            if (string.IsNullOrEmpty(model.Email) || !requestValidate.IsValid)
            {
                _errors = [.. _errors, requestValidate.Errors.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).FirstOrDefault().ErrorMessage];

                StateHasChanged();
                return;
            }

            requestInProgress = true;
            EmailInputIcon = Icons.Material.Filled.ChangeCircle;
            StateHasChanged();

            //Sending request...
            Thread.Sleep(15000);

            requestInProgress = false;
            model.Email = "";
            EmailInputIcon = Icons.Material.Filled.Send;
            Snackbar.Add("Thanks for your subscribtion!", MudBlazor.Severity.Success);
            StateHasChanged();
        }
        public bool DisabledSend()
        {
            return requestInProgress;
        }
        protected async override Task OnInitializedAsync()
        {
            FirstloadState.OnChange += FirsLoadChanged;
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            _email = _translationService.Translate(SystemTextId.Email, "Email").Text;
            _errors = [];

            await base.OnInitializedAsync();
        }
        private void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
        private async void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            _errors = [];
            var validator = SubscribeValidator();
            var requestValidate = await validator.ValidateAsync(model);
            if (!requestValidate.IsValid)
            {
                _errors = [.. _errors, requestValidate.Errors.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).FirstOrDefault().ErrorMessage];
            }
            _email = _translationService.Translate(SystemTextId.Email, "Email").Text;

            StateHasChanged();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _errors = [];
            await Task.FromResult(1);
            /*  await JS.InvokeAsync<string>("FooterResized", DotNetObjectReference.Create(this)); */
        }
        private async Task SendEmailByKeyBoard(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await SendEmailClick();
            }
        }
        protected async Task ToggleSocialIcon()
        {
            await Task.FromResult(1);
        }
        public string GetFirstTextDecoration(string text)
        {
            return text[..(text.Length / 2)];
        }
        public string GetLastTextDecoration(string text)
        {
            var index = (text[..(text.Length / 2)]).Length;
            return text[index..];
        }
        private void FirsLoadChanged()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        protected override void Dispose(bool disposed)
        {
            FirstloadState.OnChange -= FirsLoadChanged;
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
            base.Dispose(disposed);
        }
    }

    public class SubscribeCommand
    {
        public string Email { get; set; } = "";
    }
    public class SubscribeCommandFluentValidator : AbstractValidator<SubscribeCommand>
    {
        public SubscribeCommandFluentValidator(ITranslationService translationService)
        {
            RuleFor(x => x.Email)
            .Cascade(CascadeMode.Continue)
            .EmailAddress()
            .WithMessage(translationService.Translate(SystemTextId.Avalidemailisrequired, "A valid email is required").Text)
            .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
            .WithMessage(translationService.Translate(SystemTextId.Avalidemailisrequired, "A valid email is required").Text)
            .MustAsync(async (value, cancellationToken) => await IsUniqueAsync(value));
        }
        private async Task<bool> IsUniqueAsync(string email)
        {
            // Simulates a long running http call
            await Task.Delay(1);
            return !email.Equals("test@test.com", StringComparison.CurrentCultureIgnoreCase);
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
         {
             var result = await ValidateAsync(ValidationContext<SubscribeCommand>.CreateWithOptions((SubscribeCommand)model, x => x.IncludeProperties(propertyName)));
             if (result.IsValid)
                 return [];
             return result.Errors.Select(e => e.ErrorMessage);
         };
    }
}

