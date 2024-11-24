using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.Enums;
using ShareRazorClassLibrary.Models.FormModels;
using ShareRazorClassLibrary.Services;
using MudBlazor.Extensions.Options;
using TozawaNGO.Services;
using TozawaNGO.Shared;
using MudBlazor.Extensions;

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
        private AttachmentType _attachmentType { get; set; } = AttachmentType.Intern;
        protected static Guid _areYouSureTranslation = Guid.Parse("c41187b6-6dc7-4e9a-a403-4982b34f21f8");
        protected int _thumbnailSize = 24;
        private bool _onProgress;
        private readonly List<IBrowserFile> _files = [];
        private readonly List<(byte[], string, string)> _filesToSend = [];
        private string _alphaNumericFileNameValidationMessage;
        private string _fileNameLengthValidationMessage;
        private string _fileTypeValidationMessage;
        private string _error = "";
        private string _searchString = null;
        private bool _RequestInProgress = false;
        private List<FileAttachmentDto> _attachments = [];
        private FileAttachmentDto _selectedItem;

        protected void Add() => MudDialog.Close(DialogResult.Ok(Entity));
        protected void Cancel() => MudDialog.Cancel();
        private bool _disabledPage = false;
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
        public override void Dispose()
        {
            base.Dispose();
        }
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
            showAlert = false;
            if (!FileValidator.IsValidContentType(file.ContentType))
            {
                _error = _fileTypeValidationMessage;
                showAlert = true;
                return false;
            }
            else if (!FileValidator.IsValidName(file.Name))
            {
                _error = _alphaNumericFileNameValidationMessage;
                showAlert = true;
                return false;
            }
            else if (!FileValidator.IsValidLength(file.Name))
            {
                _error = _fileNameLengthValidationMessage;
                showAlert = true;
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
        private bool showAlert = true;
        private bool showOptionButtonToPdf = false;
        private Dictionary<Guid, bool> showSelect = [];
        private void CloseMe(bool value)
        {
            showAlert = value;
        }
        private async Task ConvertToPdf(FileAttachmentDto attachment, string command)
        {
            showSelect = [];
            showAlert = false;
            showOptionButtonToPdf = !showOptionButtonToPdf;
            var item = await GetAttachmentDownloadDto(attachment);
            _RequestInProgress = true;
            _onProgress = true;
            var converted = await AttachmentService.ConvertToPdf(item.Content, item.MineType);
            attachment.PdfConvertedContent = converted;
            item.PdfConvertedContent = converted;
            var name = (!FileValidator.IsPdf(item.MineType) && FileValidator.IsValiBytes(item.PdfConvertedContent)) ? item.Name.Split(".")[0] + ".pdf" : item.Name;
            if (command == "view")
            {
                var options = new DialogOptionsEx
                {
                    Resizeable = true,
                    BackdropClick = false,
                    DragMode = MudDialogDragMode.Simple,
                    Position = DialogPosition.TopCenter,
                    CloseButton = false,
                    MaxWidth = MaxWidth.ExtraLarge
                };
                var attachments = _attachments.Where(x => FileValidator.IsImage(x.MimeType) || FileValidator.IsPdf(x.MimeType) || (!FileValidator.IsPdf(x.MimeType) && FileValidator.IsValiBytes(x.PdfConvertedContent)));
                var parameters = new DialogParameters
                {
                    ["Attachment"] = new Dictionary<Guid, AttachmentDownloadDto> { { attachment.Id, item } },
                    ["Attachments"] = attachments
                };
                _onProgress = false;
                _RequestInProgress = false;
                var dialog = await DialogService.ShowEx<ShowDocumentDialog>($"{name}", parameters, options);
                var result = await dialog.Result;
            }
            else
            {
                if (!string.IsNullOrEmpty(item.Name))
                {
                    await FileService.Download(name, converted);
                }
            }
            showSelect.Add(attachment.Id, showOptionButtonToPdf);
            _onProgress = false;
            _RequestInProgress = false;
            MudDialog.StateHasChanged();
        }
        private async Task ShowDocumentInFrame(FileAttachmentDto attachment)
        {
            showSelect = [];
            _error = "";
            showAlert = false;
            showOptionButtonToPdf = !showOptionButtonToPdf;
            if (FileValidator.IsImage(attachment.MimeType) || FileValidator.IsPdf(attachment.MimeType))
            {
                var item = await GetAttachmentDownloadDto(attachment);
                if (FileValidator.IsValideImage(item.Content) || FileValidator.IsValidePdf(item.Content))
                {
                    var options = new DialogOptionsEx
                    {
                        Resizeable = true,
                        BackdropClick = false,
                        DragMode = MudDialogDragMode.Simple,
                        Position = DialogPosition.TopCenter,
                        CloseButton = false,
                        MaxWidth = MaxWidth.ExtraLarge
                    };
                    var attachments = _attachments.Where(x => FileValidator.IsImage(x.MimeType) || FileValidator.IsPdf(x.MimeType) || FileValidator.IsTextplain(x.MimeType));
                    var parameters = new DialogParameters
                    {
                        ["Attachment"] = new Dictionary<Guid, AttachmentDownloadDto> { { attachment.Id, item } },
                        ["Attachments"] = attachments
                    };
                    var dialog = await DialogService.ShowEx<ShowDocumentDialog>($"{attachment.Name}", parameters, options);
                    var result = await dialog.Result;
                }
                else
                {
                    _error = _translationService.Translate(SystemTextId.Error, "Error").Text;
                    showAlert = true;
                }
                MudDialog.StateHasChanged();
            }
            else
            {
                _error = _translationService.Translate(SystemTextId.OnlyPdfAndImage, "Only image and pdf are visable").Text;
                showAlert = !showAlert;
                showSelect.Add(attachment.Id, showOptionButtonToPdf);
            }
            MudDialog.StateHasChanged();
        }
        private async Task<AttachmentDownloadDto> GetAttachmentDownloadDto(FileAttachmentDto attachment)
        {
            AttachmentDownloadDto item = new();
            _onProgress = true;
            _RequestInProgress = true;

            if (FileValidator.IsValiBytes(attachment.Content))
            {
                item = new AttachmentDownloadDto
                {
                    Name = attachment.Name,
                    Content = attachment.Content,
                    MineType = attachment.MimeType
                };
            }
            else
            {
                var attachmentResponse = await AttachmentService.AttachmentDownload(attachment.Id);
                if (attachmentResponse.Success)
                {
                    item = attachmentResponse.Entity ?? new AttachmentDownloadDto();
                    attachment.Content = item.Content;
                }
                else
                {
                    Snackbar.Add(attachmentResponse.Message, Severity.Error);
                }
            }
            _RequestInProgress = false;
            _onProgress = false;
            return item;
        }
        private async void Download(FileAttachmentDto attachment)
        {
            var item = await GetAttachmentDownloadDto(attachment);
            if (!string.IsNullOrEmpty(item.Name))
            {
                await FileService.Download(item.Name, item.Content);
            }
            MudDialog.StateHasChanged();
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
                _RequestInProgress = true;
                _onProgress = true;
                MudDialog.StateHasChanged();

                var deleteResponse = await AttachmentService.AttachmentDelete(attachment.Id, Entity.Id, Guid.Parse(attachment.BlobId), Source);
                if (deleteResponse.Success)
                {
                    Entity.Attachments.RemoveAll(x => x.Id == attachment.Id);
                    var files = new OwnerAttachments
                    {
                        OwnerId = Entity.Id,
                        Attachments = [attachment]
                    };
                    MudDialog.StateHasChanged();
                    AttachmentService.SetNotifyChange(files, true);
                }
                else
                {
                    Snackbar.Add(deleteResponse.Message, Severity.Error);
                }
                _RequestInProgress = false;
                _onProgress = false;
                LoadData();
                MudDialog.StateHasChanged();
            }
        }

        private async void UploadFiles(InputFileChangeEventArgs e)
        {
            try
            {
                var request = new AttachmentUploadRequest();
                _error = string.Empty;
                showAlert = false;
                if (e.FileCount > 10)
                {
                    _error = _translationService.Translate(SystemTextId.MaximumFilesAllowed, "You can only upload 10 files at a moment!").Text;
                    showAlert = true;
                    return;
                }
                _RequestInProgress = true;
                _onProgress = true;
                MudDialog.StateHasChanged();
                foreach (var file in e.GetMultipleFiles())
                {
                    if (!IsValideFile(file))
                    {
                        _RequestInProgress = false;
                        _onProgress = false;
                        return;
                    }

                    var fileBytes = await request.GetBytes(file);
                    var fileName = file.Name;
                    var fileType = file.ContentType;
                    var typesToConvert = FileValidator._imagesToConvertToPng.Split(",");
                    if (typesToConvert.FirstOrDefault(x => file.ContentType == x) != null)
                    {
                        var response = await AttachmentService.ConvertImageToPng(await file.GetBytes());
                        if (response != null && response.Length > 0)
                        {
                            fileBytes = response;
                            fileName = file.Name.Split(".")[0] + ".png";
                            fileType = "image/png";
                        }
                        else
                        {
                            _error = $"{_translationService.Translate(SystemTextId.Error, "Error").Text} file: {file.Name}";
                            showAlert = true;
                            return;
                        }
                    }

                    if (Entity.Attachments.Any(x => x.Name == file.Name && x.MimeType == file.ContentType))
                    {
                        _error = $"{Translate(SystemTextId.FileName, "Filename")} {file.Name.ToUpper()} {Translate(SystemTextId.AlreadyExists, "already exist")} : {Translate(SystemTextId.Name, "Name")}";
                        showAlert = true;
                        _RequestInProgress = false;
                        _onProgress = false;
                        MudDialog.StateHasChanged();

                        return;
                    }
                    else
                    {
                        _filesToSend.Add((fileBytes, fileType, fileName));
                        _files.Add(file);
                    }
                }
                if (_files.Count != 0)
                {
                    if (_files.Any(x => x.Size > FileValidator.MaxAllowedSize))
                    {
                        _error = $"{Translate(SystemTextId.TheAllowedMaximumSizeIs, "The Allowed maximum size is")} {Math.Round(Convert.ToDouble(FileValidator.MaxAllowedSize) / 1000, 2)}KB";
                        showAlert = true;
                        _files.Clear();
                        _filesToSend.Clear();
                    }
                    else
                    {
                        request.AddFiles(_filesToSend);
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
                _RequestInProgress = false;
                _onProgress = false;
                LoadData();
                MudDialog.StateHasChanged();
            }
            catch (Exception)
            {
                _files.Clear();
            }
        }
    }
}