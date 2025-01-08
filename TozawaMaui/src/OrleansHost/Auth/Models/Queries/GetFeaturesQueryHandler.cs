using System.Buffers;
using System.Collections.Immutable;
using Grains;
using Grains.Auth.Controllers;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos;
using Grains.Helpers;
using MediatR;
using Microsoft.Extensions.Primitives;

namespace OrleansHost.Auth.Models.Queries;

public class GetFeaturesQuery(Dictionary<string, StringValues> queryParameters = null) : BaseQuery(queryParameters), IRequest<TableDataDto<FeatureDto>>
{

}
public class GetFeaturesQueryHandler(IGrainFactory factory, Grains.Auth.Services.ICurrentUserService currentUserService) : IRequestHandler<GetFeaturesQuery, TableDataDto<FeatureDto>>
{
    private readonly IGrainFactory _factory = factory;
    private readonly Grains.Auth.Services.ICurrentUserService _currentUserService = currentUserService;

    public async Task<TableDataDto<FeatureDto>> Handle(GetFeaturesQuery request, CancellationToken cancellationToken)
    {
        var converted = new List<FeatureDto>();
        // get all item keys for this owner
        var keys = await _factory.GetGrain<IFeatureManagerGrain>(SystemTextId.FeatureOwnerId).GetAllAsync();

        // fast path for empty owner
        if (keys.Length == 0) return new TableDataDto<FeatureDto> { Items = [], TotalItems = 0, ItemPage = 0 };

        // fan out and get all individual items in parallel
        var tasks = ArrayPool<Task<FeatureItem>>.Shared.Rent(keys.Length);
        try
        {
            // issue all requests at the same time
            for (var i = 0; i < keys.Length; ++i)
            {
                tasks[i] = _factory.GetGrain<IFeatureGrain>(keys[i]).GetAsync();
            }

            // compose the result as requests complete
            var result = ImmutableArray.CreateBuilder<FeatureItem>(tasks.Length);
            for (var i = 0; i < keys.Length; ++i)
            {
                result.Add(await tasks[i]);
            }
            foreach (var featureItem in result)
            {
                if (!request.IncludeDeleted && featureItem.Deleted) continue;

                var convertedfeature = new FeatureDto
                {
                    Id = featureItem.Id,
                    TextId = featureItem.TextId,
                    DescriptionTextId = featureItem.DescriptionTextId,
                    Deleted = featureItem.Deleted

                };
                await SetTranslation(convertedfeature);
                converted.Add(convertedfeature);
            }

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                converted = converted.Where(Filtered(request.SearchString)).ToList();
            }
            var paged = converted.Skip(request.Page * request.PageSize).Take(request.PageSize);
            return new TableDataDto<FeatureDto> { Items = paged, TotalItems = converted.Count };
        }
        finally
        {
            ArrayPool<Task<FeatureItem>>.Shared.Return(tasks);
        }
    }

    private static Func<FeatureDto, bool> Filtered(string searchString) => x => x.Id.ToString().Contains(searchString, StringComparison.InvariantCultureIgnoreCase) ||
                                                                              (!string.IsNullOrEmpty(x.Text) && x.Text.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                              (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchString, StringComparison.InvariantCultureIgnoreCase));

    private async Task SetTranslation(FeatureDto feature)
    {
        if (feature.DescriptionTextId != Guid.Empty)
        {
            var translationItem = await _factory.GetGrain<ITranslationGrain>(feature.DescriptionTextId).GetAsync();
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
                    feature.Description = value;
                }
                else
                {
                    feature.Description = "";
                }
            }
        }

        if (feature.TextId != Guid.Empty)
        {
            var translationItem = await _factory.GetGrain<ITranslationGrain>(feature.TextId).GetAsync();
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
                    feature.Text = value;
                }
                else
                {
                    feature.Text = "";
                }
            }
        }
    }
}