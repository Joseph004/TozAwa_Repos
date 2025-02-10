using System.Buffers;
using System.Collections.Immutable;
using Grains;
using Grains.Auth.Controllers;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Dtos;
using Grains.Helpers;
using MediatR;
using Microsoft.Extensions.Primitives;

namespace OrleansHost.Auth.Models.Queries;

public class GetOrganizationsQuery(Dictionary<string, StringValues> queryParameters = null) : BaseQuery(queryParameters), IRequest<TableDataDto<OrganizationDto>>
{
}

public class GetOrganizationsQueryHandler(IMediator mediator, IGrainFactory factory, Grains.Auth.Services.ICurrentUserService currentUserService) : IRequestHandler<GetOrganizationsQuery, TableDataDto<OrganizationDto>>
{
    private readonly IGrainFactory _factory = factory;
    public readonly IMediator _mediator = mediator;
    private readonly Grains.Auth.Services.ICurrentUserService _currentUserService = currentUserService;

    public async Task<TableDataDto<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var converted = new List<OrganizationDto>();
        // get all item keys for this owner
        var keys = await _factory.GetGrain<IOrganizationManagerGrain>(SystemTextId.OrganizationOwnerId).GetAllAsync();

        // fast path for empty owner
        if (keys.Length == 0) return new TableDataDto<OrganizationDto> { Items = [], TotalItems = 0, ItemPage = 0 };

        // fan out and get all individual items in parallel
        var tasks = ArrayPool<Task<OrganizationItem>>.Shared.Rent(keys.Length);
        try
        {
            // issue all requests at the same time
            for (var i = 0; i < keys.Length; ++i)
            {
                tasks[i] = _factory.GetGrain<IOrganizationGrain>(keys[i]).GetAsync();
            }

            // compose the result as requests complete
            var result = ImmutableArray.CreateBuilder<OrganizationItem>(tasks.Length);
            for (var i = 0; i < keys.Length; ++i)
            {
                result.Add(await tasks[i]);
            }
            foreach (var organizationItem in result)
            {
                if (!request.IncludeDeleted && organizationItem.Deleted) continue;
                var roleDtos = await _mediator.Send(new GetRolesQuery(organizationItem.Id));
                var addressesDtos = await _mediator.Send(new GetAddressesQuery(organizationItem.Id));
                var convertedOrganization = OrganizationConverter.Convert(organizationItem, addressesDtos.ToList(), roleDtos.ToList());
                convertedOrganization.AttachmentsCount = organizationItem.AttachmentsCount;
                await SetTranslation(convertedOrganization);
                converted.Add(convertedOrganization);
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                converted = converted.Where(x => x.Email.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase)).ToList();
                return new TableDataDto<OrganizationDto> { Items = converted, TotalItems = converted.Count };
            }
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                converted = converted.Where(Filtered(request.SearchString)).ToList();
            }
            var paged = converted.Skip(request.Page * request.PageSize).Take(request.PageSize);
            return new TableDataDto<OrganizationDto> { Items = paged, TotalItems = converted.Count };
        }
        finally
        {
            ArrayPool<Task<OrganizationItem>>.Shared.Return(tasks);
        }
    }

    private static Func<OrganizationDto, bool> Filtered(string searchString) => x => x.Email.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) ||
                                                                              (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                              (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                                (!string.IsNullOrEmpty(x.Comment) && x.Comment.Contains(searchString, StringComparison.InvariantCultureIgnoreCase));

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