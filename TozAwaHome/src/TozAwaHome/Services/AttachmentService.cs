using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TozAwaHome.HttpClients;
using TozAwaHome.Models.Dtos;
using TozAwaHome.Models.FormModels;
using TozAwaHome.Models.ResponseRequests;

namespace TozAwaHome.Services;

public class AttachmentService
{
    private readonly ITozAwaBffHttpClient _client;
    private const string _baseUriPath = $"attachment";
    public AttachmentService(ITozAwaBffHttpClient client) => _client = client;

    public event Action OnChange;
    public void SetNotifyChange()
    {
        NotifyStateChanged();
    }
    private void NotifyStateChanged() => OnChange?.Invoke();

    public async Task<DeleteResponse> AttachmentDelete(Guid id) => await _client.SendDelete<AttachmentDownloadDto>($"{_baseUriPath}/{id}");
    public async Task<GetResponse<AttachmentDownloadDto>> AttachmentDownload(Guid id) => await _client.SendGet<AttachmentDownloadDto>($"{_baseUriPath}/{id}");
    public async Task<AddResponse<List<FileAttachmentDto>>> AttachmentUpload(Guid id, AttachmentUploadRequest request) => await _client.SendPost<List<FileAttachmentDto>>($"{_baseUriPath}/{id}", request);
}

