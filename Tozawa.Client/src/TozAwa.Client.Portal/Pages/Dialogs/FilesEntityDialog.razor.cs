using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Tozawa.Client.Portal.HttpClients.Helpers;
using Tozawa.Client.Portal.Models;
using Tozawa.Client.Portal.Models.Dtos;
using Tozawa.Client.Portal.Models.Enums;
using Tozawa.Client.Portal.Models.FormModels;
using Tozawa.Client.Portal.Services;
using Tozawa.Client.Portal.Shared;
using TozAwa.Client.Portal.Services;

namespace Tozawa.Client.Portal.Pages
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
        private readonly List<IBrowserFile> _files = new();

        protected void Add() => MudDialog.Close(DialogResult.Ok(Entity));
        protected void Cancel() => MudDialog.Cancel();

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

            if (!result.Cancelled)
            {
                _onProgress = true;
                StateHasChanged();

                var deleteResponse = await AttachmentService.AttachmentDelete(attachment.Id);
                if (deleteResponse.Success)
                {
                    Entity.Attachments.Remove(attachment);
                    StateHasChanged();
                    AttachmentService.SetNotifyChange();
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
            _onProgress = true;
            StateHasChanged();
            foreach (var file in e.GetMultipleFiles())
            {
                if (Entity.Attachments.Any(x => x.Name == file.Name && x.MimeType == file.ContentType))
                {
                    Snackbar.Add($"{Translate(SystemTextId.FileName, "Filename")} {file.Name.ToUpper()} {Translate(SystemTextId.AlreadyExists, "already exist")} : {Translate(SystemTextId.Name, "Name")} {Entity.Code}", Severity.Warning);
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
                long maxSize = 10 * 1024 * 1024;

                if (_files.Any(x => x.Size > maxSize))
                {
                    Snackbar.Add($"{Translate(SystemTextId.TheAllowedMaximumSizeIs, "The Allowed maximum size is")} {Math.Round(Convert.ToDouble(maxSize) / 1000, 2)}KB", Severity.Warning);
                    _files.Clear();
                }
                else
                {
                    await request.AddFiles(_files);
                    request.FileAttachmentType = _attachmentType;
                    request.FolderName = Entity.Email;
                    var attachmentsResponse = await AttachmentService.AttachmentUpload(Entity.Id, request);
                    if (attachmentsResponse.Success)
                    {
                        Entity.Attachments.AddRange(attachmentsResponse.Entity ?? new List<FileAttachmentDto>());
                        _files.Clear();
                        AttachmentService.SetNotifyChange();
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
    }
}