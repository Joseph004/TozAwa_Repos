using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Grains.Auth.Controllers;
using Grains.Auth.Services;
using OrleansHost.Attachment.Models.Commands;
using Grains.Models.Enums;
using OrleansHost.Attachment.Models.Queries;

namespace OrleansHost.Attachment.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
public class FileAttachmentController(IMediator mediator, Grains.Auth.Services.ICurrentUserService currentUserService, IUserTokenService userTokenService) : InitController(mediator, currentUserService, userTokenService)
{

    [HttpPost, Route("{id}")]
    public async Task<IActionResult> AddAttachment(Guid id, [FromBody] AddAttachmentCommand request)
    {
        request.Id = id;
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpGet, Route("owner/{ownerId}")]
    public async Task<IActionResult> GetAttachments(Guid ownerId) => Ok(await _mediator.Send(new GetAttachmentsQuery { OwnerId = ownerId }));

    [HttpPost, Route("owners")]
    public async Task<IActionResult> GetAttachmentsByOwnerIds([FromBody] GetAttachmentsByOwnerIdsQuery request) => Ok(await _mediator.Send(request));

    [HttpGet, Route("{id}")]
    public async Task<IActionResult> Get(Guid id) => Ok(await _mediator.Send(new GetAttachmentQuery(id)));

    [HttpPut, Route("")]
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

    [HttpDelete, Route("{id}/{source}")]
    public async Task<IActionResult> DeleteAttachment(Guid id, string source)
    {
        await _mediator.Send(new DeleteAttachmentCommand { Id = id, Source = source });
        return NoContent();
    }

    [HttpGet, Route("getAttachments/{ownerId}/{attachmentType}")]
    public async Task<IActionResult> GetAttachmentsByFileType(Guid ownerId, AttachmentType attachmentType) => Ok(await _mediator.Send(new GetAttachmentsByFileTypeQuery { OwnerId = ownerId, AttachementType = attachmentType }));
}
