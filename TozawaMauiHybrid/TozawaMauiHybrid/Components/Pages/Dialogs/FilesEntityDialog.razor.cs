using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Models;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.Enums;
using TozawaMauiHybrid.Models.FormModels;
using TozawaMauiHybrid.Services;
using MudBlazor.Extensions.Options;
using TozawaMauiHybrid.Component;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using Nextended.Core.Extensions;
using MudBlazor.Extensions.Components;

namespace TozawaMauiHybrid.Pages
{
    public partial class FilesEntityDialog : BaseDialog<FilesEntityDialog>
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
        protected int[] _pageSizeOptions = [20, 50, 100];
        protected int _page = 0;
        protected int _pageSize = 20;
        private IEnumerable<FileAttachmentDto> pagedData;
        private MudTable<FileAttachmentDto> _table;
        private DialogParameters _parameters = [];
        private Stream _fileStream = null;
        private string _fileName = "";
        private string _fileMimeType = "";
        private readonly List<IBrowserFile> _files = [];
        public Dictionary<Guid, AttachmentDownloadDto> Attachment { get; set; }
        public IEnumerable<FileAttachmentDto> Attachments { get; set; }
        public List<Guid> AttachmentIds { get; set; }
        private bool _disabledForwards = false;
        private bool _disabledBackwards = false;
        private string _textForwards = string.Empty;
        private string _textBackwards = string.Empty;
        private bool _disabledFileViewPage = false;
        IMudExDialogReference<MudExFileDisplayDialog> _fileDialog;
        private readonly List<(byte[], string, string)> _filesToSend = [];
        private string _alphaNumericFileNameValidationMessage;
        private string _fileNameLengthValidationMessage;
        private string _fileTypeValidationMessage;
        private string _error = "";
        private string _searchString = null;
        private bool _RequestInProgress = false;
        private bool _isServerReload = true;
        private int _totalItems;
        private FileAttachmentDto _selectedItem;
        Action<DialogOptionsEx> _fileOptionsEx;
        Action _DownloadFile;
        public int? _currentFileIndex = null;

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
        public void HandleAttachment()
        {
            if (!AttachmentService.OnDelete)
            {
                if (AttachmentService.FileAttachmentDtos != null && !pagedData.Any(x => x.Id == AttachmentService.FileAttachmentDtos.Attachments.First().Id))
                {
                    pagedData.Append(AttachmentService.FileAttachmentDtos.Attachments.First());
                    _totalItems++;
                    _isServerReload = false;
                    _table.ReloadServerData();
                }
            }
            else
            {
                if (AttachmentService.FileAttachmentDtos != null && pagedData.Any(x => x.Id == AttachmentService.FileAttachmentDtos.Attachments.First().Id))
                {
                    pagedData = pagedData.Where(x => x.Id != AttachmentService.FileAttachmentDtos.Attachments.First().Id);
                    _totalItems--;
                    _isServerReload = false;
                    _table.ReloadServerData();
                }
            }
        }
        public override void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Dispose();
                _fileStream.Close();
            }
            if (_fileDialog != null)
            {
                _fileDialog.Dismiss(DialogResult.Cancel());
                _fileDialog.DialogReference.Dismiss(DialogResult.Cancel());
                _fileDialog.DialogComponent.Cancel();
                _fileDialog.DialogComponent.DisposeAsync();
            }
            AttachmentService.OnChange -= HandleAttachment;
            _fileOptionsEx -= SetOptions;
            base.Dispose();
        }
        protected override void OnInitialized()
        {
            AttachmentService.OnChange += HandleAttachment;

            _alphaNumericFileNameValidationMessage = _translationService.Translate(SystemTextId.PleaseUsevalidFileName, "The file name should be alpha numeric").Text;
            _fileNameLengthValidationMessage = _translationService.Translate(SystemTextId.PleaseUseNormalFileNameLength, "Please the file name is too large, use max 255 characters as recommanded").Text;
            _fileTypeValidationMessage = _translationService.Translate(SystemTextId.PleaseUseValidAllowedFiles, "the file type is not valid, only images pdf word textfile and excel are allowed").Text;
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
        private Func<FileAttachmentDto, int, string> _disableRowClick => (x, i) =>
        {
            string style = string.Empty;
            //condition for the row to be disabled
            if (_RequestInProgress)
            {
                style += "pointer-events: none;";
            }

            return style;
        };
        private async Task<TableData<FileAttachmentDto>> ServerReload(TableState state, CancellationToken token)
        {
            if (!_isServerReload)
            {
                _isServerReload = true;
                return new TableData<FileAttachmentDto>() { TotalItems = _totalItems, Items = pagedData };
            }

            _RequestInProgress = true;
            DisabledPage();
            _page = state.Page;
            _pageSize = state.PageSize;
            var response = await AttachmentService.GetAttachments(new TozawaMauiHybrid.Models.Requests.GetAttachments
            {
                SearchString = _searchString,
                OwnerId = Entity.Id,
                Page = _page,
                PageSize = _pageSize
            });
            var items = response.Entity.Items ?? [];
            _totalItems = response.Entity.TotalItems;
            pagedData = items;
            _RequestInProgress = false;
            DisabledPage();
            return new TableData<FileAttachmentDto>() { TotalItems = _totalItems, Items = pagedData };
        }
        protected async Task OnSearch(string text)
        {
            _searchString = text;
            await _table.ReloadServerData();
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
        public void SetParameters()
        {
            _parameters = new DialogParameters
            {
                {
                    nameof(MudExFileDisplay.StreamUrlHandling),
                    StreamUrlHandling.DataUrl
                    },
                    {
                        nameof(MudExFileDisplay.ContentStream),
                        _fileStream
                    },
                      {
                        nameof(MudExFileDisplay.Dense),
                        true
                      },
                      {
                        nameof(MudExFileDisplayDialog.Buttons),
                        new MudExDialogResultAction[]{
                        new(){
                       Label = Translate(SystemTextId.Download, "Download"),
                       Color = MudBlazor.Color.Secondary,
                       Variant = Variant.Filled,
                       OnClick = _DownloadFile
                        },
                        new() {
                           Label = Translate(SystemTextId.Previous, "Previous"),
                           Color = MudBlazor.Color.Primary,
                           Variant = Variant.Filled,
                           OnClick = Previous
                         },
                        new() {
                           Label = Translate(SystemTextId.Next, "Next"),
                           Color = MudBlazor.Color.Primary,
                           Variant = Variant.Filled,
                           OnClick = Next
                        }
                    }
                }
            };
        }
        public void SetOptions(DialogOptionsEx options)
        {
            options.BackgroundClass = "tz-mud-overlay";
            options.Position = DialogPosition.Center;
            options.DialogAppearance = MudExAppearance.FromStyle(b =>
            {
                b.WithBackgroundImage("url('/images/plain-white-background.jpg')")
                .WithBackgroundSize("cover")
                .WithBackgroundPosition("center center")
                .WithBackgroundRepeat("no-repeat")
                .WithOpacity(0.9);
            });
        }
        private async Task ConvertToPdf(FileAttachmentDto attachment, string command)
        {
            showSelect = [];
            showAlert = false;
            showOptionButtonToPdf = !showOptionButtonToPdf;
            _RequestInProgress = true;
            var item = await GetAttachmentDownloadDto(attachment);
            DisabledPage();
            var converted = await AttachmentService.ConvertToPdf(item.Content, item.MineType);
            attachment.PdfConvertedContent = converted;
            item.PdfConvertedContent = converted;
            var name = (!FileValidator.IsPdf(item.MineType) && FileValidator.IsValiBytes(item.PdfConvertedContent)) ? item.Name.Split(".")[0] + ".pdf" : item.Name;
            if (command == "view")
            {
                _fileOptionsEx += SetOptions;
                _fileStream = GetStream(item.PdfConvertedContent);
                SetParameters();
                SetFileExAttachments(item, attachment.Id, true);
                _RequestInProgress = false;
                DisabledPage();
                _fileName = (!FileValidator.IsPdf(Attachment.First().Value.MineType) && FileValidator.IsValiBytes(Attachment.First().Value.PdfConvertedContent)) ? Attachment.First().Value.Name.Split(".")[0] + ".pdf" : Attachment.First().Value.Name;
                _fileMimeType = (!FileValidator.IsPdf(Attachment.First().Value.MineType) && FileValidator.IsValiBytes(Attachment.First().Value.PdfConvertedContent)) ? FileValidator.pdf : Attachment.First().Value.MineType;
                _fileDialog = await DialogService.ShowFileDisplayDialog(_fileStream, _fileName, _fileMimeType, HandleContentError, _fileOptionsEx, _parameters);
                var result = await _fileDialog.Result;
            }
            else
            {
                if (!string.IsNullOrEmpty(item.Name))
                {
                    await FileService.Download(name, converted);
                }
            }
            showSelect.Add(attachment.Id, showOptionButtonToPdf);
            _RequestInProgress = false;
            DisabledPage();
            _fileDialog?.CallStateHasChanged();
            MudDialog.StateHasChanged();
        }
        private Task<MudExFileDisplayContentErrorResult> HandleContentError(IMudExFileDisplayInfos arg)
        {
            if (!FileValidator.IsPdf(arg.ContentType) || !FileValidator.IsImage(arg.ContentType))
            {
                return Task.FromResult(MudExFileDisplayContentErrorResult
                    .RedirectTo("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTiZiqnBKWS8NHcKbRH04UkYjrCgxUMz6sVNw&usqp=CAU", "image/png")
                    .WithMessage("No word plugin found we display a sheep"));
            }
            return Task.FromResult(MudExFileDisplayContentErrorResult.Unhandled);
        }
        private Stream GetStream(byte[] content)
        {
            return new MemoryStream(content);
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
                    _DownloadFile = DownloadFromFileView;
                    _fileOptionsEx += SetOptions;
                    _fileStream = GetStream(item.Content);
                    SetParameters();
                    SetFileExAttachments(item, attachment.Id);
                    _fileName = attachment.Name;
                    _fileMimeType = attachment.MimeType;
                    _fileDialog = await DialogService.ShowFileDisplayDialog(_fileStream, _fileName, _fileMimeType, HandleContentError, _fileOptionsEx, _parameters);
                    var result = await _fileDialog.Result;
                }
                else
                {
                    _error = _translationService.Translate(SystemTextId.Error, "Error").Text;
                    showAlert = true;
                }
                _fileDialog?.CallStateHasChanged();
                MudDialog.StateHasChanged();
            }
            else
            {
                _error = _translationService.Translate(SystemTextId.OnlyPdfAndImage, "Only image and pdf are visable").Text;
                showAlert = !showAlert;
                showSelect.Add(attachment.Id, showOptionButtonToPdf);
            }
            _fileDialog?.CallStateHasChanged();
            MudDialog.StateHasChanged();
        }
        private void Previous()
        {
            Move(Direction.Backwards);
            _fileDialog?.CallStateHasChanged();
            MudDialog.StateHasChanged();
        }
        private void Next()
        {
            Move(Direction.Forwards);
            _fileDialog?.CallStateHasChanged();
            MudDialog.StateHasChanged();
        }
        private void SetFileExAttachments(AttachmentDownloadDto item, Guid id, bool isConvertedToPdf = false)
        {
            Attachment = new Dictionary<Guid, AttachmentDownloadDto> { { id, item } };

            if (isConvertedToPdf)
            {
                Attachments = pagedData.Where(x => FileValidator.IsImage(x.MimeType) || FileValidator.IsPdf(x.MimeType) || (!FileValidator.IsPdf(x.MimeType) && FileValidator.IsValiBytes(x.PdfConvertedContent)));
            }
            else
            {
                Attachments = pagedData.Where(x => FileValidator.IsImage(x.MimeType) || FileValidator.IsPdf(x.MimeType) || FileValidator.IsTextplain(x.MimeType));
            }
            AttachmentIds = Attachments.Select(x => x.Id).ToList();
            HandleDisabled();
        }
        private void HandleDisabled()
        {
            if (AttachmentIds.Count == 1)
            {
                _disabledForwards = true;
                _disabledBackwards = true;
                _textBackwards = "";
                _textForwards = "";
                return;
            }

            _disabledForwards = false;
            _disabledBackwards = false;
            _textBackwards = "";
            _textForwards = "";
            var currentId = Attachment.First().Key;
            int index = AttachmentIds.FindIndex(a => a == currentId);
            var lastItem = AttachmentIds.ElementAtOrDefault(index + 1);
            if (lastItem == Guid.Empty)
            {
                _disabledForwards = true;
                _textForwards = "";
                _disabledBackwards = false;
                _textBackwards = (!FileValidator.IsPdf(Attachments.ElementAt(index - 1).MimeType) && FileValidator.IsValiBytes(Attachments.ElementAt(index - 1).PdfConvertedContent)) ? Attachments.ElementAt(index - 1).Name.Split(".")[0] + ".pdf" : Attachments.ElementAt(index - 1).Name;
                StateHasChanged();
                _fileDialog?.CallStateHasChanged();
                MudDialog.StateHasChanged();
                return;
            }

            var FirstItem = AttachmentIds.ElementAtOrDefault(index - 1);
            if (FirstItem == Guid.Empty)
            {
                _disabledForwards = false;
                _disabledBackwards = true;
                _textBackwards = "";
                _textForwards = (!FileValidator.IsPdf(Attachments.ElementAt(index + 1).MimeType) && FileValidator.IsValiBytes(Attachments.ElementAt(index + 1).PdfConvertedContent)) ? Attachments.ElementAt(index + 1).Name.Split(".")[0] + ".pdf" : Attachments.ElementAt(index + 1).Name;
                StateHasChanged();
                _fileDialog?.CallStateHasChanged();
                MudDialog.StateHasChanged();
                return;
            }
            _textBackwards = (!FileValidator.IsPdf(Attachments.ElementAt(index - 1).MimeType) && FileValidator.IsValiBytes(Attachments.ElementAt(index - 1).PdfConvertedContent)) ? Attachments.ElementAt(index - 1).Name.Split(".")[0] + ".pdf" : Attachments.ElementAt(index - 1).Name;
            _textForwards = (!FileValidator.IsPdf(Attachments.ElementAt(index + 1).MimeType) && FileValidator.IsValiBytes(Attachments.ElementAt(index + 1).PdfConvertedContent)) ? Attachments.ElementAt(index + 1).Name.Split(".")[0] + ".pdf" : Attachments.ElementAt(index + 1).Name;
            StateHasChanged();
            _fileDialog?.CallStateHasChanged();
            MudDialog.StateHasChanged();
        }
        private async void Move(Direction direction, int? indexparam = null)
        {
            if (LoadingState.RequestInProgress || _RequestInProgress) return;
            var currentId = Attachment.First().Key;
            _currentFileIndex = indexparam ?? AttachmentIds.FindIndex(a => a == currentId);

            var id = direction == Direction.Forwards ? AttachmentIds.ElementAtOrDefault(_currentFileIndex.Value + 1) : AttachmentIds.ElementAtOrDefault(_currentFileIndex.Value - 1);

            if (id != Guid.Empty)
            {
                var attach = Attachments.First(x => x.Id == id);
                var response = await GetAttachmentDownloadDtoFileView(id);
                if (!string.IsNullOrEmpty(response.Name))
                {
                    if (FileValidator.IsValideImage(response.Content) || FileValidator.IsValidePdf(response.Content) || (!FileValidator.IsPdf(response.MineType) && FileValidator.IsValiBytes(response.PdfConvertedContent)))
                    {
                        Attachment = new Dictionary<Guid, AttachmentDownloadDto> { { id, response } };
                    }
                    else
                    {
                        Move(direction, direction == Direction.Forwards ? _currentFileIndex + 1 : _currentFileIndex - 1);
                        return;
                    }
                }
                SetFile();
                HandleDisabled();
            }
        }
        public async void DownloadFromFileView()
        {
            if (LoadingState.RequestInProgress || _RequestInProgress) return;
            _RequestInProgress = true;
            LoadingState.SetRequestInProgress(true);
            _fileDialog?.CallStateHasChanged();
            MudDialog.StateHasChanged();
            if (!string.IsNullOrEmpty(Attachment.First().Value.Name))
            {
                if (FileValidator.IsValiBytes(Attachment.First().Value.PdfConvertedContent))
                {
                    var name = Attachment.First().Value.Name.Split(".")[0] + ".pdf";
                    await FileService.Download(name, Attachment.First().Value.PdfConvertedContent);
                }
                else
                {
                    await FileService.Download(Attachment.First().Value.Name, Attachment.First().Value.Content);
                }
            }
            _RequestInProgress = false;
            LoadingState.SetRequestInProgress(false);
            _fileDialog?.CallStateHasChanged();
            MudDialog.StateHasChanged();
        }
        private async void SetFile()
        {
            _fileName = (!FileValidator.IsPdf(Attachment.First().Value.MineType) && FileValidator.IsValiBytes(Attachment.First().Value.PdfConvertedContent)) ? Attachment.First().Value.Name.Split(".")[0] + ".pdf" : Attachment.First().Value.Name;
            _fileMimeType = (!FileValidator.IsPdf(Attachment.First().Value.MineType) && FileValidator.IsValiBytes(Attachment.First().Value.PdfConvertedContent)) ? FileValidator.pdf : Attachment.First().Value.MineType;

            _fileDialog.Dismiss(DialogResult.Cancel());
            _fileDialog.DialogReference.Dismiss(DialogResult.Cancel());
            _fileDialog.DialogComponent.Cancel();
            _fileDialog.CallStateHasChanged();

            _fileStream.Dispose();
            _fileStream.Close();
            var fileAttach = pagedData.First(x => x.Id == Attachment.First().Key);
            await ShowDocumentInFrame(fileAttach);
        }
        private async Task<AttachmentDownloadDto> GetAttachmentDownloadDtoFileView(Guid id)
        {
            AttachmentDownloadDto item = new();
            if (FileValidator.IsValiBytes(Attachments.First(x => x.Id == id).Content))
            {
                if (!FileValidator.IsPdf(Attachments.First(x => x.Id == id).MimeType) && FileValidator.IsValiBytes(Attachments.First(x => x.Id == id).PdfConvertedContent))
                {
                    item = new AttachmentDownloadDto
                    {
                        Name = Attachments.First(x => x.Id == id).Name.Split(".")[0] + ".pdf",
                        Content = Attachments.First(x => x.Id == id).PdfConvertedContent,
                        PdfConvertedContent = Attachments.First(x => x.Id == id).PdfConvertedContent,
                        MineType = FileValidator.pdf
                    };
                }
                else
                {
                    item = new AttachmentDownloadDto
                    {
                        Name = Attachments.First(x => x.Id == id).Name,
                        Content = Attachments.First(x => x.Id == id).Content,
                        MineType = Attachments.First(x => x.Id == id).MimeType
                    };
                }
            }
            else
            {
                LoadingState.SetRequestInProgress(true);
                _RequestInProgress = true;
                _fileDialog?.CallStateHasChanged();
                MudDialog.StateHasChanged();
                var attachmentResponse = await AttachmentService.AttachmentDownload(id);
                if (attachmentResponse.Success)
                {
                    item = attachmentResponse.Entity ?? new AttachmentDownloadDto();
                    Attachments.First(x => x.Id == id).Content = item.Content;
                }
                else
                {
                    Snackbar.Add(attachmentResponse.Message, Severity.Error);
                }
                LoadingState.SetRequestInProgress(false);
                _RequestInProgress = false;
                _fileDialog?.CallStateHasChanged();
                MudDialog.StateHasChanged();
            }
            return item;
        }
        private async Task<AttachmentDownloadDto> GetAttachmentDownloadDto(FileAttachmentDto attachment)
        {
            AttachmentDownloadDto item = new();
            _RequestInProgress = true;
            _fileDialog?.CallStateHasChanged();
            MudDialog.StateHasChanged();
            DisabledPage();

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
            MudDialog.StateHasChanged();
            _fileDialog?.CallStateHasChanged();
            DisabledPage();
            return item;
        }
        private async void Download(FileAttachmentDto attachment)
        {
            var item = await GetAttachmentDownloadDto(attachment);
            if (!string.IsNullOrEmpty(item.Name))
            {
                await FileService.Download(item.Name, item.Content);
            }
            _fileDialog?.CallStateHasChanged();
            MudDialog.StateHasChanged();
        }
        private bool DisabledUploadFiles()
        {
            return Entity.Deleted || !HasPermission || _RequestInProgress;
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
            var options = new DialogOptionsEx
            {
                BackgroundClass = "tz-mud-overlay",
                BackdropClick = false,
                CloseButton = false,
                MaxWidth = MaxWidth.Small,
                MaximizeButton = true,
                FullHeight = false,
                FullWidth = true,
                DragMode = MudDialogDragMode.Simple,
                Animations = [AnimationType.Pulse],
                Position = DialogPosition.Center
            };

            options.SetProperties(ex => ex.Resizeable = true);
            options.DialogAppearance = MudExAppearance.FromStyle(b =>
            {
                b.WithBackgroundImage("url('/images/plain-white-background.jpg')")
              .WithBackgroundSize("cover")
              .WithBackgroundPosition("center center")
              .WithBackgroundRepeat("no-repeat")
              .WithOpacity(0.9);
            });

            var dialog = await DialogService.ShowEx<DeleteEntityDialog>(Translate(SystemTextId.Delete), parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                _RequestInProgress = true;
                DisabledPage();
                _fileDialog?.CallStateHasChanged();
                MudDialog.StateHasChanged();

                var deleteResponse = await AttachmentService.AttachmentDelete(attachment.Id, Entity.Id, Source);
                if (deleteResponse.Success)
                {
                    Entity.Attachments.RemoveAll(x => x.Id == attachment.Id);
                    _fileDialog?.CallStateHasChanged();
                    MudDialog.StateHasChanged();
                }
                else
                {
                    Snackbar.Add(deleteResponse.Message, Severity.Error);
                }
                _RequestInProgress = false;
                DisabledPage();
                await _table.ReloadServerData();
                _fileDialog?.CallStateHasChanged();
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
                DisabledPage();
                _fileDialog?.CallStateHasChanged();
                MudDialog.StateHasChanged();
                foreach (var file in e.GetMultipleFiles())
                {
                    if (!IsValideFile(file))
                    {
                        _RequestInProgress = false;
                        DisabledPage();
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
                        DisabledPage();
                        _fileDialog?.CallStateHasChanged();
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
                            _filesToSend.Clear();
                        }
                        else
                        {
                            Snackbar.Add(attachmentsResponse.Message, Severity.Error);
                        }
                    }
                }
                _RequestInProgress = false;
                DisabledPage();
                await _table.ReloadServerData();
                _fileDialog?.CallStateHasChanged();
                MudDialog.StateHasChanged();
            }
            catch (Exception)
            {
                _files.Clear();
            }
        }
    }
    public enum Direction
    {
        None = 0,
        Forwards = 1,
        Backwards = 2
    }
}