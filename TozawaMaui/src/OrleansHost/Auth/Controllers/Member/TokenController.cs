using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos;
using OrleansHost.Auth.Models.Queries;
using Grains.Auth.Services;
using Grains.Helpers;

namespace Grains.Auth.Controllers
{
    [Route("api/[controller]")]
    [AuthorizeUserRequirementWithNoExpireToken]
    [Produces("application/json")]
    [ApiController]
    public class TokenController(UserManager<ApplicationUser> userManager, IMediator mediator, IUserTokenService userTokenService, IGrainFactory factory) : ControllerBase
    {
        private readonly IGrainFactory _factory = factory;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUserTokenService _userTokenService = userTokenService;
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto tokenDto)
        {
            if (tokenDto is null)
            {
                return BadRequest(new LoginResponseDto { LoginSuccess = false, ErrorMessage = "Invalid client request" });
            }
            var userDtoExisting = _userTokenService.GetUserFromExpiredToken(tokenDto.Token);

            var user = await _userManager.FindByNameAsync(userDtoExisting.UserName);
            if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest(new LoginResponseDto { LoginSuccess = false, ErrorMessage = "Invalid client request" });

            var userDto = await _mediator.Send(new GetCurrentUserQuery(user.UserId));
            var tokenOptions = _userTokenService.GenerateTokenOptions(userDto);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            user.RefreshToken = _userTokenService.GenerateRefreshToken();
            await _userManager.UpdateAsync(user);
            await _factory.GetGrain<ILoggedInGrain>(user.UserId).SetAsync(new LoggedInItem(user.UserId, token, user.RefreshToken, SystemTextId.LoggedInOwnerId));
            return Ok(new LoginResponseDto { Token = token, RefreshToken = user.RefreshToken, LoginSuccess = true });
        }

        [HttpPost]
        [Route("logout/{id}")]
        public async Task<IActionResult> Logout(Guid id)
        {
            await _factory.GetGrain<ILoggedInGrain>(id).ClearAsync();
            return Ok(new LoginResponseDto
            {
                LoginSuccess = true
            });
        }
    }
}