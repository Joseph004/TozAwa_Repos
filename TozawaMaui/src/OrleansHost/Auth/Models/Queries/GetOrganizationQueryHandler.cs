
using MediatR;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Dtos;
using FluentValidation;
using Grains;

namespace OrleansHost.Auth.Models.Queries
{
    public class GetOrganizationQuery : IRequest<OrganizationDto>
    {
        public Guid Id { get; set; }
    }
    public class GetOrganizationQueryFluentValidator : AbstractValidator<GetOrganizationQuery>
    {
        public GetOrganizationQueryFluentValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }

    public class GetOrganizationQueryHandler(IMediator mediator, IGrainFactory factory, Grains.Auth.Services.ICurrentUserService currentUserService) : IRequestHandler<GetOrganizationQuery, OrganizationDto>
    {
        private readonly IGrainFactory _factory = factory;
        public readonly IMediator _mediator = mediator;
        private readonly Grains.Auth.Services.ICurrentUserService _currentUserService = currentUserService;

        public async Task<OrganizationDto> Handle(GetOrganizationQuery request, CancellationToken cancellationToken)
        {
            var organizationItem = await _factory.GetGrain<IOrganizationGrain>(request.Id).GetAsync();

            if (organizationItem == null) return new OrganizationDto();

            var roleDtos = await _mediator.Send(new GetRolesQuery(organizationItem.Id), cancellationToken);
            var addressesDtos = await _mediator.Send(new GetAddressesQuery(organizationItem.Id), cancellationToken);
            var convertedOrganization = OrganizationConverter.Convert(organizationItem, [.. addressesDtos], [.. roleDtos]);
            convertedOrganization.AttachmentsCount = organizationItem.AttachmentsCount;

            await SetTranslation(convertedOrganization);
            return convertedOrganization;
        }

        private async Task SetTranslation(OrganizationDto organization)
        {
            if (organization.DescriptionTextId != Guid.Empty)
            {
                var translationItem = await _factory.GetGrain<ITranslationGrain>(organization.DescriptionTextId).GetAsync();
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

                if (translation != null && translation.TextId != Guid.Empty)
                {
                    if (translation.LanguageText.TryGetValue(_currentUserService.LanguageId, out string value))
                    {
                        organization.Description = value;
                    }
                    else
                    {
                        organization.Description = "";
                    }
                }
            }

            if (organization.CommentTextId != Guid.Empty)
            {
                var translationItem = await _factory.GetGrain<ITranslationGrain>(organization.CommentTextId).GetAsync();
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

                if (translation != null && translation.TextId != Guid.Empty)
                {
                    if (translation.LanguageText.TryGetValue(_currentUserService.LanguageId, out string value))
                    {
                        organization.Comment = value;
                    }
                    else
                    {
                        organization.Comment = "";
                    }
                }
            }
        }
    }
}