using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Helpers;
using Tozawa.Attachment.Svc.Models.Commands;
using Tozawa.Attachment.Svc.Models.Dtos;
using Tozawa.Attachment.Svc.Models.Queries;
using Tozawa.Attachment.Svc.Services;

namespace Tozawa.Attachment.Svc.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[EnableCors("TozAwaCorsPolicyBff")]
[Produces("application/json")]
[Route("api/[controller]")]
public class FileAttachmentController : InitController
{
    public FileAttachmentController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService) : base(mediator, currentUserService, userTokenService)
    {
    }

    [HttpPost, Route(""), CheckRole(FunctionType.WriteTravling, FunctionType.WriteImmovable, FunctionType.WriteActivity)]
    public async Task<IActionResult> AddAttachment([FromForm] AddAttachmentCommand request)
    {
        var formValues = await Request.ReadFormAsync();

        if (formValues.TryGetValue("ownerIds", out var formOwnerIds))
        {
            request.OwnerIds = JsonConvert.DeserializeObject<List<Guid>>(formOwnerIds[0]);
        }
        if (formValues.TryGetValue("fileAttachmentType", out var formFileAttachmentType))
        {
            request.FileAttachmentType = JsonConvert.DeserializeObject<AttachmentType>(formFileAttachmentType);
        }
        if (formValues.TryGetValue("blobId", out var formBlobId))
        {
            request.BlobId = JsonConvert.DeserializeObject<string>(formBlobId);
        }
        if (formValues.TryGetValue("extension", out var formExtension))
        {
            request.Extension = JsonConvert.DeserializeObject<string>(formExtension);
        }
        if (formValues.TryGetValue("mimeType", out var formMimeType))
        {
            request.MimeType = JsonConvert.DeserializeObject<string>(formMimeType);
        }
        if (formValues.TryGetValue("miniatureId", out var formMiniatureId))
        {
            request.MiniatureId = JsonConvert.DeserializeObject<string>(formMiniatureId);
        }
        if (formValues.TryGetValue("name", out var formName))
        {
            request.Name = JsonConvert.DeserializeObject<string>(formName);
        }
        if (formValues.TryGetValue("size", out var formSize))
        {
            request.Size = JsonConvert.DeserializeObject<double>(formSize);
        }
        if (formValues.TryGetValue("metaData", out var formMetaData))
        {
            request.MetaData = formMetaData;
        }

        return Ok(await _mediator.Send(request));
    }

    [HttpGet, Route("owner/{ownerId}"), CheckRole(FunctionType.ReadTravling, FunctionType.ReadImmovable, FunctionType.ReadActivity)]
    public async Task<IActionResult> GetAttachments(Guid ownerId) => Ok(await _mediator.Send(new GetAttachmentsQuery { OwnerId = ownerId }));

    [HttpPost, Route("owners"), CheckRole(FunctionType.ReadTravling, FunctionType.ReadImmovable, FunctionType.ReadActivity)]
    public Task<IEnumerable<TravlingFileAttachments>> GetAttachmentsByOwnerIds([FromBody] GetAttachmentsByOwnerIdsQuery request) => _mediator.Send(request);

    [HttpGet, Route("{id}"), CheckRole(FunctionType.ReadTravling, FunctionType.ReadImmovable, FunctionType.ReadActivity)]
    public async Task<IActionResult> Get(Guid id) => Ok(await _mediator.Send(new GetAttachmentQuery(id)));

    [HttpPut, Route(""), CheckRole(FunctionType.WriteTravling, FunctionType.WriteImmovable, FunctionType.WriteActivity)]
    public async Task<IActionResult> UpdateAttachment([FromForm] UpdateAttachmentCommand request)
    {
        var formValues = await Request.ReadFormAsync();

        if (formValues.TryGetValue("ownerIds", out var formOwnerIds))
        {
            request.OwnerIds = JsonConvert.DeserializeObject<List<Guid>>(formOwnerIds[0]);
        }
        if (formValues.TryGetValue("fileAttachmentType", out var formFileAttachmentType))
        {
            request.FileAttachmentType = JsonConvert.DeserializeObject<AttachmentType>(formFileAttachmentType);
        }
        if (formValues.TryGetValue("metaData", out var formMetaData))
        {
            request.MetaData = formMetaData;
        }
        if (formValues.TryGetValue("id", out var formId))
        {
            request.Id = JsonConvert.DeserializeObject<Guid>(formId);
        }

        return Ok(await _mediator.Send(request));
    }

    [HttpDelete, Route("{id}"), CheckRole(FunctionType.WriteTravling, FunctionType.WriteImmovable, FunctionType.WriteActivity)]
    public async Task<IActionResult> DeleteAttachment(Guid id)
    {
        await _mediator.Send(new DeleteAttachmentCommand { Id = id });
        return NoContent();
    }

    [HttpGet, Route("getAttachments/{ownerId}/{attachmentType}"), CheckRole(FunctionType.ReadTravling, FunctionType.ReadImmovable, FunctionType.ReadActivity)]
    public async Task<IActionResult> GetAttachmentsByFileType(Guid ownerId, AttachmentType attachmentType) => Ok(await _mediator.Send(new GetAttachmentsByFileTypeQuery { OwnerId = ownerId, AttachementType = attachmentType }));
}
