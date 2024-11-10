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
        private List<FileAttachmentDto> _value;
        [Parameter]
        public List<FileAttachmentDto> Attachments
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
        public EventCallback<List<FileAttachmentDto>> AttachmentsChanged { get; set; }

        public List<Guid> AttachmentIds { get; set; }
        private bool _disabledForwards = false;
        private bool _disabledBackwards = false;
        private bool _disabledPage = false;
        private bool _showImage = false;
        private bool _showFile = false;
        private string _disableAttrString = "";
        private void DisabledPage()
        {
            _disabledPage = LoadingState.RequestInProgress;

            _disableAttrString = _disabledPage ? "pointer-events: none;" : "";
            StateHasChanged();
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
                item = new AttachmentDownloadDto
                {
                    Name = Attachments.First(x => x.Id == id).Name,
                    Content = Attachments.First(x => x.Id == id).Content,
                    MineType = Attachments.First(x => x.Id == id).MimeType
                };
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
                return;
            }

            _disabledForwards = false;
            _disabledBackwards = false;
            var currentId = Attachment.First().Key;
            int index = AttachmentIds.FindIndex(a => a == currentId);
            var lastItem = AttachmentIds.ElementAtOrDefault(index + 1);
            if (lastItem == Guid.Empty)
            {
                _disabledForwards = true;
                _disabledBackwards = false;
                return;
            }
            var FirstItem = AttachmentIds.ElementAtOrDefault(index - 1);
            if (FirstItem == Guid.Empty)
            {
                _disabledForwards = false;
                _disabledBackwards = true;
                return;
            }
            StateHasChanged();
        }
        private async Task Move(Direction direction)
        {
            var currentId = Attachment.First().Key;
            int index = AttachmentIds.FindIndex(a => a == currentId);

            var id = direction == Direction.Forwards ? AttachmentIds.ElementAtOrDefault(index + 1) : AttachmentIds.ElementAtOrDefault(index - 1);

            if (id != Guid.Empty)
            {
                var response = await GetAttachmentDownloadDto(id);
                if (!string.IsNullOrEmpty(response.Name))
                {
                    Attachment = new Dictionary<Guid, AttachmentDownloadDto> { { id, response } };
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
        private async Task SetFile()
        {
            _showFile = !FileValidator.IsImage(Attachment.First().Value.MineType);
            _showImage = FileValidator.IsImage(Attachment.First().Value.MineType);
            StateHasChanged();

            var stream = GetStream(Attachment.First().Value.Content);
            await FileService.ShowFile(stream, FileValidator.IsImage(Attachment.First().Value.MineType) ? "avatar" : "iframe", Attachment.First().Value.MineType, Attachment.First().Value.Name);
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