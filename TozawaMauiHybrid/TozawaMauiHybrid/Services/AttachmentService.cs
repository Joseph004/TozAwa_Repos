using MudBlazor;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.HttpClients;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.FormModels;
using TozawaMauiHybrid.Models.Requests;
using TozawaMauiHybrid.Models.ResponseRequests;

namespace TozawaMauiHybrid.Services;

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

        if (attachmentDtos != null)
        {
            NotifyStateChanged();
        }
    }
    private void NotifyStateChanged() => OnChange.Invoke();

    public async Task<AddResponse<TableData<FileAttachmentDto>>> GetAttachments(GetAttachments request) => await _client.SendPost02<TableData<FileAttachmentDto>>($"{_baseUriPath}", request);
    public async Task<DeleteResponse> AttachmentDelete(Guid id, Guid ownerId, string source)
    {
        return await _client.SendDelete<AttachmentDownloadDto>($"{_baseUriPath}/{id}/{ownerId}/{source}");
    }
    public async Task<GetResponse<AttachmentDownloadDto>> AttachmentDownload(Guid id)
    {
        var response = await _client.SendGet<AttachmentDownloadDto>($"{_baseUriPath}/{id}");
        return response;
    }
    public async Task<AddResponse<List<FileAttachmentDto>>> AttachmentUpload(Guid id, AttachmentUploadRequest request) => await _client.SendPost<List<FileAttachmentDto>>($"{_baseUriPath}/{id}", request);
    public async Task<byte[]> ConvertImageToPng(byte[] bytes) => await FileValidator.ImageToPng(bytes);
    public async Task<byte[]> ConvertToPdf(byte[] bytes, string conentType) => await FileValidator.ConvertToPdf(bytes, conentType);
}

