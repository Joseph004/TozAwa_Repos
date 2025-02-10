using MediatR;
using Microsoft.AspNetCore.Identity;
using Grains.Context;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos.Backend;
using Grains.Services;
using Grains.Auth.Models.Dtos;
using Grains;
using OrleansHost.Auth.Models.Queries;

namespace OrleansHost.Auth.Models.Commands
{
    public class CreateUserCommandHandler(TozawangoDbContext context, IMediator mediator, IGrainFactory factory, ILookupNormalizer normalizer, IPasswordHashService passwordHashService) : IRequestHandler<CreateUserCommand, MemberDto>
    {
        private readonly TozawangoDbContext _context = context;
        private readonly ILookupNormalizer _normalizer = normalizer;
        private readonly IGrainFactory _factory = factory;
        public readonly IMediator _mediator = mediator;
        private readonly IPasswordHashService _passwordHashService = passwordHashService;

        public async Task<MemberDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var newuser = new ApplicationUser
            {
                UserId = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                AdminMember = false,
                LockoutEnabled = true
            };
            newuser.NormalizedEmail = _normalizer.NormalizeEmail(newuser.Email);
            newuser.EmailConfirmed = true;
            newuser.SecurityStamp = Guid.NewGuid().ToString();
            newuser.NormalizedUserName = _normalizer.NormalizeName(newuser.UserName);
            _context.TzUsers.Add(newuser);

            _context.SaveChanges();
            var addressDto = newuser.Addresses.Select(x => new AddressDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                City = x.City,
                State = x.State,
                Country = x.Country,
                ZipCode = x.ZipCode,
                Active = x.Active
            }).ToList();
            var item = new MemberItem(
            newuser.UserId,
            newuser.Description,
            newuser.DescriptionTextId,
            newuser.FirstName,
            newuser.LastName,
            newuser.LastLoginCountry,
            newuser.LastLoginCity,
            newuser.LastLoginState,
            newuser.LastLoginIPAdress,
            request.Roles,
            newuser.LastAttemptLogin,
            newuser.RefreshToken,
            newuser.RefreshTokenExpiryTime,
            newuser.UserCountry,
            newuser.Deleted,
            newuser.AdminMember,
            newuser.LastLogin,
            newuser.CreatedBy,
            newuser.CreateDate,
            newuser.ModifiedBy,
            newuser.ModifiedDate,
            newuser.StationIds,
            newuser.Email,
            newuser.PasswordHash,
            0,
            newuser.Tenants.Select(x => x.UserTenant_TenantUser.UserId).ToList(),
            newuser.LandLords.Select(x => x.UserLandLord_LandLordUser.UserId).ToList(),
            request.Features,
            request.Functions,
            newuser.Comment,
            newuser.CommentTextId,
            newuser.UserOrganizations.Select(x => x.OrganizationId).ToList(),
            newuser.CityCode,
            newuser.CountryCode,
            newuser.Gender,
            newuser.UserOrganizations.First(u => u.PrimaryOrganization).OrganizationId
            );
            await _factory.GetGrain<IMemberGrain>(newuser.UserId).SetAsync(item);
            var memberItem = await _factory.GetGrain<IMemberGrain>(newuser.UserId).GetAsync();
            var organizations = new List<CurrentUserOrganizationDto>();
            foreach (var orgItem in newuser.UserOrganizations)
            {
                var org = (await _mediator.Send(new GetOrganizationQuery { Id = orgItem.OrganizationId })) ?? new();
                organizations.Add(new CurrentUserOrganizationDto
                {
                    Id = org.Id,
                    Addresses = org.Addresses,
                    Features = org.Features,
                    Name = org.Name,
                    Active = true
                });
            }
            var roleDtos = await _mediator.Send(new GetRolesQuery(newuser.UserId));
            var functions = roleDtos.SelectMany(x => x.Functions).Select(y => new CurrentUserFunctionDto
            {
                FunctionType = y.FunctionType
            }).Distinct().ToList();
            var response = MemberConverter.Convert(memberItem, organizations, addressDto, roleDtos.ToList(), functions);

            return await Task.FromResult(response);

        }
    }
}