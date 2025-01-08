
using MediatR;
using Grains.Auth.Models.Authentication;
using Grains.Helpers;
using System.Buffers;
using System.Collections.Immutable;
using OrleansHost.Auth.Models.Queries;

namespace Grains.Auth.Controllers
{
    public class GetApplicationUserQueryHandler(IGrainFactory factory, IMediator mediator) : IRequestHandler<GetApplicationUserQuery, ApplicationUser>
    {
        private readonly IGrainFactory _factory = factory;
        public readonly IMediator _mediator = mediator;

        public async Task<ApplicationUser> Handle(GetApplicationUserQuery request, CancellationToken cancellationToken)
        {
            var member = new ApplicationUser();

            // get all item keys for this owner
            var keys = await _factory.GetGrain<IMemberManagerGrain>(SystemTextId.MemberOwnerId).GetAllAsync();

            // fast path for empty owner
            if (keys.Length == 0) return new ApplicationUser();

            // fan out and get all individual items in parallel
            var tasks = ArrayPool<Task<MemberItem>>.Shared.Rent(keys.Length);
            try
            {
                // issue all requests at the same time
                for (var i = 0; i < keys.Length; ++i)
                {
                    tasks[i] = _factory.GetGrain<IMemberGrain>(keys[i]).GetAsync();
                }

                // compose the result as requests complete
                var result = ImmutableArray.CreateBuilder<MemberItem>(tasks.Length);
                for (var i = 0; i < keys.Length; ++i)
                {
                    var item = await tasks[i];
                    if (item == null)
                    {
                        await _factory.GetGrain<IMemberManagerGrain>(SystemTextId.MemberOwnerId).UnregisterAsync(keys[i]);
                    }
                    result.Add(item);
                }

                var memberItem = result.FirstOrDefault(x => x.Email == request.Email);

                if (memberItem == null) return null;
                var roleDtos = await _mediator.Send(new GetRolesQuery(memberItem.UserId));
                var addressesDtos = await _mediator.Send(new GetAddressesQuery(memberItem.UserId));
                return new ApplicationUser
                {
                    PasswordHash = memberItem.PasswordHash,
                    Email = memberItem.Email,
                    UserId = memberItem.UserId,
                    PartnerId = memberItem.PartnerId,
                    Description = memberItem.Description,
                    DescriptionTextId = memberItem.DescriptionTextId,
                    FirstName = memberItem.FirstName,
                    LastName = memberItem.LastName,
                    LastLoginCountry = memberItem.LastLoginCountry,
                    LastLoginCity = memberItem.LastLoginCity,
                    LastLoginState = memberItem.LastLoginState,
                    LastLoginIPAdress = memberItem.LastLoginIPAdress,
                    Adress = memberItem.Adress,
                    UserPasswordHash = memberItem.UserPasswordHash,
                    Roles = roleDtos.Select(y => new UserRole
                    {
                        UserId = memberItem.UserId,
                        RoleId = y.Id,
                        OrganizationId = y.OrganizationId,
                        Role = new Role
                        {
                            Id = y.Id,
                            RoleEnum = (RoleEnum)y.Role,
                            Functions = y.Functions.Select(f => new Function
                            {
                                Functiontype = f.FunctionType,
                                RoleId = f.RoleId,
                                Role = new Role
                                {
                                    OrganizationId = y.OrganizationId,
                                    Functions = y.Functions.Select(t => new Function
                                    {
                                        Functiontype = t.FunctionType
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
            finally
            {
                ArrayPool<Task<MemberItem>>.Shared.Return(tasks);
            }
        }
    }
}