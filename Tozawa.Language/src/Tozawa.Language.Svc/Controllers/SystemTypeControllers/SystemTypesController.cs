using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Tozawa.Language.Svc.extension;
using Tozawa.Language.Svc.Models.Enums;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Controllers.SystemTypeControllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SystemTypesController : InitController
    {
        public SystemTypesController(IMediator mediator, ICurrentUserService currentUserService)
         : base(mediator, currentUserService)
        {
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> Get() => Ok(await _mediator.Send(new GetSystemTypesQuery()));

        [HttpGet, Route("paged")]
        public async Task<IActionResult> GetPaged() => Ok(await _mediator.Send(new GetSystemTypesPagedQuery(Request.QueryString.HasValue ? QueryHelpers.ParseQuery(Request.QueryString.Value) : null)));

        [HttpPut, Route("default"), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> SetDefault([FromBody] SetSystemTypeAsDefaultCommand request) => Ok(await _mediator.Send(request));

        // Not used by Tozawa.Language Client

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetSystemType(Guid id) => Ok(await _mediator.Send(new GetSystemTypeQuery(id)));

        [HttpPost, Route(""), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> Create([FromBody] CreateSystemTypeCommand request) => Ok(await _mediator.Send(request));

        [HttpDelete, Route("{id}"), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> Delete(Guid id) => Ok(await _mediator.Send(new DeleteSystemTypeCommand() { Id = id }));
    }
}