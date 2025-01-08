
using MediatR;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Dtos.Backend;
using OrleansHost.Auth.Models.Queries;

namespace Grains.Auth.Controllers
{
    public class GetMemberQueryHandler(IMediator mediator, IGrainFactory factory, Grains.Auth.Services.ICurrentUserService currentUserService) : IRequestHandler<GetMemberQuery, Models.Dtos.Backend.MemberDto>
    {
        private readonly IGrainFactory _factory = factory;
        public readonly IMediator _mediator = mediator;
        private readonly Grains.Auth.Services.ICurrentUserService _currentUserService = currentUserService;

        public async Task<Models.Dtos.Backend.MemberDto> Handle(GetMemberQuery request, CancellationToken cancellationToken)
        {
            var memberItem = await _factory.GetGrain<IMemberGrain>(request.Id).GetAsync();

            if (memberItem == null) return new MemberDto();
            var roleDtos = await _mediator.Send(new GetRolesQuery(memberItem.UserId));
            var addressesDtos = await _mediator.Send(new GetAddressesQuery(memberItem.UserId));
            var member = MemberConverter.Convert(new ApplicationUser
            {
                UserId = memberItem.UserId,
                PartnerId = memberItem.PartnerId,
                Email = memberItem.Email,
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
            }, addressesDtos.ToList());
            member.Timestamp = memberItem.Timestamp;
            await SetTranslation(member);
            return member;
        }

        private async Task SetTranslation(MemberDto member)
        {
            if (member.DescriptionTextId != Guid.Empty)
            {
                var translationItem = await _factory.GetGrain<ITranslationGrain>(member.DescriptionTextId).GetAsync();
                var translation = translationItem != null ? new Translation
                {
                    Id = translationItem.Id,
                    TextId = translationItem.TextId,
                    LanguageText = translationItem.LanguageText,
                    CreateDate = translationItem.CreatedDate,
                    CreatedBy = translationItem.CreatedBy,
                    ModifiedBy = translationItem.ModifiedBy,
                    ModifiedDate = translationItem.ModifiedDate
                } : new Translation();

                if (translation != null && translation.Id != Guid.Empty)
                {
                    if (translation.LanguageText.TryGetValue(_currentUserService.LanguageId, out string value))
                    {
                        member.Description = value;
                    }
                    else
                    {
                        member.Description = "";
                    }
                }
            }
        }
    }
}