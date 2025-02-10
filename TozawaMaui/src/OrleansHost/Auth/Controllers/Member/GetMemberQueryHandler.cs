
using MediatR;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Dtos.Backend;
using OrleansHost.Auth.Models.Queries;
using Grains.Auth.Models.Dtos;

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
            var member = MemberConverter.Convert(memberItem, organizations, addressesDtos.ToList(), roleDtos.ToList(), functions);
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