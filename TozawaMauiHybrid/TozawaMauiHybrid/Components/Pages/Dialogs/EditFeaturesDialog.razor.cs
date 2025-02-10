using FluentValidation;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.FormModels;
using TozawaMauiHybrid.Services;
using TozawaMauiHybrid.Component;
using TozawaMauiHybrid.State.Feature.Store;

namespace TozawaMauiHybrid.Pages
{
    public partial class EditFeaturesDialog : BaseDialog<EditFeaturesDialog>
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public FeatureDto Feature { get; set; }
        [Inject] FeatureService FeatureService { get; set; }
        [Inject] private ISnackBarService snackBarService { get; set; }
        [Inject] Fluxor.IDispatcher Dispatcher { get; set; }
        private bool _disabledPage = false;
        private bool _RequestInProgress = false;
        private string _disableAttrString = "";
        private MudForm _editForm;
        private FeatureDto _backupItem;
        protected PatchFeatureRequest _patchFeatureRequest = new();
        private bool _success;
        private bool _descriptionIsRequired = false;
        private bool _onProgress = false;
        private string[] _errors = [];
        private Dictionary<string, string> RestoreInputIcon = [];

        private async Task RestoreText(string type)
        {
            if (type == nameof(FeatureDto.Text))
            {
                Feature.Text = _backupItem.Text;
                RestoreInputIcon[nameof(FeatureDto.Text)] = "";
                MudDialog.StateHasChanged();
            }
            else if (type == nameof(FeatureDto.Description))
            {
                Feature.Description = _backupItem.Description;
                RestoreInputIcon[nameof(FeatureDto.Description)] = "";
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
            if (type == nameof(FeatureDto.Text))
            {
                if (!string.IsNullOrEmpty(Feature.Text) && Feature.Text.Equals(_backupItem.Text))
                {
                    RestoreInputIcon[nameof(FeatureDto.Text)] = "";
                }
                else
                {
                    RestoreInputIcon[nameof(FeatureDto.Text)] = Icons.Material.Filled.Undo;
                }
            }
            else if (type == nameof(FeatureDto.Description))
            {
                if ((!string.IsNullOrEmpty(Feature.Description) && Feature.Description.Equals(_backupItem.Description)) ||
                      Feature.Description == _backupItem.Description)
                {
                    RestoreInputIcon[nameof(FeatureDto.Description)] = "";
                }
                else
                {
                    if (!string.IsNullOrEmpty(_backupItem.Description))
                    {
                        RestoreInputIcon[nameof(FeatureDto.Description)] = Icons.Material.Filled.Undo;
                    }
                }
            }
            await _editForm.Validate();
        }
        private FeatureDtoFluentValidator IdValidator()
        {
            return new FeatureDtoFluentValidator(_translationService);
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
            var memperProperties = typeof(FeatureDto).GetProperties();
            foreach (var prop in memperProperties)
            {
                RestoreInputIcon.Add(prop.Name, "");
            }
            _descriptionIsRequired = !string.IsNullOrEmpty(Feature.Description);
            BackupItem();
        }
        private void ResetItemToOriginalValues()
        {
            Feature.Text = _backupItem.Text;
            Feature.Description = _backupItem.Description;
        }
        private void BackupItem()
        {
            _backupItem = new()
            {
                Text = Feature.Text,
                Description = Feature.Description,
            };
        }
        private async Task<IEnumerable<string>> ValidateText(string text)
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
        private bool DisabledAddButton()
        {
            return !_success || _onProgress || !AnyTextIsUpdated();
        }
        private bool AnyTextIsUpdated()
        {
            var response = false;
            if (!_backupItem.Text.Equals(Feature.Text))
            {
                _patchFeatureRequest.Text = Feature.Text;
                response = true;
            }
            if (!_backupItem.Description.Equals(Feature.Description))
            {
                _patchFeatureRequest.Description = Feature.Description;
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
            if (!AnyTextIsUpdated())
            {
                MudDialog.Cancel();
            }
            _RequestInProgress = true;
            _onProgress = true;

            if (!Feature.Deleted)
            {
                Dispatcher.Dispatch(new FeaturePatchAction(Feature.Id, _patchFeatureRequest, _backupItem));

                _patchFeatureRequest = new();
                MudDialog.Close(DialogResult.Ok(true));
            }
        }
    }

    public class FeatureDtoFluentValidator : AbstractValidator<FeatureDto>
    {
        public FeatureDtoFluentValidator(ITranslationService translationService)
        {
            RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage(translationService.Translate(SystemTextId.Required, "A valid text is required").Text)
            .NotNull();
        }
    }
}