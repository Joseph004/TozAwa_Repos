using Microsoft.AspNetCore.Mvc;
using Grains;
using System.Collections.Immutable;
using Asp.Versioning;
using Grains.Helpers;
using Grains.Auth.Controllers;
using Grains.Auth.Services;
using MediatR;
using Grains.Auth.Models.Dtos;
using Grains.Models;

namespace OrleansHost.Api
{
    [AuthorizeUserRequirement]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/[controller]")]
    public class WeatherController(IGrainFactory factory, IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService) : InitController(mediator, currentUserService, userTokenService, factory)
    {
        private readonly IGrainFactory _factory = factory;

        [HttpGet, Route(""), CheckRole(FunctionType.ReadCashier)]
        public async Task<IActionResult> GetAsync()
         => Ok(await _factory.GetGrain<IWeatherGrain>(Guid.Empty).GetForecastAsync());
    }
}