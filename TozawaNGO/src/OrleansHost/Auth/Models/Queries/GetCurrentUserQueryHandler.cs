using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Grains.Context;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Dtos;
using Grains.Auth.Services;
using Grains;
using Grains.Helpers;

namespace OrleansHost.Auth.Models.Queries
{
    public class GetCurrentUserQueryHandler(IGrainFactory factory, TozawangoDbContext context, ICurrentUserConverter currentUserConverter, ILookupNormalizer normalizer, ICurrentCountry currentCountry) : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
    {
        private readonly IGrainFactory _factory = factory;
        private readonly TozawangoDbContext _context = context;
        private readonly ICurrentUserConverter _currentUserConverter = currentUserConverter;
        private readonly ICurrentCountry _currentCountry = currentCountry;
        private readonly ILookupNormalizer _normalizer = normalizer;

        public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.TzUsers
                           .Include(x => x.Partner)
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

            await _factory.GetGrain<IMemberGrain>(user.UserId).SetAsync(new MemberItem(
                user.UserId,
      user.PartnerId,
      user.Description,
     user.DescriptionTextId,
      user.FirstName,
      user.LastName,
      user.LastLoginCountry,
      user.LastLoginCity,
      user.LastLoginState,
      user.LastLoginIPAdress,
      user.Adress,
      user.UserPasswordHash,
      user.Roles,
      user.LastAttemptLogin,
      user.RefreshToken,
      user.RefreshTokenExpiryTime,
      user.UserCountry,
      user.Deleted,
      user.AdminMember,
      user.LastLogin,
      user.CreatedBy,
      user.CreateDate,
      user.ModifiedBy,
      user.ModifiedDate,
      user.StationIds,
      user.Email,
      user.PasswordHash,
      SystemTextId.MemberOwnerId
            ));

            var response = _currentUserConverter.Convert(user);

            return await Task.FromResult(response);
        }
    }
}