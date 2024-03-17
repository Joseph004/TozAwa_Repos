
using MediatR;
using Grains.Auth.Models.Authentication;
using Grains.Helpers;
using System.Buffers;
using System.Collections.Immutable;

namespace Grains.Auth.Controllers
{
    public class GetApplicationUserQueryHandler(IGrainFactory factory) : IRequestHandler<GetApplicationUserQuery, ApplicationUser>
    {
        private readonly IGrainFactory _factory = factory;

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
                    Roles = memberItem.Roles,
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