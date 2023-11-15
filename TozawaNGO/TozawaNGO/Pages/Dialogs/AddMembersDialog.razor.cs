using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TozawaNGO.Helpers;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.FormModels;
using TozawaNGO.Services;
using TozawaNGO.Shared;

namespace TozawaNGO.Pages
{
    public partial class AddMembersDialog : BaseDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public string Name { get; set; }
        [Inject] MemberService memberService { get; set; }
        [Inject] private ISnackBarService snackBarService { get; set; }

        private MudForm _addForm;
        private AddMemberRequest _addFormModel = new();
        private bool _success;
        private bool _onProgress = false;
        private string[] _errors = Array.Empty<string>();
        [Parameter] public List<ActiveLanguageDto> _activeLanguages { get; set; } = new();
        public ActiveLanguageDto _currentCulture { get; set; } = new();

        void Cancel() => MudDialog.Cancel();

        protected override async Task OnInitializedAsync()
        {
            _currentCulture = await _translationService.GetActiveLanguage();

            foreach (var item in _activeLanguages.Where(l => l.Id != _currentCulture.Id))
            {
                _addFormModel.DescriptionTranslations.Add(new TranslationRequest
                {
                    LanguageId = item.Id,
                    Text = ""
                });
            }
        }


        private async Task<IEnumerable<string>> EmailExists(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new List<string> { Translate(SystemTextId.EmailIsRequired) };
            }

            var emailExists = await memberService.EmailExists(email);
            if (emailExists)
            {
                return new List<string> { Translate(SystemTextId.EmailAlreadyExists) };
            }
            return new List<string>();
        }
        private bool DisabledAddButton()
        {
            return !_success || _onProgress;
        }
        private string GetLabelText(string fieldName, ActiveLanguageDto language)
        {
            var fieldRuleText = Translate(SystemTextId.Required);
            return $"{fieldName} [{language.LongName} ({fieldRuleText})]";
        }
        private async Task AddItem()
        {
            _onProgress = true;

            var model = _addForm.Model as AddMemberRequest;
            var added = await memberService.AddMember(model);

            if (added.Success)
            {
                MudDialog.Close(DialogResult.Ok(added));
            }
            else
            {
                _errors = _errors.Append(await _translationService.GetHttpStatusText(added.StatusCode)).ToArray();
                snackBarService.Add(added);
                _onProgress = false;
                StateHasChanged();
            }
        }

    }
}