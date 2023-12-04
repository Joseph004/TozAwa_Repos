using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TozawaNGO.Context;
using TozawaNGO.Helpers;
using TozawaNGO.Services;
using TozawaNGO.Helpers;
using TozawaNGO.Auth.Controllers;
using TozawaNGO.Auth.Services;
using TozawaNGO.Auth.Models.Dtos;
using TozawaNGO.Attachment.Models.Commands;
using TozawaNGO.Models.Enums;
using TozawaNGO.Attachment.Models.Queries;
using TozawaNGO.Attachment.Models.Dtos;

namespace TozawaNGO.Attachment.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[EnableCors("TozAwaCorsPolicyBff")]
[Produces("application/json")]
[Route("api/[controller]")]
public class FileAttachmentController : InitController
{
    public FileAttachmentController(IMediator mediator, TozawaNGO.Auth.Services.ICurrentUserService currentUserService, IUserTokenService userTokenService) : base(mediator, currentUserService, userTokenService)
    {
    }

    [HttpPost, Route(""), CheckRole(RoleDto.President, RoleDto.VicePresident)]
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

        if (!FileValidator.IsValideFile(request))
        {
            return Ok(StatusCode(500));
        }

        return Ok(await _mediator.Send(request));
    }

    [HttpGet, Route("owner/{ownerId}"), CheckRole(RoleDto.President, RoleDto.VicePresident)]
    public async Task<IActionResult> GetAttachments(Guid ownerId) => Ok(await _mediator.Send(new GetAttachmentsQuery { OwnerId = ownerId }));

    [HttpPost, Route("owners"), CheckRole(RoleDto.President, RoleDto.VicePresident)]
    public Task<IEnumerable<AnalyseFileAttachments>> GetAttachmentsByOwnerIds([FromBody] GetAttachmentsByOwnerIdsQuery request) => _mediator.Send(request);

    [HttpGet, Route("{id}"), CheckRole(RoleDto.President, RoleDto.VicePresident)]
    public async Task<IActionResult> Get(Guid id) => Ok(await _mediator.Send(new GetAttachmentQuery(id)));

    [HttpPut, Route(""), CheckRole(RoleDto.President, RoleDto.VicePresident)]
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

    [HttpDelete, Route("{id}"), CheckRole(RoleDto.President, RoleDto.VicePresident)]
    public async Task<IActionResult> DeleteAttachment(Guid id)
    {
        await _mediator.Send(new DeleteAttachmentCommand { Id = id });
        return NoContent();
    }

    [HttpGet, Route("getAttachments/{ownerId}/{attachmentType}"), CheckRole(RoleDto.President, RoleDto.VicePresident)]
    public async Task<IActionResult> GetAttachmentsByFileType(Guid ownerId, AttachmentType attachmentType) => Ok(await _mediator.Send(new GetAttachmentsByFileTypeQuery { OwnerId = ownerId, AttachementType = attachmentType }));
}
