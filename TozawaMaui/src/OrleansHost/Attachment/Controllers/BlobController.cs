using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrleansHost.Attachment.Models.Commands;
using OrleansHost.Attachment.Models.Queries;
using Grains.Auth.Controllers;
using Grains.Auth.Services;
using Grains.Helpers;
using Grains.Models;

namespace OrleansHost.Attachment.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [AuthorizeUserRequirement]
    [EnableCors("TozAwaCorsPolicyBff")]
    public class BlobController(IMediator mediator, Grains.Auth.Services.ICurrentUserService currentUserService, IUserTokenService userTokenService, IGrainFactory factory) : InitController(mediator, currentUserService, userTokenService, factory)
    {
        [HttpGet, Route("{id}"), CheckRole(FunctionType.ReadAttachment)]
        public async Task<IActionResult> Get(Guid id) => Ok(await _mediator.Send(new GetBlobQuery(id)));

        [HttpPost, Route(""), CheckRole(FunctionType.WriteAttachment, FunctionType.WriteAdmin, FunctionType.WriteLandLoard)]
        public async Task<IActionResult> Post([FromForm] IFormFile file) => Ok(await _mediator.Send(new AddBlobCommand(file)));

        [HttpPost, Route("ConvertImageToPng"), CheckRole(FunctionType.WriteAttachment)]
        public async Task<IActionResult> ConvertImageToPng([FromBody] byte[] bytes) => Ok(await _mediator.Send(new ConvertImageToPngCommand(bytes)));
    }
}

