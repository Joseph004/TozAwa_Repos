using ShareRazorClassLibrary.HttpClients;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.FormModels;
using ShareRazorClassLibrary.Models.Requests;
using ShareRazorClassLibrary.Models.ResponseRequests;

namespace ShareRazorClassLibrary.Services;

public class AttachmentService(ITozAwaBffHttpClient client)
{
    private readonly ITozAwaBffHttpClient _client = client;
    private const string _baseUriPath = $"fileattachment";

    public event Action OnChange;
    public OwnerAttachments FileAttachmentDtos = new();
    public bool OnDelete = false;
    public void SetNotifyChange(OwnerAttachments attachmentDtos, bool onDelete = false)
    {
        OnDelete = onDelete;
        FileAttachmentDtos = attachmentDtos;

        NotifyStateChanged();
    }
    private void NotifyStateChanged() => OnChange.Invoke();

    public async Task<AddResponse<List<FileAttachmentDto>>> GetAttachments(GetAttachments request) => await _client.SendPost02<List<FileAttachmentDto>>($"{_baseUriPath}", request);
    public async Task<DeleteResponse> AttachmentDelete(Guid id, Guid ownerId, string source) => await _client.SendDelete<AttachmentDownloadDto>($"{_baseUriPath}/{id}/{ownerId}/{source}");
    public async Task<GetResponse<AttachmentDownloadDto>> AttachmentDownload(Guid id) => await _client.SendGet<AttachmentDownloadDto>($"{_baseUriPath}/{id}");
    public async Task<AddResponse<List<FileAttachmentDto>>> AttachmentUpload(Guid id, AttachmentUploadRequest request) => await _client.SendPost<List<FileAttachmentDto>>($"{_baseUriPath}/{id}", request);
}

