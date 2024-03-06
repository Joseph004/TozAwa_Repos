using MediatR;
using Microsoft.EntityFrameworkCore;
using OrleansHost.Attachment.Converters;
using Grains.Attachment.Models.Dtos;
using OrleansHost.Attachment.Models.Queries;
using Grains.Context;

namespace OrleansHost.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentsByOwnerIdsQueryHandler(TozawangoDbContext context,
    IFileAttachmentConverter fileAttachmentConverter) : IRequestHandler<GetAttachmentsByOwnerIdsQuery, List<OwnerAttachments>>
{
    private readonly TozawangoDbContext _context = context;
    private readonly IFileAttachmentConverter _fileAttachmentConverter = fileAttachmentConverter;

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
