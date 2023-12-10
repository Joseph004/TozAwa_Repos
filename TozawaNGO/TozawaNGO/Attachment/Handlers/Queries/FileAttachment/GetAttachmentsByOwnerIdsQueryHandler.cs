using MediatR;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Attachment.Converters;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Attachment.Models.Queries;
using TozawaNGO.Context;

namespace TozawaNGO.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentsByOwnerIdsQueryHandler : IRequestHandler<GetAttachmentsByOwnerIdsQuery, List<OwnerAttachments>>
{
    private readonly TozawangoDbContext _context;
    private readonly IFileAttachmentConverter _fileAttachmentConverter;

    public GetAttachmentsByOwnerIdsQueryHandler(TozawangoDbContext context,
        IFileAttachmentConverter fileAttachmentConverter)
    {
        _context = context;
        _fileAttachmentConverter = fileAttachmentConverter;
    }

    public async Task<List<OwnerAttachments>> Handle(GetAttachmentsByOwnerIdsQuery request, CancellationToken cancellationToken)
    {
        var ownerFileAttachments = await _context.OwnerFileAttachments
                .Include(x => x.FileAttachment).ThenInclude(x => x.Owners)
                .Include(x => x.FileAttachment)
                .Where(x => request.OwnerIds.Contains(x.OwnerId))
                .ToListAsync(cancellationToken: cancellationToken);

        var groupedOwnerFileAttachments = ownerFileAttachments.GroupBy(ofa => ofa.OwnerId);

        return groupedOwnerFileAttachments
            .Select(x =>
                new OwnerAttachments
                {
                    OwnerId = x.Key,
                    Attachments = x.Select(y => _fileAttachmentConverter.Convert(y.FileAttachment)).ToList()
                }).ToList();
    }
}
