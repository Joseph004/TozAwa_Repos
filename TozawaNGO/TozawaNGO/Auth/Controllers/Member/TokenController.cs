using Blazored.LocalStorage;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TozawaNGO.Auth.Models.Authentication;
using TozawaNGO.Auth.Models.Dtos;
using TozawaNGO.Auth.Models.Queries;
using TozawaNGO.Auth.Services;
using TozawaNGO.Helpers;

namespace TozawaNGO.Auth.Controllers
{
    [Route("api/[controller]")]
    [AuthorizeUserRequirementWithNoExpireToken]
    [Produces("application/json")]
    [ApiController]
    public class TokenController(UserManager<ApplicationUser> userManager, IMediator mediator, IUserTokenService userTokenService) : ControllerBase
    {
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

            return Ok(new LoginResponseDto { Token = token, RefreshToken = user.RefreshToken, LoginSuccess = true });
        }
    }
}