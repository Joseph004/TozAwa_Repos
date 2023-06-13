using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tozawa.Auth.Svc.Controllers.Login;
using Tozawa.Auth.Svc.Helper;
using Tozawa.Auth.Svc.Models;
using Tozawa.Auth.Svc.Models.Authentication;
using Tozawa.Auth.Svc.Models.Commands;
using Tozawa.Auth.Svc.Models.Dtos;
using Tozawa.Auth.Svc.Models.FormModels;
using Tozawa.Auth.Svc.Models.Queries;
using Tozawa.Auth.Svc.Services;

namespace Tozawa.Auth.Svc.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("TozAwaCorsPolicyBff")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserTokenService _userTokenService;
        private readonly IMediator _mediator;
        public TokenController(UserManager<ApplicationUser> userManager, IMediator mediator, IUserTokenService userTokenService)
        {
            _userManager = userManager;
            _userTokenService = userTokenService;
            _mediator = mediator;
        }
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