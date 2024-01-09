using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using TozawaNGO.Helpers;
using TozawaNGO.Models;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.Enums;
using TozawaNGO.Models.FormModels;
using TozawaNGO.Services;
using TozawaNGO.Shared;

namespace TozawaNGO.Pages
{
    public partial class FilesEntityDialog : BaseDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public IAttachmentEntity Entity { get; set; }
        [Parameter] public bool HasPermission { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected AttachmentService AttachmentService { get; set; }
        [Inject] protected FileService FileService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        private AttachmentType _attachmentType { get; set; } = AttachmentType.Intern;
        protected static Guid _areYouSureTranslation = Guid.Parse("c41187b6-6dc7-4e9a-a403-4982b34f21f8");
        protected int _thumbnailSize = 24;
        private bool _onProgress;
        private readonly List<IBrowserFile> _files = [];
        private string _alphaNumericFileNameValidationMessage;
        private string _fileNameLengthValidationMessage;
        private string _fileTypeValidationMessage;

        protected void Add() => MudDialog.Close(DialogResult.Ok(Entity));
        protected void Cancel() => MudDialog.Cancel();
        protected override void OnInitialized()
        {
            _alphaNumericFileNameValidationMessage = _translationService.Translate(SystemTextId.PleaseUsevalidFileName, "The file name should be alpha numeric").Text;
            _fileNameLengthValidationMessage = _translationService.Translate(SystemTextId.PleaseUseNormalFileNameLength, "Please the file name is too large, use maz 255 characters as recommanded").Text;
            _fileTypeValidationMessage = _translationService.Translate(SystemTextId.PleaseUseValidAllowedFiles, "the file type is not valid, only images pdf word textfile and excel are allowed").Text;
        }
        private bool IsValideFile(IBrowserFile file)
        {
            if (!FileValidator.IsValidContentType(file.ContentType))
            {
                Snackbar.Add(_fileTypeValidationMessage, Severity.Info);
                return false;
            }
            else if (!FileValidator.IsValidName(file.Name))
            {
                Snackbar.Add(_alphaNumericFileNameValidationMessage, Severity.Info);
                return false;
            }
            else if (!FileValidator.IsValidLength(file.Name))
            {
                Snackbar.Add(_fileNameLengthValidationMessage, Severity.Info);
                return false;
            }

            return true;
        }

        protected string GetFileTypeIcon(FileAttachmentDto attachment) =>
            attachment.MimeType.Contains("image") ? Icons.Material.Filled.Image : Icons.Material.Filled.FileCopy;

        private async void Download(FileAttachmentDto attachment)
        {
            _onProgress = true;
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
                ["item"] = attachment,
                ["title"] = Translate(SystemTextId.Delete)
            };
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var dialog = DialogService.Show<DeleteEntityDialog>(Translate(SystemTextId.Delete), parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                _onProgress = true;
                StateHasChanged();

                var deleteResponse = await AttachmentService.AttachmentDelete(attachment.Id);
                if (deleteResponse.Success)
                {
                    Entity.Attachments.Remove(attachment);
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
                _onProgress = false;
                StateHasChanged();
            }
        }

        private async void UploadFiles(InputFileChangeEventArgs e)
        {
            try
            {
                _onProgress = true;
                StateHasChanged();
                foreach (var file in e.GetMultipleFiles())
                {
                    if (!IsValideFile(file))
                    {
                        _onProgress = false;
                        return;
                    }

                    if (Entity.Attachments.Any(x => x.Name == file.Name && x.MimeType == file.ContentType))
                    {
                        Snackbar.Add($"{Translate(SystemTextId.FileName, "Filename")} {file.Name.ToUpper()} {Translate(SystemTextId.AlreadyExists, "already exist")} : {Translate(SystemTextId.Name, "Name")}", Severity.Warning);
                        _onProgress = false;
                        StateHasChanged();

                        return;
                    }
                    else
                    {
                        _files.Add(file);
                    }
                }
                if (_files.Any())
                {
                    var request = new AttachmentUploadRequest();

                    if (_files.Any(x => x.Size > FileValidator.MaxAllowedSize))
                    {
                        Snackbar.Add($"{Translate(SystemTextId.TheAllowedMaximumSizeIs, "The Allowed maximum size is")} {Math.Round(Convert.ToDouble(FileValidator.MaxAllowedSize) / 1000, 2)}KB", Severity.Warning);
                        _files.Clear();
                    }
                    else
                    {
                        foreach (var file in _files)
                        {
                            var buffers = new byte[file.Size];
                            await file.OpenReadStream(maxAllowedSize: FileValidator.MaxAllowedSize).ReadAsync(buffers);
                        }
                        await request.AddFiles(_files);
                        request.FileAttachmentType = _attachmentType;
                        request.FolderName = Entity.Email;
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
                _onProgress = false;
                StateHasChanged();
            }
            catch (Exception)
            {
                _files.Clear();
            }
        }
    }
}