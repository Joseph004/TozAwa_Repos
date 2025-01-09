using System.Net;
using FluentValidation;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos;
using Grains.Auth.Services;
using Grains.Context;
using Grains.Services;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrleansHost.Helpers;
using OrleansHost.Helpers.MiddlewareExceptions;
using Shared.SignalR;

namespace Grains.Auth.Controllers;

public class ResetUserPasswordCommand : IRequest<ResetPasswordResponse>
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string CurrentPassword { get; set; }
}

public class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Continue)
            .NotNull()
            .EmailAddress()
            .WithMessage("A valid email is required")
            .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
            .WithMessage("A valid email is required");
    }
}

public class ResetPasswordResponse
{
    public bool IsPasswordCheckSucceed { get; set; }
}

public class ResetUserPasswordCommandHandler(TozawangoDbContext context, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService,
    IGrainFactory factory, IHubContext<ClientHub> hub, IEmailMessageService emailMessageService, ILogger<ResetUserPasswordCommandHandler> logger) : IRequestHandler<ResetUserPasswordCommand, ResetPasswordResponse>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IGrainFactory _factory = factory;
    private readonly IHubContext<ClientHub> _hub = hub;
    private readonly IEmailMessageService _emailMessageService = emailMessageService;
    private readonly TozawangoDbContext _context = context;
    private readonly ILogger<ResetUserPasswordCommandHandler> _logger = logger;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<ResetPasswordResponse> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = _context.TzUsers.Include(x => x.Organizations).Include(x => x.UserOrganizations).FirstOrDefault(x => x.UserId == request.UserId) ?? throw new NotFoundStatusCodeException(nameof(request.UserId));
        if (!_currentUserService.IsAdmin() && user.AdminMember)
        {
            throw new ForbiddenStatusCodeException("Can not modify admin accounts");
        }

        if (!UserHasPermissionToEditUsersInOrganization(_currentUserService.User, user.Organizations.Select(x => x.Id).ToList()))
        {
            throw new ForbiddenStatusCodeException();
        }

        var isPasswordCheckSucceed = _currentUserService.IsAdmin() || await CheckIfUserPasswordExist(user, request.CurrentPassword);
        ResetPasswordResponse resetPasswordResponse = new() { IsPasswordCheckSucceed = isPasswordCheckSucceed };
        if (!isPasswordCheckSucceed)
        {
            return resetPasswordResponse;
        }
        var newpassword = await ResetUserPassword(user);
        if (string.IsNullOrEmpty(newpassword))
        {
            return resetPasswordResponse;
        }
        await _emailMessageService.SendNewPassword(request.Email, newpassword);
        _context.UserLogs.Add(new UserLog
        {
            Event = LogEventType.ChangePassword,
            UserName = user.UserName,
            Email = request.Email
        });
        _context.SaveChanges();
        return resetPasswordResponse;
    }
    private static bool UserHasPermissionToEditUsersInOrganization(CurrentUserDto currentUser, List<Guid> organizationIds)
    {
        if (organizationIds.Count == 0) return false;
        if (currentUser.Admin) return true;

        return false;
    }
    private async Task<bool> CheckIfUserPasswordExist(ApplicationUser user, string currentPassword)
    {
        var validPassword = await userManager.CheckPasswordAsync(user, currentPassword);

        return validPassword;
    }
    private async Task<string> ResetUserPassword(ApplicationUser user)
    {
        var newPassword = GeneratePassword.RandomPassword();

        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
        var res = await _userManager.UpdateAsync(user);
        if (res.Succeeded)
        {
            return newPassword;
        }

        return "";
    }
}