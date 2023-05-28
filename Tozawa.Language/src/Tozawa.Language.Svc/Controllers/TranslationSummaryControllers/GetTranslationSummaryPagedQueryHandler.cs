using Tozawa.Language.Svc.Configuration;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Converters;
using Tozawa.Language.Svc.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Controllers.TranslationSummaryControllers
{
    public class GetTranslationSummaryPagedQuery : IRequest<PagedDto<TranslatedTextDto>>
    {
        public Guid SourceLanguageId { get; set; }
        public Guid TargetLanguageId { get; set; }
        public Guid SystemTypeId { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public string FilterText { get; set; }
        public XliffState? XliffState { get; set; }

        public GetTranslationSummaryPagedQuery(Dictionary<string, StringValues> queryParameters)
        {
            if (queryParameters == null)
            {
                return;
            }
            if (queryParameters.ContainsKey(nameof(SourceLanguageId)) && Guid.TryParse(queryParameters[nameof(SourceLanguageId)], out var sourceLanguageId))
            {
                SourceLanguageId = sourceLanguageId;
            }
            if (queryParameters.ContainsKey(nameof(TargetLanguageId)) && Guid.TryParse(queryParameters[nameof(TargetLanguageId)], out var targetLanguageId))
            {
                TargetLanguageId = targetLanguageId;
            }
            if (queryParameters.ContainsKey(nameof(SystemTypeId)) && Guid.TryParse(queryParameters[nameof(SystemTypeId)], out var systemTypeId))
            {
                SystemTypeId = systemTypeId;
            }
            if (queryParameters.ContainsKey(nameof(Page)) && int.TryParse(queryParameters[nameof(Page)], out var page) && page >= 1)
            {
                Page = page;
            }
            if (queryParameters.ContainsKey(nameof(PageSize)) && int.TryParse(queryParameters[nameof(PageSize)], out var pageSize) && pageSize >= 1)
            {
                PageSize = pageSize;
            }
            if (queryParameters.ContainsKey(nameof(XliffState)) && Enum.TryParse(typeof(XliffState), queryParameters[nameof(XliffState)], out var state))
            {
                XliffState = (XliffState)state;
            }
            if (queryParameters.ContainsKey(nameof(FilterText)))
            {
                FilterText = queryParameters[nameof(FilterText)];
            }
        }
    }
    public class GetTranslationSummaryPagedQueryValidator : AbstractValidator<GetTranslationSummaryPagedQuery>
    {
        public GetTranslationSummaryPagedQueryValidator()
        {
            RuleFor(x => x.SourceLanguageId).NotEmpty();
            RuleFor(x => x.TargetLanguageId).NotEmpty();
            RuleFor(x => x.SystemTypeId).NotEmpty();
        }
    }

    public class GetTranslationSummaryPagedQueryHandler : IRequestHandler<GetTranslationSummaryPagedQuery, PagedDto<TranslatedTextDto>>
    {
        private readonly LanguageContext _context;
        private readonly ITranslatedTextConverter _translatedTextConverter;
        private readonly ICurrentUserService _currentUserService;

        private bool IsRoot => !_currentUserService.IsRoot();

        public GetTranslationSummaryPagedQueryHandler(LanguageContext context, ITranslatedTextConverter translatedTextConverter, ICurrentUserService currentUserService)
        {
            _context = context;
            _translatedTextConverter = translatedTextConverter;
            _currentUserService = currentUserService;
        }
        private Expression<Func<Translation, bool>> FilterBySystemType(GetTranslationSummaryPagedQuery request) => IsRoot
                ? (x => _currentUserService.AllOrganizationIds.Contains(x.SystemTypeId))
                : (x => x.SystemTypeId == request.SystemTypeId);
        private Expression<Func<Translation, bool>> FilterByTargetLanguage(GetTranslationSummaryPagedQuery request) => x => x.LanguageId == request.TargetLanguageId;
        private Expression<Func<Translation, bool>> FilterBySourceOrTargetLanguage(GetTranslationSummaryPagedQuery request) => x => x.LanguageId == request.TargetLanguageId || x.LanguageId == request.SourceLanguageId;
        private Expression<Func<Translation, bool>> FilterBySourceLanguage(GetTranslationSummaryPagedQuery request) => x => x.LanguageId == request.SourceLanguageId;
        private Expression<Func<Translation, bool>> FilterByXLiffState(GetTranslationSummaryPagedQuery request) => x => !request.XliffState.HasValue || x.XliffState == request.XliffState.Value;
        private Expression<Func<Translation, bool>> FilterBySearchString(GetTranslationSummaryPagedQuery request) => x => !string.IsNullOrEmpty(x.Text) && x.Text.Contains(request.FilterText);

        public async Task<PagedDto<TranslatedTextDto>> Handle(GetTranslationSummaryPagedQuery request, CancellationToken cancellationToken)
        {

            if (!IsRoot && request.SystemTypeId == Guid.Empty)
                throw new ArgumentNullException(nameof(request));

            if (!string.IsNullOrEmpty(request.FilterText) && Guid.TryParse(request.FilterText, out var textIdFilter))
            {
                return await FilterByTextId(textIdFilter, request);
            }

            if (!string.IsNullOrEmpty(request.FilterText))
            {
                return await FilterByText(request);
            }

            if (request.XliffState.HasValue)
            {
                return await FilterTextsByXliffState(request);
            }
            return await UnfilteredTexts(request);
        }

        private async Task<PagedDto<TranslatedTextDto>> FilterTextsByXliffState(GetTranslationSummaryPagedQuery request)
        {
            var targetTextsWithXliffStates = await _context.Translations
                .Where(FilterBySystemType(request))
                .Where(FilterByTargetLanguage(request))
                .Where(FilterByXLiffState(request))
                .ToListAsync();

            var sourceTranslations = await _context.Translations
                .Where(x => targetTextsWithXliffStates.Select(y => y.TextId).Contains(x.TextId))
                .Where(FilterBySourceLanguage(request))
                .OrderBy(x => x.Text)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var targetTextsWithXliffStatesPaged = targetTextsWithXliffStates.Where(x => sourceTranslations.Select(y => y.TextId).Contains(x.TextId));

            var translatedTextDto = _translatedTextConverter.Convert(sourceTranslations, targetTextsWithXliffStatesPaged);
            return await Task.FromResult(new PagedDto<TranslatedTextDto>
            {
                PageSize = request.PageSize,
                Page = request.Page,
                TotalItems = targetTextsWithXliffStates.Count,
                Items = translatedTextDto
            });
        }

        public async Task GetMissingTranslationsIfExist(List<Translation> translations, GetTranslationSummaryPagedQuery request)
        {
            var grouped = translations.GroupBy(x => x.TextId);
            foreach (var group in grouped)
            {
                var allTranslations = await _context.Translations.Where(x => x.TextId == group.Key)
                                      .Where(FilterBySourceOrTargetLanguage(request))
                                      .AsNoTracking().ToListAsync();
                translations.RemoveAll(x => x.TextId == group.Key);
                translations.AddRange(allTranslations);
            }
        }

        private async Task<PagedDto<TranslatedTextDto>> FilterByText(GetTranslationSummaryPagedQuery request)
        {
            var filteredTexts = await _context.Translations
                                   .Where(FilterBySystemType(request))
                                   .Where(FilterBySearchString(request))
                                   .Where(FilterBySourceOrTargetLanguage(request))
                                   .AsNoTracking()
                                   .ToListAsync();

            await GetMissingTranslationsIfExist(filteredTexts, request);

            var filteredSourceCount = filteredTexts
            .AsQueryable()
            .Where(FilterBySystemType(request))
            .Count(FilterBySourceLanguage(request));

            var filteredSourceLanguageTexts = filteredTexts
                .Where(x => x.LanguageId == request.SourceLanguageId)
                .OrderBy(x => x.Text)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).ToList();

            var filteredTargetLanguageTexts = filteredTexts.AsQueryable()
                .Where(FilterByTargetLanguage(request))
                .Where(x => filteredSourceLanguageTexts.Select(y => y.TextId).Contains(x.TextId));

            var translatedTextDto = _translatedTextConverter.Convert(filteredSourceLanguageTexts, filteredTargetLanguageTexts);
            return await Task.FromResult(new PagedDto<TranslatedTextDto>
            {
                PageSize = request.PageSize,
                Page = request.Page,
                TotalItems = filteredSourceCount,
                Items = translatedTextDto
            });
        }

        private async Task<PagedDto<TranslatedTextDto>> UnfilteredTexts(GetTranslationSummaryPagedQuery request)
        {
            var sourceLanguageTextsCount = await _context.Translations
                       .Where(FilterBySystemType(request))
                       .Where(FilterBySourceLanguage(request))
                       .Where(x => !string.IsNullOrEmpty(x.Text))
                       .AsNoTracking()
                       .CountAsync();

            var sourceLanguageTexts = await _context.Translations
                .Where(FilterBySystemType(request))
                .Where(FilterBySourceLanguage(request))
                .Where(x => !string.IsNullOrEmpty(x.Text))
                .OrderBy(x => x.Text).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync();

            var targetLanguageTexts = await _context.Translations
                .Where(x => sourceLanguageTexts.Select(y => y.TextId).Contains(x.TextId))
                .Where(FilterBySystemType(request))
                .Where(FilterByTargetLanguage(request))
                .AsNoTracking()
                .ToListAsync();

            var converted = _translatedTextConverter.Convert(sourceLanguageTexts, targetLanguageTexts);

            return await Task.FromResult(new PagedDto<TranslatedTextDto>
            {
                PageSize = request.PageSize,
                Page = request.Page,
                TotalItems = sourceLanguageTextsCount,
                Items = converted
            });
        }

        private async Task<PagedDto<TranslatedTextDto>> FilterByTextId(Guid textId, GetTranslationSummaryPagedQuery request)
        {
            var translationsFilteredByTextId = await _context.Translations
                .Where(x => x.TextId == textId)
                .Where(FilterBySystemType(request))
                .Where(FilterBySourceOrTargetLanguage(request))
                .AsNoTracking()
                .ToListAsync();
            if (!translationsFilteredByTextId.Any())
            {
                return new PagedDto<TranslatedTextDto>();
            }

            var sourceTextIdTranslation = translationsFilteredByTextId.FirstOrDefault(x => x.LanguageId == request.SourceLanguageId);
            if (sourceTextIdTranslation == null)
            {
                return new PagedDto<TranslatedTextDto>();
            }

            var sourceTranslations = translationsFilteredByTextId.Where(x => x.LanguageId == request.SourceLanguageId);
            var targetTranslations = translationsFilteredByTextId.Where(x => x.LanguageId == request.TargetLanguageId);

            var translatedTextDto = _translatedTextConverter.Convert(sourceTranslations, targetTranslations);
            return new PagedDto<TranslatedTextDto>
            {
                Items = translatedTextDto,
                Page = 1,
                PageSize = request.PageSize,
                TotalItems = 1
            };
        }
    }
}
