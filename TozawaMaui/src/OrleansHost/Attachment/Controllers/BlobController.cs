using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrleansHost.Attachment.Models.Commands;
using OrleansHost.Attachment.Models.Queries;
using Grains.Auth.Controllers;
using Grains.Auth.Models.Dtos;
using Grains.Auth.Services;
using Grains.Services;
using Grains.Helpers;

namespace OrleansHost.Attachment.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [AuthorizeUserRequirement]
    [EnableCors("TozAwaCorsPolicyBff")]
    public class BlobController(IMediator mediator, Grains.Auth.Services.ICurrentUserService currentUserService, IUserTokenService userTokenService) : InitController(mediator, currentUserService, userTokenService)
    {
        [HttpGet, Route("{id}"), CheckRole(RoleDto.President, RoleDto.VicePresident)]
        public async Task<IActionResult> Get(Guid id) => Ok(await _mediator.Send(new GetBlobQuery(id)));

        [HttpPost, Route(""), CheckRole(RoleDto.President, RoleDto.VicePresident)]
        public async Task<IActionResult> Post([FromForm] IFormFile file) => Ok(await _mediator.Send(new AddBlobCommand(file)));

        [HttpPost, Route("ConvertImageToPng"), CheckRole(RoleDto.President, RoleDto.VicePresident)]
        public async Task<IActionResult> ConvertImageToPng([FromBody] byte[] bytes) => Ok(await _mediator.Send(new ConvertImageToPngCommand(bytes)));
    }
}

