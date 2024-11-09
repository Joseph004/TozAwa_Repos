using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Grains.Auth.Controllers.Login;
using Grains.Auth.Models;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos;
using Grains.Auth.Models.FormModels;
using OrleansHost.Auth.Models.Queries;
using Grains.Auth.Services;
using Grains.Models.ResponseRequests;
using System.Net;
using Grains.Context;
using Grains.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Grains.Helpers;
using OrleansHost.Auth.Controllers;

namespace Grains.Auth.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]

    [ApiController]
    public class AuthenticateController(UserManager<ApplicationUser> userManager, IGrainFactory factory, AppSettings appSettings, TozawangoDbContext context, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IUserTokenService userTokenService, IMediator mediator, IEncryptDecrypt encryptDecrypt, ILookupNormalizer normalizer) : ControllerBase
    {
        private readonly IGrainFactory _factory = factory;
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly RoleManager<IdentityRole> roleManager = roleManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMediator _mediator = mediator;
        private readonly IUserTokenService _userTokenService = userTokenService;
        private readonly TozawangoDbContext _context = context;
        private readonly AppSettings _appSettings = appSettings;
        private readonly IEncryptDecrypt _encryptDecrypt = encryptDecrypt;
        private readonly ILookupNormalizer _normalizer = normalizer;

        [HttpGet, Route("current/{oid:Guid}")]
        public async Task<IActionResult> GetCurrentUser(Guid oid) => Ok(await _mediator.Send(new GetCurrentUserQuery(oid)));

        [HttpPost, Route("signin")]
        public async Task<ActionResult> SignInPost([FromBody] LoginRequest request)
        {
            var response = new AddResponse<LoginResponseDto>(true, "Success", HttpStatusCode.OK, new LoginResponseDto
            {
                LoginSuccess = true,
                ErrorMessage = ""
            });

            var key = "Uj=?1PowK<ai57:t%`Ro]P1~1q2&-i?b";
            var iv = "Rh2nvSARdZRDeYiB";
            var pwdBytes = Cryptography.Decrypt(request.Content, key, iv);

            var pswd = _encryptDecrypt.DecryptUsingCertificate(Encoding.UTF8.GetString(pwdBytes));

            var command = new LoginCommand
            {
                Email = request.Email,
                Password = pswd
            };

            var validator = new LoginCommandFluentValidator();

            var requestValidate = await validator.ValidateAsync(command);

            if (!requestValidate.IsValid)
            {
                response.Success = false;
                response.Message = "Error";
                response.StatusCode = HttpStatusCode.BadRequest;

                response.Entity.LoginSuccess = false;
                response.Entity.ErrorMessage = requestValidate.Errors.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).FirstOrDefault().ErrorMessage;
                return Ok(response);
            }

            var user = await _mediator.Send(new GetApplicationUserQuery(command.Email));

            if (user == null)
            {
                response.Success = true;
                response.Message = "Error";
                response.StatusCode = HttpStatusCode.BadRequest;

                response.Entity.LoginSuccess = false;
                response.Entity.ErrorMessageGuid = Helpers.SystemTextId.EmailOrPasswordWrong;
                return Ok(response);
            }
            if (user.Deleted)
            {
                response.Success = false;
                response.Message = "Error";
                response.StatusCode = HttpStatusCode.BadRequest;

                response.Entity.LoginSuccess = false;
                response.Entity.ErrorMessageGuid = Helpers.SystemTextId.Unauthorized;
                return Ok(response);
            }

            var validPassword = await userManager.CheckPasswordAsync(user, command.Password);
            if (!validPassword)
            {
                response.Success = true;
                response.Message = "Error";
                response.StatusCode = HttpStatusCode.BadRequest;

                response.Entity.LoginSuccess = false;
                response.Entity.ErrorMessageGuid = Helpers.SystemTextId.EmailOrPasswordWrong;
                return Ok(response);
            }

            var userDto = await _mediator.Send(new GetCurrentUserQuery(user.UserId));

            var securityToken = _userTokenService.GenerateTokenOptions(userDto);
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            user.RefreshToken = _userTokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await userManager.UpdateAsync(user);

            response.Entity.Token = token;
            response.Entity.ExpiresIn = _appSettings.JWTSettings.ExpiryInMinutes;
            response.Entity.RefreshToken = user.RefreshToken;

            return Ok(response);
        }

        [HttpPost, Route("root/{userName}")]
        public async Task<ActionResult> CheckLockout(string userName)
        {
            var response = new LoginResponseDto
            {
                LoginSuccess = true,
                ErrorMessage = ""
            };
            var user = await userManager.FindByNameAsync(userName);
            if (user != null && user.AccessFailedCount == 3)
            {
                response.LoginSuccess = false;
                response.LoginAttemptCount = 3;
            }
            else if (user != null)
            {
                response.LoginAttemptCount = user.AccessFailedCount;
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
    }
}