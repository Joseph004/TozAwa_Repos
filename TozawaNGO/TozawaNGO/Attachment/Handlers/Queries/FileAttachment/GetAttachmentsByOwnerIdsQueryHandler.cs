using MediatR;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Attachment.Converters;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Attachment.Models.Queries;
using TozawaNGO.Context;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Services;

namespace TozawaNGO.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentsByOwnerIdsQueryHandler : IRequestHandler<GetAttachmentsByOwnerIdsQuery, IEnumerable<AnalyseFileAttachments>>
{
    private readonly TozawangoDbContext _context;
    private readonly IFileAttachmentConverter _fileAttachmentConverter;
    private readonly ICurrentUserService _currentUserService;

    public GetAttachmentsByOwnerIdsQueryHandler(TozawangoDbContext context,
        IFileAttachmentConverter fileAttachmentConverter,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _fileAttachmentConverter = fileAttachmentConverter;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<AnalyseFileAttachments>> Handle(GetAttachmentsByOwnerIdsQuery request, CancellationToken cancellationToken)
    {
        var ownerFileAttachments = await _context.OwnerFileAttachments
                .Include(x => x.FileAttachment).ThenInclude(x => x.Owners)
                .Include(x => x.FileAttachment)
                .Where(x => request.OwnerIds.Contains(x.OwnerId))
                .ToListAsync();

        var groupedOwnerFileAttachments = ownerFileAttachments.GroupBy(ofa => ofa.OwnerId);

        return groupedOwnerFileAttachments
            .Select(x =>
                new AnalyseFileAttachments
                {
                    OwnerId = x.Key,
                    Attachments = x.Select(y => _fileAttachmentConverter.Convert(y.FileAttachment)).ToList()
                });
    }
}
