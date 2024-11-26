using FluentValidation;
using Fluxor;
//using Grains.Auth.Models.Dtos.Backend;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.FormModels;
using ShareRazorClassLibrary.Services;
using TozawaNGO.Services;
using TozawaNGO.Shared;
using TozawaNGO.State.Member.Store;

namespace TozawaNGO.Pages
{
    public partial class EditMembersDialog : BaseDialog<EditMembersDialog>
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public MemberDto Member { get; set; }
        [Inject] MemberService memberService { get; set; }
        [Inject] private ISnackBarService snackBarService { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }
        private bool _disabledPage = false;
        private bool _RequestInProgress = false;
        private string _disableAttrString = "";
        private MudForm _editForm;
        private MemberDto _backupItem;
        protected PatchMemberRequest _patchMemberRequest = new();
        private bool _success;
        private bool _descriptionIsRequired = false;
        private bool _onProgress = false;
        private string[] _errors = [];
        private Dictionary<string, string> RestoreInputIcon = [];

        private async Task RestoreText(string type)
        {
            if (type == nameof(MemberDto.FirstName))
            {
                Member.FirstName = _backupItem.FirstName;
                RestoreInputIcon[nameof(MemberDto.FirstName)] = "";
            }
            else if (type == nameof(MemberDto.LastName))
            {
                Member.LastName = _backupItem.LastName;
                RestoreInputIcon[nameof(MemberDto.LastName)] = "";
            }
            else if (type == nameof(MemberDto.Email))
            {
                Member.Email = _backupItem.Email;
                RestoreInputIcon[nameof(MemberDto.Email)] = "";
                MudDialog.StateHasChanged();
            }
            else if (type == nameof(MemberDto.Description))
            {
                Member.Description = _backupItem.Description;
                RestoreInputIcon[nameof(MemberDto.Description)] = "";
            }
            await Task.Delay(new TimeSpan(0, 0, Convert.ToInt32(0.6))).ContinueWith(async o =>
            {
                await InvokeAsync(async () =>
            {
                await _editForm.Validate();
            });
            });
        }
        private async Task HandleTextField(string type)
        {
            if (type == nameof(MemberDto.FirstName))
            {
                if (!string.IsNullOrEmpty(Member.FirstName) && Member.FirstName.Equals(_backupItem.FirstName))
                {
                    RestoreInputIcon[nameof(MemberDto.FirstName)] = "";
                }
                else
                {
                    RestoreInputIcon[nameof(MemberDto.FirstName)] = Icons.Material.Filled.Undo;
                }
            }
            else if (type == nameof(MemberDto.LastName))
            {
                if (!string.IsNullOrEmpty(Member.LastName) && Member.LastName.Equals(_backupItem.LastName))
                {
                    RestoreInputIcon[nameof(MemberDto.LastName)] = "";
                }
                else
                {
                    RestoreInputIcon[nameof(MemberDto.LastName)] = Icons.Material.Filled.Undo;
                }
            }
            else if (type == nameof(MemberDto.Email))
            {
                if (!string.IsNullOrEmpty(Member.Email) && Member.Email.Equals(_backupItem.Email))
                {
                    RestoreInputIcon[nameof(MemberDto.Email)] = "";
                }
                else
                {
                    RestoreInputIcon[nameof(MemberDto.Email)] = Icons.Material.Filled.Undo;
                }
                MudDialog.StateHasChanged();
            }
            else if (type == nameof(MemberDto.Description))
            {
                if ((!string.IsNullOrEmpty(Member.Description) && Member.Description.Equals(_backupItem.Description)) ||
                      Member.Description == _backupItem.Description)
                {
                    RestoreInputIcon[nameof(MemberDto.Description)] = "";
                }
                else
                {
                    if (!string.IsNullOrEmpty(_backupItem.Description))
                    {
                        RestoreInputIcon[nameof(MemberDto.Description)] = Icons.Material.Filled.Undo;
                    }
                }
            }
            await _editForm.Validate();
        }
        private MemberDtoFluentValidator EmailValidator()
        {
            return new MemberDtoFluentValidator(_translationService);
        }
        void Cancel()
        {
            ResetItemToOriginalValues();
            MudDialog.Cancel();
        }
        private async void DisabledPage()
        {
            _disabledPage = _RequestInProgress;

            _disableAttrString = _disabledPage ? "pointer-events: none;" : "";
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        public override void Dispose()
        {
            base.Dispose();
        }
        protected override void OnInitialized()
        {
            var memperProperties = typeof(MemberDto).GetProperties();
            foreach (var prop in memperProperties)
            {
                RestoreInputIcon.Add(prop.Name, "");
            }
            _descriptionIsRequired = !string.IsNullOrEmpty(Member.Description);
            BackupItem();
        }
        private void ResetItemToOriginalValues()
        {
            Member.FirstName = _backupItem.FirstName;
            Member.Description = _backupItem.Description;
            Member.LastName = _backupItem.LastName;
            Member.Email = _backupItem.Email;
        }
        private void BackupItem()
        {
            _backupItem = new()
            {
                FirstName = Member.FirstName,
                LastName = Member.LastName,
                Email = Member.Email,
                Description = Member.Description,
            };
        }
        private async Task<IEnumerable<string>> ValidateFstName(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return await Task.FromResult<IEnumerable<string>>([Translate(SystemTextId.FirstNameIsRequired)]);
            }

            if (text.Length < 3)
            {
                return await Task.FromResult<IEnumerable<string>>(["Name too short!"]);
            }

            return [];
        }
        private async Task<IEnumerable<string>> ValidateLastName(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return await Task.FromResult<IEnumerable<string>>([Translate(SystemTextId.LastNameIsRequired)]);
            }

            if (text.Length < 3)
            {
                return await Task.FromResult<IEnumerable<string>>(["Name too short!"]);
            }

            return [];
        }
        private async Task<IEnumerable<string>> ValidateDescName(string text)
        {
            if (!_descriptionIsRequired) return [];
            if (string.IsNullOrEmpty(text))
            {
                return await Task.FromResult<IEnumerable<string>>([Translate(SystemTextId.DescriptionIsRequired)]);
            }

            if (text.Length < 3)
            {
                return await Task.FromResult<IEnumerable<string>>(["Description too short!"]);
            }

            return [];
        }
        private async Task<IEnumerable<string>> EmailExists(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return [Translate(SystemTextId.EmailIsRequired)];
            }

            if (email.Equals(_backupItem.Email)) return [];

            var emailExists = await memberService.EmailExists(email);
            if (emailExists)
            {
                return [Translate(SystemTextId.EmailAlreadyExists)];
            }
            var validator = await EmailValidator().ValidateAsync(Member);

            if (!validator.IsValid)
            {
                return [validator.Errors.First().ErrorMessage];
            }

            return [];
        }
        private bool DisabledAddButton()
        {
            return !_success || _onProgress || (!AnyPropertyIsUpdated() && !AnyTextIsUpdated());
        }
        private string GetLabelText(string fieldName, ActiveLanguageDto language)
        {
            var fieldRuleText = Translate(SystemTextId.Required);
            return $"{fieldName} [{language.LongName} ({fieldRuleText})]";
        }
        private bool AnyTextIsUpdated()
        {
            var response = false;
            if (!_backupItem.Description.Equals(Member.Description))
            {
                _patchMemberRequest.Description = Member.Description;
                response = true;
            }
            return response;
        }
        private bool AnyPropertyIsUpdated()
        {
            var response = false;
            if (!_backupItem.FirstName.Equals(Member.FirstName))
            {
                _patchMemberRequest.FirstName = Member.FirstName;
                response = true;
            }
            if (!_backupItem.LastName.Equals(Member.LastName))
            {
                _patchMemberRequest.LastName = Member.LastName;
                response = true;
            }
            if (!_backupItem.Email.Equals(Member.Email))
            {
                _patchMemberRequest.Email = Member.Email;
                response = true;
            }
            return response;
        }
        private void SaveItemByKeyBoard(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                if (!DisabledAddButton())
                {
                    SaveItem();
                }
            }
        }
        private void SaveItem()
        {
            if (!_success) return;
            if (!AnyPropertyIsUpdated() && !AnyTextIsUpdated())
            {
                MudDialog.Cancel();
            }
            _RequestInProgress = true;
            _onProgress = true;

            if (!Member.Deleted)
            {
                Dispatcher.Dispatch(new MemberPatchAction(Member.Id, _patchMemberRequest, _backupItem));

                _patchMemberRequest = new();
                MudDialog.Close(DialogResult.Ok(true));
            }
        }
    }

    public class MemberDtoFluentValidator : AbstractValidator<MemberDto>
    {
        public MemberDtoFluentValidator(ITranslationService translationService)
        {
            RuleFor(x => x.Email)
            .Cascade(CascadeMode.Continue)
            .NotNull()
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
            return email.ToLower() != "test@test.com";
        }
    }
}