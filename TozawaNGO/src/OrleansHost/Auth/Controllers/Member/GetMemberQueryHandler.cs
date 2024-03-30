
using MediatR;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Dtos.Backend;
using OrleansHost.Attachment.Models.Queries;

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
            });
            member.Timestamp = memberItem.Timestamp;

            await SetTranslation(member);
            await GetAttachments(member);
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

        private async Task GetAttachments(MemberDto member)
        {
            var request = new GetAttachmentsByOwnerIdsQuery
            {
                OwnerIds = [member.Id]
            };
            var ownerAttachments = await _mediator.Send(request);
            if (ownerAttachments != null)
            {
                foreach (var ownerAttachment in ownerAttachments)
                {
                    member.Attachments.AddRange(ownerAttachment.Attachments);
                    if (member.Attachments != null && member.Attachments.Any(x => !string.IsNullOrEmpty(x.MiniatureId) && !string.IsNullOrEmpty(x.Thumbnail)))
                    {
                        member.Thumbnail = member.Attachments.First(x => !string.IsNullOrEmpty(x.MiniatureId) && !string.IsNullOrEmpty(x.Thumbnail)).Thumbnail;
                    }
                }
            }
        }
    }
}