
using MediatR;
using Microsoft.Extensions.Logging;
using Grains.Auth.Services;
using Grains.Auth.Models.Dtos;
using OrleansHost.Auth.Models.Queries;
using FluentValidation;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Grains.Auth.Models.Authentication;
using Grains.Helpers;
using Grains.Models.ResponseRequests;
using System.Net;

namespace Grains.Auth.Controllers
{
    public class SwitchOrganizationCommand : IRequest<AddResponse<LoginResponseDto>>
    {
        public Guid Id { get; set; }
    }

    public class SwitchOrganizationCommandRequestFluentValidator : AbstractValidator<SwitchOrganizationCommand>
    {
        public SwitchOrganizationCommandRequestFluentValidator()
        {
            RuleFor(x => x.Id)
            .NotNull()
            .NotEmpty();
        }
    }

    public class SwitchOrganizationCommandHandler(ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, IMediator mediator, IGrainFactory factory, IUserTokenService userTokenService, ILogger<SwitchOrganizationCommandHandler> logger) : IRequestHandler<SwitchOrganizationCommand, AddResponse<LoginResponseDto>>
    {
        public readonly IMediator _mediator = mediator;
        private readonly IGrainFactory _factory = factory;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUserTokenService _userTokenService = userTokenService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<AddResponse<LoginResponseDto>> Handle(SwitchOrganizationCommand request, CancellationToken cancellationToken)
        {
            var response = new AddResponse<LoginResponseDto>(true, "Success", HttpStatusCode.OK, null);

            var currentUser = await _mediator.Send(new GetCurrentUserQuery(_currentUserService.User.Id, request.Id), cancellationToken);
            var user = await _userManager.FindByEmailAsync(currentUser.Email);
            var tokenOptions = _userTokenService.GenerateTokenOptions(currentUser);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            user.RefreshToken = _userTokenService.GenerateRefreshToken();
            await _userManager.UpdateAsync(user);
            await _factory.GetGrain<ILoggedInGrain>(user.UserId).SetAsync(new LoggedInItem(user.UserId, token, user.RefreshToken, SystemTextId.LoggedInOwnerId));
            response.Entity = new LoginResponseDto { Token = token, RefreshToken = user.RefreshToken, LoginSuccess = true };
            return response;
        }
    }
}