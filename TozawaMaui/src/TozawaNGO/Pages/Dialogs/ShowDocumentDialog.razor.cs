using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Services;
using TozawaNGO.Services;
using TozawaNGO.Shared;

namespace TozawaNGO.Pages
{
    public partial class ShowDocumentDialog : BaseDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] protected FileService FileService { get; set; }
        [Inject] protected AttachmentService AttachmentService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] private LoadingState LoadingState { get; set; }
        [Parameter] public Dictionary<Guid, AttachmentDownloadDto> Attachment { get; set; }
        private IEnumerable<FileAttachmentDto> _value;
        [Parameter]
        public IEnumerable<FileAttachmentDto> Attachments
        {
            get => _value;
            set
            {
                if (_value == value) return;
                _value = value;
                AttachmentsChanged.InvokeAsync(value);
            }
        }
        [Parameter]
        public EventCallback<IEnumerable<FileAttachmentDto>> AttachmentsChanged { get; set; }

        public List<Guid> AttachmentIds { get; set; }
        private bool _disabledForwards = false;
        private bool _disabledBackwards = false;
        private string _textForwards = string.Empty;
        private string _textBackwards = string.Empty;
        private bool _disabledPage = false;
        private bool _showImage = false;
        private bool _showFile = false;
        private string _disableAttrString = "";
        private void DisabledPage()
        {
            _disabledPage = LoadingState.RequestInProgress;

            _disableAttrString = _disabledPage ? "pointer-events: none;" : "";
            StateHasChanged();
            MudDialog.StateHasChanged();
        }
        private Stream GetStream(byte[] content)
        {
            return new MemoryStream(content);
        }
        private async Task<AttachmentDownloadDto> GetAttachmentDownloadDto(Guid id)
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
            }
            return item;
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
                MudDialog.StateHasChanged();
                return;
            }
            _textBackwards = (!FileValidator.IsPdf(Attachments.ElementAt(index - 1).MimeType) && FileValidator.IsValiBytes(Attachments.ElementAt(index - 1).PdfConvertedContent)) ? Attachments.ElementAt(index - 1).Name.Split(".")[0] + ".pdf" : Attachments.ElementAt(index - 1).Name;
            _textForwards = (!FileValidator.IsPdf(Attachments.ElementAt(index + 1).MimeType) && FileValidator.IsValiBytes(Attachments.ElementAt(index + 1).PdfConvertedContent)) ? Attachments.ElementAt(index + 1).Name.Split(".")[0] + ".pdf" : Attachments.ElementAt(index + 1).Name;
            StateHasChanged();
            MudDialog.StateHasChanged();
        }
        private async Task Move(Direction direction, int? indexparam = null)
        {
            var currentId = Attachment.First().Key;
            int index = indexparam ?? AttachmentIds.FindIndex(a => a == currentId);

            var id = direction == Direction.Forwards ? AttachmentIds.ElementAtOrDefault(index + 1) : AttachmentIds.ElementAtOrDefault(index - 1);

            if (id != Guid.Empty)
            {
                var attach = Attachments.First(x => x.Id == id);

                var response = await GetAttachmentDownloadDto(id);
                if (!string.IsNullOrEmpty(response.Name))
                {
                    if (FileValidator.IsValideImage(response.Content) || FileValidator.IsValidePdf(response.Content) || (!FileValidator.IsPdf(response.MineType) && FileValidator.IsValiBytes(response.PdfConvertedContent)))
                    {
                        Attachment = new Dictionary<Guid, AttachmentDownloadDto> { { id, response } };
                    }
                    else
                    {
                        await Move(direction, direction == Direction.Forwards ? index + 1 : index - 1);
                        return;
                    }
                }
                await SetFile();
                HandleDisabled();
            }
        }
        public override void Dispose()
        {
            LoadingState.OnChange -= DisabledPage;
            base.Dispose();
        }
        protected override void OnInitialized()
        {
            LoadingState.OnChange += DisabledPage;
            AttachmentIds = Attachments.Select(x => x.Id).ToList();
            HandleDisabled();
        }
        private async void Download()
        {
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
            StateHasChanged();
            MudDialog.StateHasChanged();
        }
        private async Task SetFile()
        {
            var name = (!FileValidator.IsPdf(Attachment.First().Value.MineType) && FileValidator.IsValiBytes(Attachment.First().Value.PdfConvertedContent)) ? Attachment.First().Value.Name.Split(".")[0] + ".pdf" : Attachment.First().Value.Name;
            var mineType = (!FileValidator.IsPdf(Attachment.First().Value.MineType) && FileValidator.IsValiBytes(Attachment.First().Value.PdfConvertedContent)) ? FileValidator.pdf : Attachment.First().Value.MineType;
            _showFile = FileValidator.IsPdf(Attachment.First().Value.MineType) || (!FileValidator.IsPdf(Attachment.First().Value.MineType) && FileValidator.IsValiBytes(Attachment.First().Value.PdfConvertedContent));
            _showImage = FileValidator.IsImage(Attachment.First().Value.MineType);
            StateHasChanged();
            MudDialog.StateHasChanged();

            var stream = (!FileValidator.IsPdf(Attachment.First().Value.MineType) && FileValidator.IsValiBytes(Attachment.First().Value.PdfConvertedContent)) ? GetStream(Attachment.First().Value.PdfConvertedContent) : GetStream(Attachment.First().Value.Content);
            await FileService.ShowFile(stream, FileValidator.IsImage(mineType) ? "avatar" : "iframe", mineType, name, _showImage);
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SetFile();
            }
        }
        void Cancel() => MudDialog.Cancel();
    }
    public enum Direction
    {
        Forwards = 1,
        Backwards = 2,
    }
}