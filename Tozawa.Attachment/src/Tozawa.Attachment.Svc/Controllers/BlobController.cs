using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Models.Commands;
using Tozawa.Attachment.Svc.Models.Queries;
using Tozawa.Attachment.Svc.Services;

namespace Tozawa.Attachment.Svc.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class BlobController : InitController
    {
        public BlobController(IMediator mediator, ICurrentUserService currentUserService) : base(mediator, currentUserService)
        {
        }

        [HttpGet, Route("{id}"), CheckRole(FunctionType.ReadImmovable, FunctionType.ReadTravling, FunctionType.ReadActivity)]
        public async Task<IActionResult> Get(Guid id) => Ok(await _mediator.Send(new GetBlobQuery(id)));

        [HttpPost, Route(""), CheckRole(FunctionType.ReadImmovable, FunctionType.ReadTravling, FunctionType.ReadActivity)]
        public async Task<IActionResult> Post([FromForm] IFormFile file) => Ok(await _mediator.Send(new AddBlobCommand(file)));

        [HttpPost, Route("ConvertImageToPng"), CheckRole(FunctionType.ReadImmovable, FunctionType.ReadTravling, FunctionType.ReadActivity)]
        public async Task<IActionResult> ConvertImageToPng(IFormFile file) => Ok(await _mediator.Send(new ConvertImageToPngCommand(file)));
    }
}

