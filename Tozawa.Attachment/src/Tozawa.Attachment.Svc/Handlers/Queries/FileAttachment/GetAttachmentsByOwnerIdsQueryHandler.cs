using MediatR;
using Microsoft.EntityFrameworkCore;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Converters;
using Tozawa.Attachment.Svc.Models.Dtos;
using Tozawa.Attachment.Svc.Models.Queries;
using Tozawa.Attachment.Svc.Services;

namespace Tozawa.Attachment.Svc.Handlers.Queries.FileAttachment;

public class GetAttachmentsByOwnerIdsQueryHandler : IRequestHandler<GetAttachmentsByOwnerIdsQuery, IEnumerable<TravlingFileAttachments>>
{
    private readonly AttachmentContext _context;
    private readonly IFileAttachmentConverter _fileAttachmentConverter;
    private readonly ICurrentUserService _currentUserService;

    public GetAttachmentsByOwnerIdsQueryHandler(AttachmentContext context,
        IFileAttachmentConverter fileAttachmentConverter,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _fileAttachmentConverter = fileAttachmentConverter;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<TravlingFileAttachments>> Handle(GetAttachmentsByOwnerIdsQuery request, CancellationToken cancellationToken)
    {
        var ownerFileAttachments = await _context.OwnerFileAttachments
                .Include(x => x.FileAttachment).ThenInclude(x => x.Owners)
                .Include(x => x.FileAttachment)
                .Where(x => request.OwnerIds.Contains(x.OwnerId)
                    && (_currentUserService.IsRoot() || x.FileAttachment.OrganizationId == _currentUserService.User.OrganizationId))
                .ToListAsync();

        var groupedOwnerFileAttachments = ownerFileAttachments.GroupBy(ofa => ofa.OwnerId);

        return groupedOwnerFileAttachments
            .Select(x =>
                new TravlingFileAttachments
                {
                    OwnerId = x.Key,
                    Attachments = x.Select(y => _fileAttachmentConverter.Convert(y.FileAttachment)).ToList()
                });
    }
}
