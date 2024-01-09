using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TozawaNGO.Auth.Controllers.Login;
using TozawaNGO.Auth.Models;
using TozawaNGO.Auth.Models.Authentication;
using TozawaNGO.Auth.Models.Dtos;
using TozawaNGO.Auth.Models.FormModels;
using TozawaNGO.Auth.Models.Queries;
using TozawaNGO.Auth.Services;
using TozawaNGO.Models.ResponseRequests;
using System.Net;
using TozawaNGO.Context;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Configurations;
using TozawaNGO.Services;

namespace TozawaNGO.Auth.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]

    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IUserTokenService _userTokenService;
        private readonly TozawangoDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly IEncryptDecrypt _encryptDecrypt;

        public AuthenticateController(UserManager<ApplicationUser> userManager, AppSettings appSettings, TozawangoDbContext context, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IUserTokenService userTokenService, IMediator mediator, IEncryptDecrypt encryptDecrypt)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _mediator = mediator;
            _userTokenService = userTokenService;
            _context = context;
            _appSettings = appSettings;
            _encryptDecrypt = encryptDecrypt;
        }

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

            var pswd = _encryptDecrypt.DecryptUsingCertificate(request.Content);

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

            //var user = await userManager.FindByEmailAsync(command.Email);
            var user = await _context.TzUsers.FirstOrDefaultAsync(x => x.Email == command.Email);
            //await userManager.AddPasswordAsync(user, "Zairenumber01?");

            if (user == null)
            {
                response.Success = false;
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
                response.Success = false;
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