using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.Enums;
using ShareRazorClassLibrary.Models.FormModels;
using ShareRazorClassLibrary.Services;
using TozawaNGO.Services;
using TozawaNGO.Shared;

namespace TozawaNGO.Pages
{
    public partial class FilesEntityDialog : BaseDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public IAttachmentEntity Entity { get; set; }
        [Parameter] public string Source { get; set; }
        [Parameter] public bool HasPermission { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected AttachmentService AttachmentService { get; set; }
        [Inject] protected FileService FileService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] private LoadingState LoadingState { get; set; }
        private AttachmentType _attachmentType { get; set; } = AttachmentType.Intern;
        protected static Guid _areYouSureTranslation = Guid.Parse("c41187b6-6dc7-4e9a-a403-4982b34f21f8");
        protected int _thumbnailSize = 24;
        private bool _onProgress;
        private readonly List<IBrowserFile> _files = [];
        private string _alphaNumericFileNameValidationMessage;
        private string _fileNameLengthValidationMessage;
        private string _fileTypeValidationMessage;
        private string _error = "";
        private string _searchString = null;
        private List<FileAttachmentDto> _attachments = [];
        private FileAttachmentDto _selectedItem;

        protected void Add() => MudDialog.Close(DialogResult.Ok(Entity));
        protected void Cancel() => MudDialog.Cancel();
        protected override void OnInitialized()
        {
            _alphaNumericFileNameValidationMessage = _translationService.Translate(SystemTextId.PleaseUsevalidFileName, "The file name should be alpha numeric").Text;
            _fileNameLengthValidationMessage = _translationService.Translate(SystemTextId.PleaseUseNormalFileNameLength, "Please the file name is too large, use max 255 characters as recommanded").Text;
            _fileTypeValidationMessage = _translationService.Translate(SystemTextId.PleaseUseValidAllowedFiles, "the file type is not valid, only images pdf word textfile and excel are allowed").Text;

            LoadData();
        }
        private string GetLabel(Guid labelId, string label)
        {
            return Translate(labelId, label);
        }
        protected string SelectedRowClassFunc(FileAttachmentDto element, int rowNumber)
        {
            if (_selectedItem != null && _selectedItem.Id == element.Id)
            {
                return "selected";
            }
            return string.Empty;
        }
        protected void RowClickEvent(TableRowClickEventArgs<FileAttachmentDto> tableRowClickEventArgs)
        {

        }
        private bool IsValideFile(IBrowserFile file)
        {
            if (!FileValidator.IsValidContentType(file.ContentType))
            {
                _error = _fileTypeValidationMessage;
                return false;
            }
            else if (!FileValidator.IsValidName(file.Name))
            {
                _error = _alphaNumericFileNameValidationMessage;
                return false;
            }
            else if (!FileValidator.IsValidLength(file.Name))
            {
                _error = _fileNameLengthValidationMessage;
                return false;
            }

            return true;
        }
        private static Func<FileAttachmentDto, bool> Filtered(string searchString) => x =>
                                                                              (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                               (!string.IsNullOrEmpty(x.FileAttachmentType) && x.FileAttachmentType.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                               (!string.IsNullOrEmpty(x.Size.ToString()) && x.Size.ToString().Contains(searchString, StringComparison.InvariantCultureIgnoreCase));

        private void LoadData()
        {
            if (!string.IsNullOrEmpty(_searchString))
            {
                _attachments = (Entity.Attachments ?? []).Where(Filtered(_searchString)).ToList();
            }
            else
            {
                _attachments = (Entity.Attachments ?? []).Select(x => (FileAttachmentDto)x.Clone()).ToList();
            }
            StateHasChanged();
        }
        protected void OnSearch(string text)
        {
            _searchString = text;

            _attachments = Entity.Attachments.Where(Filtered(_searchString)).ToList();
            StateHasChanged();
        }
        protected string GetFileTypeIcon(FileAttachmentDto attachment) =>
            attachment.MimeType.Contains("image") ? Icons.Material.Filled.Image : Icons.Material.Filled.FileCopy;

        private async void Download(FileAttachmentDto attachment)
        {
            _onProgress = true;
            LoadingState.SetRequestInProgress(true);
            StateHasChanged();

            var attachmentResponse = await AttachmentService.AttachmentDownload(attachment.Id);
            if (attachmentResponse.Success)
            {
                var item = attachmentResponse.Entity ?? new AttachmentDownloadDto();
                await FileService.Download(item.Name, item.Content);
            }
            else
            {
                Snackbar.Add(attachmentResponse.Message, Severity.Error);
            }
            LoadingState.SetRequestInProgress(false);
            _onProgress = false;
            StateHasChanged();
        }
        private bool DisabledUploadFiles()
        {
            return Entity.Deleted || !HasPermission || _onProgress;
        }
        private async void Delete(FileAttachmentDto attachment)
        {
            var parameters = new DialogParameters
            {
                ["hardDelete"] = true,
                ["body"] = Translate(SystemTextId.AreYouSure),
                ["item"] = attachment,
                ["title"] = Translate(SystemTextId.Delete)
            };
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var dialog = DialogService.Show<DeleteEntityDialog>(Translate(SystemTextId.Delete), parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                LoadingState.SetRequestInProgress(true);
                _onProgress = true;
                StateHasChanged();

                var deleteResponse = await AttachmentService.AttachmentDelete(attachment.Id, Entity.Id, Source);
                if (deleteResponse.Success)
                {
                    Entity.Attachments.RemoveAll(x => x.Id == attachment.Id);
                    var files = new OwnerAttachments
                    {
                        OwnerId = Entity.Id,
                        Attachments = [attachment]
                    };
                    StateHasChanged();
                    AttachmentService.SetNotifyChange(files, true);
                }
                else
                {
                    Snackbar.Add(deleteResponse.Message, Severity.Error);
                }
                LoadingState.SetRequestInProgress(false);
                _onProgress = false;
                LoadData();
                StateHasChanged();
            }
        }

        private async void UploadFiles(InputFileChangeEventArgs e)
        {
            try
            {
                _error = string.Empty;
                if (e.FileCount > 10)
                {
                    _error = _translationService.Translate(SystemTextId.MaximumFilesAllowed, "You can only upload 10 files at a moment!").Text;
                    return;
                }
                LoadingState.SetRequestInProgress(true);
                _onProgress = true;
                StateHasChanged();
                foreach (var file in e.GetMultipleFiles())
                {
                    if (!IsValideFile(file))
                    {
                        LoadingState.SetRequestInProgress(false);
                        _onProgress = false;
                        return;
                    }

                    if (Entity.Attachments.Any(x => x.Name == file.Name && x.MimeType == file.ContentType))
                    {
                        _error = $"{Translate(SystemTextId.FileName, "Filename")} {file.Name.ToUpper()} {Translate(SystemTextId.AlreadyExists, "already exist")} : {Translate(SystemTextId.Name, "Name")}";
                        LoadingState.SetRequestInProgress(false);
                        _onProgress = false;
                        StateHasChanged();

                        return;
                    }
                    else
                    {
                        _files.Add(file);
                    }
                }
                if (_files.Count != 0)
                {
                    var request = new AttachmentUploadRequest();

                    if (_files.Any(x => x.Size > FileValidator.MaxAllowedSize))
                    {
                        _error = $"{Translate(SystemTextId.TheAllowedMaximumSizeIs, "The Allowed maximum size is")} {Math.Round(Convert.ToDouble(FileValidator.MaxAllowedSize) / 1000, 2)}KB";
                        _files.Clear();
                    }
                    else
                    {
                        await request.AddFiles(_files);
                        request.FileAttachmentType = _attachmentType;
                        request.FolderName = Entity.Email;
                        request.Source = Source;
                        var attachmentsResponse = await AttachmentService.AttachmentUpload(Entity.Id, request);
                        if (attachmentsResponse.Success)
                        {
                            Entity.Attachments.AddRange(attachmentsResponse.Entity ?? []);
                            _files.Clear();
                            var files = new OwnerAttachments
                            {
                                OwnerId = Entity.Id,
                                Attachments = attachmentsResponse.Entity ?? []
                            };
                            AttachmentService.SetNotifyChange(files);
                        }
                        else
                        {
                            Snackbar.Add(attachmentsResponse.Message, Severity.Error);
                        }
                    }
                }
                LoadingState.SetRequestInProgress(false);
                _onProgress = false;
                LoadData();
                StateHasChanged();
            }
            catch (Exception)
            {
                _files.Clear();
            }
        }
    }
}