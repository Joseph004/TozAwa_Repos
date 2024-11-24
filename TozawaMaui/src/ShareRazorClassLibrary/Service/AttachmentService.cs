using System.Net;
using Blazored.SessionStorage;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.HttpClients;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.FormModels;
using ShareRazorClassLibrary.Models.Requests;
using ShareRazorClassLibrary.Models.ResponseRequests;

namespace ShareRazorClassLibrary.Services;

public class AttachmentService(ITozAwaBffHttpClient client, ISessionStorageService sessionStorageService)
{
    private readonly ITozAwaBffHttpClient _client = client;
    private readonly ISessionStorageService _sessionStorageService = sessionStorageService;
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
    public async Task<DeleteResponse> AttachmentDelete(Guid id, Guid ownerId, Guid blobId, string source)
    {
        if (await _sessionStorageService.ContainKeyAsync(blobId.ToString()))
        {
            await _sessionStorageService.RemoveItemAsync(blobId.ToString());
        }
        return await _client.SendDelete<AttachmentDownloadDto>($"{_baseUriPath}/{id}/{ownerId}/{source}");
    }
    public async Task<GetResponse<AttachmentDownloadDto>> AttachmentDownload(Guid id)
    {
        if (await _sessionStorageService.ContainKeyAsync(id.ToString()))
        {
            return new GetResponse<AttachmentDownloadDto>(true, "OK", HttpStatusCode.OK, await _sessionStorageService.GetItemAsync<AttachmentDownloadDto>(id.ToString()));
        }
        var response = await _client.SendGet<AttachmentDownloadDto>($"{_baseUriPath}/{id}");
        if (response.Success && response.Entity != null)
        {
            await _sessionStorageService.SetItemAsync(id.ToString(), response.Entity);
        }
        return response;
    }
    public async Task<AddResponse<List<FileAttachmentDto>>> AttachmentUpload(Guid id, AttachmentUploadRequest request) => await _client.SendPost<List<FileAttachmentDto>>($"{_baseUriPath}/{id}", request);
    public async Task<byte[]> ConvertImageToPng(byte[] bytes) => await FileValidator.ImageToPng(bytes);
    public async Task<byte[]> ConvertToPdf(byte[] bytes, string conentType) => await FileValidator.ConvertToPdf(bytes, conentType);
}

