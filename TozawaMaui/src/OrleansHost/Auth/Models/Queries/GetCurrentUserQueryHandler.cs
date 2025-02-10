using MediatR;
using Microsoft.EntityFrameworkCore;
using Grains.Context;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Dtos;
using Grains.Auth.Services;
using Grains;
using Grains.Auth.Models.Authentication;

namespace OrleansHost.Auth.Models.Queries
{
    public class GetCurrentUserQueryHandler(
    IGrainFactory factory,
    TozawangoDbContext context,
    ICurrentUserConverter currentUserConverter,
    IMediator mediator,
    ICurrentCountry currentCountry) : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
    {
        private readonly IGrainFactory _factory = factory;
        private readonly TozawangoDbContext _context = context;
        private readonly IMediator _mediator = mediator;
        private readonly ICurrentUserConverter _currentUserConverter = currentUserConverter;
        private readonly ICurrentCountry _currentCountry = currentCountry;

        public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var memberItem = await _factory.GetGrain<IMemberGrain>(request.Oid).GetAsync();

            if (memberItem == null) return new CurrentUserDto();

            var user = await _context.TzUsers.Include(y => y.UserOrganizations)
                           .FirstOrDefaultAsync(x => x.UserId == request.Oid, cancellationToken: cancellationToken) ?? throw new ArgumentNullException();
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
            _context.SaveChanges();

            var attachmentsCount = await context.FileAttachments.Include(t => t.Owners).CountAsync(x => x.Owners.Any(y => y.OwnerId == memberItem.UserId), cancellationToken: cancellationToken);
            await _factory.GetGrain<IMemberGrain>(memberItem.UserId).SetAsync(new MemberItem(
                memberItem.UserId,
      memberItem.Description,
     memberItem.DescriptionTextId,
      memberItem.FirstName,
      memberItem.LastName,
      user.LastLoginCountry,
      user.LastLoginCity,
      user.LastLoginState,
      user.LastLoginIPAdress,
      memberItem.Roles,
      memberItem.LastAttemptLogin,
      memberItem.RefreshToken,
      memberItem.RefreshTokenExpiryTime,
      memberItem.UserCountry,
      memberItem.Deleted,
      memberItem.AdminMember,
      memberItem.LastLogin,
      memberItem.CreatedBy,
      memberItem.CreateDate,
      memberItem.ModifiedBy,
      memberItem.ModifiedDate,
      memberItem.StationIds,
      memberItem.Email,
      memberItem.PasswordHash,
      attachmentsCount,
      memberItem.Tenants,
      memberItem.LandLords,
      memberItem.Features,
      memberItem.Functions,
      memberItem.Comment,
      memberItem.CommentTextId,
      memberItem.OrganizationIds,
      memberItem.CityCode,
      memberItem.CountryCode,
      memberItem.Gender,
      memberItem.OwnerKey
            ));

            var organizations = new List<CurrentUserOrganizationDto>();
            foreach (var item in memberItem.OrganizationIds)
            {
                var org = (await _mediator.Send(new GetOrganizationQuery { Id = item }, cancellationToken)) ?? new();
                organizations.Add(new CurrentUserOrganizationDto
                {
                    Id = org.Id,
                    Addresses = org.Addresses,
                    Features = org.Features,
                    Name = org.Name,
                    Active = true,
                    Country = org.Country,
                    City = org.City,
                    PrimaryOrganization = org.Id == memberItem.OwnerKey
                });
            }
            var primaryOrganization = user.UserOrganizations.First(x => x.PrimaryOrganization);
            var roleDtos = ((await _mediator.Send(new GetRolesQuery(memberItem.UserId), cancellationToken)) ?? [])
            .Where(x => x.OrganizationId == (request.OrganizationId ?? primaryOrganization.OrganizationId));
            var addressesDtos = await _mediator.Send(new GetAddressesQuery(memberItem.UserId), cancellationToken);
            var functions = roleDtos.SelectMany(x => x.Functions).Select(y => new CurrentUserFunctionDto
            {
                FunctionType = y.FunctionType
            }).Distinct().ToList();
            var response = _currentUserConverter.Convert(memberItem, organizations, [.. addressesDtos], [.. roleDtos], functions);
            response.WorkingOrganizationId = request.OrganizationId ?? primaryOrganization.OrganizationId;
            return await Task.FromResult(response);
        }
    }
}