using FluentValidation;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Services;
using Grains.Context;
using Grains.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrleansHost.Helpers.MiddlewareExceptions;
using Shared.SignalR;

namespace Grains.Auth.Controllers;

public class ChangePasswordCommand : IRequest<ResetPasswordResponse>
{
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string CurrentPassword { get; set; }

}

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    private static readonly char[] Punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.Password).MinimumLength(8);
        RuleFor(x => x.Password).Equal(x => x.ConfirmPassword);
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.Password).Must(MatchPasswordRules).WithMessage("Password doesnt meet requirements");
    }

    private bool MatchPasswordRules(string password)
    {
        return password.ToCharArray().Any(x => Punctuations.Contains(x)) && password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsNumber);
    }
}

public class ChangePasswordCommandHandler(TozawangoDbContext context, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService,
    IGrainFactory factory, IHubContext<ClientHub> hub, IEmailMessageService emailMessageService, ILogger<ChangePasswordCommandHandler> logger) : IRequestHandler<ChangePasswordCommand, ResetPasswordResponse>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IGrainFactory _factory = factory;
    private readonly IHubContext<ClientHub> _hub = hub;
    private readonly IEmailMessageService _emailMessageService = emailMessageService;
    private readonly TozawangoDbContext _context = context;
    private readonly ILogger<ChangePasswordCommandHandler> _logger = logger;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<ResetPasswordResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.TzUsers
                    .Include(t => t.UserOrganizations)
                    .Include(y => y.Organizations)
                    .ThenInclude(u => u.OrganizationUsers)
                    .Include(y => y.Organizations)
                    .ThenInclude(u => u.Features)
                    .Include(w => w.Addresses)
                    .Include(u => u.Roles)
                    .ThenInclude(y => y.Role)
                    .Include(u => u.Roles)
                    .ThenInclude(y => y.Role.Functions)
                    .FirstOrDefaultAsync(x => x.UserId == _currentUserService.User.Id, cancellationToken);
        if (user == null || user.Deleted)
        {
            throw new HttpStatusCodeException(429, "user not found");
        }

        var isPasswordCheckSucceed = await CheckIfUserPasswordExist(user, request.CurrentPassword);
        var resetPasswordResponse = new ResetPasswordResponse
        {
            IsPasswordCheckSucceed = isPasswordCheckSucceed
        };
        if (!isPasswordCheckSucceed)
        {
            return resetPasswordResponse;
        }
        var newpassword = await ChangeUserPassword(user, request.Password);
        if (string.IsNullOrEmpty(newpassword))
        {
            return resetPasswordResponse;
        }
        await _emailMessageService.SendNewPassword(user.Email, newpassword);
        _context.UserLogs.Add(new UserLog
        {
            Event = LogEventType.ChangePassword,
            UserName = user.UserName,
            Email = user.Email
        });
        _context.SaveChanges();
        return resetPasswordResponse;
    }
    private async Task<bool> CheckIfUserPasswordExist(ApplicationUser user, string currentPassword)
    {
        var validPassword = await userManager.CheckPasswordAsync(user, currentPassword);

        return validPassword;
    }
    private async Task<string> ChangeUserPassword(ApplicationUser user, string newPassword)
    {
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
        var res = await _userManager.UpdateAsync(user);
        if (res.Succeeded)
        {
            return newPassword;
        }

        return "";
    }
}