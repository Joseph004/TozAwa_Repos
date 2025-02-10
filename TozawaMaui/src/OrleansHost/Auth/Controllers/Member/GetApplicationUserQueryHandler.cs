
using MediatR;
using Grains.Auth.Models.Authentication;
using OrleansHost.Auth.Models.Queries;
using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Controllers
{
    public class GetApplicationUserQueryHandler(IGrainFactory factory, IMediator mediator) : IRequestHandler<GetApplicationUserQuery, ApplicationUser>
    {
        private readonly IGrainFactory _factory = factory;
        public readonly IMediator _mediator = mediator;

        public async Task<ApplicationUser> Handle(GetApplicationUserQuery request, CancellationToken cancellationToken)
        {
            var item = await _factory.GetGrain<IEmailGuidGrain>(request.Email).GetAsync();
            if (item == null) return new ApplicationUser();

            var memberItem = await _factory.GetGrain<IMemberGrain>(item.Id).GetAsync();

            if (memberItem == null) return new ApplicationUser();

            var organizations = new List<CurrentUserOrganizationDto>();
            foreach (var orgItem in memberItem.OrganizationIds)
            {
                var org = (await _mediator.Send(new GetOrganizationQuery { Id = orgItem }, cancellationToken)) ?? new();
                organizations.Add(new CurrentUserOrganizationDto
                {
                    Id = org.Id,
                    Addresses = org.Addresses,
                    Features = org.Features,
                    Name = org.Name,
                    Active = true,
                    PrimaryOrganization = org.Id == memberItem.OwnerKey
                });
            }
            var primaryOrganization = organizations.First(x => x.PrimaryOrganization);
            var roleDtos = ((await _mediator.Send(new GetRolesQuery(memberItem.UserId), cancellationToken)) ?? []).Where(x => x.OrganizationId == primaryOrganization.Id);
            var addressesDtos = await _mediator.Send(new GetAddressesQuery(memberItem.UserId), cancellationToken);
            var functions = roleDtos.SelectMany(x => x.Functions).Select(y => new CurrentUserFunctionDto
            {
                FunctionType = y.FunctionType
            }).Distinct().ToList();
            return new ApplicationUser
            {
                PasswordHash = memberItem.PasswordHash,
                Email = memberItem.Email,
                UserId = memberItem.UserId,
                Description = memberItem.Description,
                DescriptionTextId = memberItem.DescriptionTextId,
                FirstName = memberItem.FirstName,
                LastName = memberItem.LastName,
                LastLoginCountry = memberItem.LastLoginCountry,
                LastLoginCity = memberItem.LastLoginCity,
                LastLoginState = memberItem.LastLoginState,
                LastLoginIPAdress = memberItem.LastLoginIPAdress,
                Roles = roleDtos.Select(y => new UserRole
                {
                    UserId = memberItem.UserId,
                    RoleId = y.Id,
                    OrganizationId = y.OrganizationId,
                    Role = new Grains.Auth.Models.Authentication.Role
                    {
                        Id = y.Id,
                        RoleEnum = (RoleEnum)y.Role,
                        Functions = y.Functions.Select(f => new Function
                        {
                            FunctionType = f.FunctionType,
                            RoleId = f.RoleId,
                            Role = new Grains.Auth.Models.Authentication.Role
                            {
                                OrganizationId = y.OrganizationId,
                                Functions = y.Functions.Select(t => new Function
                                {
                                    FunctionType = t.FunctionType
                                }).ToList()
                            }
                        }).ToList()
                    }
                }).ToList(),
                LastAttemptLogin = memberItem.LastAttemptLogin,
                RefreshToken = memberItem.RefreshToken,
                RefreshTokenExpiryTime = memberItem.RefreshTokenExpiryTime,
                UserCountry = memberItem.UserCountry,
                Deleted = memberItem.Deleted,
                AdminMember = memberItem.AdminMember,
                LastLogin = memberItem.LastLogin,
                CreatedBy = memberItem.CreatedBy,
                CreateDate = memberItem.CreateDate,
                ModifiedBy = memberItem.ModifiedBy,
                ModifiedDate = memberItem.ModifiedDate,
                StationIds = memberItem.StationIds
            };
        }
    }
}