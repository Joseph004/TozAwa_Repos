using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [Route("api/[controller]")]
    [Produces("application/json")]

    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly ICurrentCountry _currentCountry;
        private readonly IMediator _mediator;
        private readonly IDataProtectionProviderService _dataProtectionProviderService;
        private readonly IUserTokenService _userTokenService;

        public AuthenticateController(UserManager<ApplicationUser> userManager, IDataProtectionProviderService dataProtectionProviderService, ICurrentCountry currentCountry, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IUserTokenService userTokenService, IMediator mediator)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _currentCountry = currentCountry;
            _mediator = mediator;
            _dataProtectionProviderService = dataProtectionProviderService;
            _userTokenService = userTokenService;
        }

        [HttpGet, Route("current/{oid:Guid}")]
        public async Task<IActionResult> GetCurrentUser(Guid oid) => Ok(await _mediator.Send(new GetCurrentUserQuery(oid)));

        [HttpPost, Route("signin")]
        public async Task<ActionResult> SignInPost([FromBody] LoginRequest request)
        {
            var response = new LoginResponseDto
            {
                LoginSuccess = true,
                ErrorMessage = ""
            };

            var pswd = request.Content;

            var command = new LoginCommand
            {
                Email = request.Email,
                Password = pswd
            };

            var validator = new LoginCommandFluentValidator();

            var requestValidate = validator.Validate(command);

            if (!requestValidate.IsValid)
            {
                response.LoginSuccess = false;
                response.ErrorMessage = requestValidate.Errors.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).FirstOrDefault().ErrorMessage;
                return Ok(response);
            }

            var user = await userManager.FindByEmailAsync(command.Email);

            if (user == null)
            {
                response.LoginSuccess = false;
                response.ErrorMessageGuid = SystemTextId.EmailOrPasswordWrong;
                return Ok(response);
            }
            if (user.Deleted)
            {
                response.LoginSuccess = false;
                response.ErrorMessageGuid = SystemTextId.Unauthorized;
                return Ok(response);
            }
            var validPassword = await userManager.CheckPasswordAsync(user, command.Password);
            if (!validPassword)
            {
                response.LoginSuccess = false;
                response.ErrorMessageGuid = SystemTextId.EmailOrPasswordWrong;
                return Ok(response);
            }

            var userDto = await _mediator.Send(new GetCurrentUserQuery(user.UserId));

            var securityToken = _userTokenService.GenerateTokenOptions(userDto);
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            response.Token = token;
            /* var ng = userManager.Users.Where(x => x.RootUser).FirstOrDefault();
            await userManager.AddPasswordAsync(ng, "Zairenumber01?"); */

            return Ok(response);
        }

        /* [HttpPost("AuthenticateUser")]
        public async Task<IActionResult> AuthenticateUser(AuthenticateUser authenticateUser)
        {
            var user = await _userManager.FindByNameAsync(authenticateUser.UserName);
            if (user == null) return Unauthorized();

            bool isValidUser = await _userManager.CheckPasswordAsync(user, authenticateUser.Password);

            if (isValidUser)
            {
                string accessToken = GenerateAccessToken(user);
                var refreshToken = GenerateRefreshToken();
                //user.RefreshToken = refreshToken;
                await _userManager.UpdateAsync(user);

                var response = new MainResponse
                {
                    Content = new AuthenticationResponse
                    {
                        RefreshToken = refreshToken,
                        AccessToken = accessToken
                    },
                    IsSuccess = true,
                    ErrorMessage = ""
                };
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        } */

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

        [HttpPost, Route("root")]
        public async Task<ActionResult> SignInPostRoot([FromBody] LoginRootRequest request)
        {
            var response = new LoginResponseDto
            {
                LoginSuccess = true,
                ErrorMessage = ""
            };

            var pswd = request.Content;
            //await _dataProtectionProviderService.DecryptAsync(request.Content);

            var command = new LoginRootCommand
            {
                UserName = request.UserName,
                Password = pswd
            };

            var validator = new LoginRootCommandFluentValidator();

            var requestValidate = validator.Validate(command);

            if (!requestValidate.IsValid)
            {
                response.LoginSuccess = false;
                response.ErrorMessage = requestValidate.Errors.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).FirstOrDefault().ErrorMessage;
                return Ok(response);
            }

            var user = new ApplicationUser();
            try
            {
                user = await userManager.FindByNameAsync(command.UserName);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                response.LoginSuccess = false;
                response.ErrorMessage = "Error";
                response.ErrorMessageGuid = SystemTextId.Error;
                return Ok(response);
            }

            if (user == null)
            {
                response.LoginSuccess = false;
                response.ErrorMessageGuid = SystemTextId.UserNamelOrPasswordWrong;
                return Ok(response);
            }

            if (!user.RootUser || user.Deleted)
            {
                response.LoginSuccess = false;
                response.ErrorMessageGuid = SystemTextId.Unauthorized;
                return Ok(response);
            }
            //var token = await userManager.GeneratePasswordResetTokenAsync(user);
            //await userManager.ResetPasswordAsync(user, token, pswd);
            //await userManager.AddPasswordAsync(user, request.Password);

            var validPassword = await userManager.CheckPasswordAsync(user, command.Password);
            if (!validPassword)
            {
                if (userManager.SupportsUserLockout && await userManager.GetLockoutEnabledAsync(user) && user.AccessFailedCount < 3)
                {
                    user.LastAttemptLogin = DateTime.UtcNow;
                    var currentCountry = await _currentCountry.GetUserCountryByIp();

                    if (!string.IsNullOrEmpty(currentCountry.Country))
                    {
                        user.LastLoginCountry = currentCountry.Country;
                    }
                    if (!string.IsNullOrEmpty(currentCountry.Ip))
                    {
                        user.LastLoginIPAdress = currentCountry.Ip;
                    }
                    if (!string.IsNullOrEmpty(currentCountry.State))
                    {
                        user.LastLoginState = currentCountry.State;
                    }
                    if (!string.IsNullOrEmpty(currentCountry.City))
                    {
                        user.LastLoginCity = currentCountry.City;
                    }
                    await userManager.UpdateAsync(user);
                    await userManager.AccessFailedAsync(user);
                }

                response.LoginSuccess = false;
                response.LoginAttemptCount = user.AccessFailedCount;
                response.ErrorMessageGuid = SystemTextId.UserNamelOrPasswordWrong;
                return Ok(response);
            }
            else
            {
                if (userManager.SupportsUserLockout && await userManager.GetAccessFailedCountAsync(user) > 0)
                {
                    await userManager.ResetAccessFailedCountAsync(user);
                }
            }

            var userDto = await _mediator.Send(new GetCurrentUserQuery(user.UserId));

            var securityToken = _userTokenService.GenerateTokenOptions(userDto);
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            response.Token = token;

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