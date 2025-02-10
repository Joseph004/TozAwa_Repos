using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using TozawaMauiHybrid.Component;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.FormModels;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Pages
{
    public partial class AddFeaturesDialog : BaseDialog<AddFeaturesDialog>
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public string Name { get; set; }
        [Parameter] public string Title { get; set; }
        [Inject] FeatureService FeatureService { get; set; }
        [Inject] private ISnackBarService snackBarService { get; set; }
        private bool _disabledPage = false;
        private bool _RequestInProgress = false;
        private string _disableAttrString = "";
        private async void DisabledPage()
        {
            _disabledPage = _RequestInProgress;

            _disableAttrString = _disabledPage ? "pointer-events: none;" : "";
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        private MudForm _addForm;
        private AddFeatureRequest _addFormModel = new();
        private bool _success;
        private bool _onProgress = false;
        private string[] _errors = [];
        [Parameter] public List<ActiveLanguageDto> _activeLanguages { get; set; } = [];
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
        private async Task<IEnumerable<string>> ValidateTextName(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return await Task.FromResult<IEnumerable<string>>([Translate(SystemTextId.Required)]);
            }

            if (text.Length < 3)
            {
                return await Task.FromResult<IEnumerable<string>>(["Text too short!"]);
            }

            return [];
        }
        private async Task<IEnumerable<string>> ValidateDescName(string text)
        {
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
        private AddFeatureRequestFluentValidator IdValidator()
        {
            return new AddFeatureRequestFluentValidator(_translationService);
        }
        private async Task<IEnumerable<string>> IdExists(int? id)
        {
            if (id == null || id == 0)
            {
                return [Translate(SystemTextId.Required)];
            }

            var idExists = await FeatureService.FeatureExists(id.Value);
            if (idExists)
            {
                return [Translate(SystemTextId.AlreadyExists)];
            }
            var validator = await IdValidator().ValidateAsync(_addFormModel);

            if (!validator.IsValid)
            {
                return [validator.Errors.First().ErrorMessage];
            }
            return [];
        }
        private void AddItemByKeyBoard(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                if (!DisabledAddButton())
                {
                    AddItem();
                }
            }
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
        private void AddItem()
        {
            if (DisabledAddButton()) return;
            _RequestInProgress = true;
            _onProgress = true;

            var model = _addForm.Model as AddFeatureRequest;

            MudDialog.Close(DialogResult.Ok(model));
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}