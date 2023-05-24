using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Helpers;
using Tozawa.Language.Svc.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Tozawa.Language.Svc.Controllers.XliffControllers
{
    public class GetXliffDistributionFilesLogsQueryPaged : IRequest<PagedDto<XliffDistributionFilesDto>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public XliffFileState XliffFileState { get; set; }

        public GetXliffDistributionFilesLogsQueryPaged(Dictionary<string, StringValues> queryParameters)
        {
            if (queryParameters == null)
            {
                return;
            }

            if (queryParameters.ContainsKey(nameof(Page)) && int.TryParse(queryParameters[nameof(Page)], out var page) && page >= 1)
            {
                Page = page;
            }

            if (queryParameters.ContainsKey(nameof(PageSize)) && int.TryParse(queryParameters[nameof(PageSize)], out var pageSize) && pageSize >= 1)
            {
                PageSize = pageSize;
            }

            if (queryParameters.ContainsKey(nameof(StartDate)) && DateTime.TryParse(queryParameters[nameof(StartDate)], out var startDate))
            {
                StartDate = startDate;
            }

            if (queryParameters.ContainsKey(nameof(EndDate)) && DateTime.TryParse(queryParameters[nameof(EndDate)], out var endDate))
            {
                EndDate = endDate;
            }

            if (queryParameters.ContainsKey(nameof(XliffFileState)))
            {
                if (queryParameters[nameof(XliffFileState)] == "Export")
                    XliffFileState = XliffFileState.Export;
                if (queryParameters[nameof(XliffFileState)] == "Import")
                    XliffFileState = XliffFileState.Import;
            }
        }
    }

    public class GetXliffDistributionFilesLogsQueryPagedValidator : AbstractValidator<GetXliffDistributionFilesLogsQueryPaged>
    {
        public GetXliffDistributionFilesLogsQueryPagedValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();
            RuleFor(x => x.XliffFileState).IsInEnum().NotNull();
            RuleFor(x => x.Page).NotEmpty();
            RuleFor(x => x.PageSize).NotEmpty();
        }
    }

    public class GetXliffDistributionFilesLogsQueryPagedHandler : IRequestHandler<GetXliffDistributionFilesLogsQueryPaged, PagedDto<XliffDistributionFilesDto>>
    {
        private readonly LanguageContext _context;
        public GetXliffDistributionFilesLogsQueryPagedHandler(LanguageContext context) => _context = context;

        public async Task<PagedDto<XliffDistributionFilesDto>> Handle(GetXliffDistributionFilesLogsQueryPaged request, CancellationToken cancellationToken)
        {
            var fileIds = await _context
                .XliffDistributionFiles
                .Where(x => x.CreatedAt >= request.StartDate && x.CreatedAt <= request.EndDate && x.FileState == request.XliffFileState)
                .Select(x => x.FileId)
                .Distinct()
                .ToListAsync();

            var xliffDistributionFiles =
                await _context.XliffDistributionFiles
                .Include(x => x.SourceLanguage)
                .Include(x => x.TargetLanguage)
                .Where(x => x.FileState == request.XliffFileState && fileIds.Contains(x.FileId))
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return new PagedDto<XliffDistributionFilesDto>
            {
                Page = request.Page,
                TotalItems = fileIds.Count,
                PageSize = request.PageSize,
                Items = xliffDistributionFiles.Select(Convert)
            };
        }

        private XliffDistributionFilesDto Convert(XliffDistributionFiles file) => new()
        {
            Id = file.Id,
            CreatedAt = file.CreatedAt,
            FileId = file.FileId,
            CreatedBy = file.CreatedBy,
            FileState = file.FileState,
            FileName = file.FileName,
            FileStateDescription = file.FileState.GetEnumDescription(),
            NumberOfTranslations = file.NumberOfTranslations,
            NumberOfWordsSentInSourceLanguage = file.NumberOfWordsSentInSourceLanguage,
            RequestedDeliveryDate = file.RequestedDeliveryDate,
            SourceLanguageLongName = file.SourceLanguage.LongName,
            TargetLanguageLongName = file.TargetLanguage.LongName
        };
    }
}

