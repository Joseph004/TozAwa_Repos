using MediatR;
using Microsoft.EntityFrameworkCore;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Converters;
using Tozawa.Attachment.Svc.Models.Dtos;
using Tozawa.Attachment.Svc.Models.Queries;
using Tozawa.Attachment.Svc.Services;

namespace Tozawa.Attachment.Svc.Handlers.Queries.FileAttachment;

public class GetAttachmentsByFileTypeQueryHandler : IRequestHandler<GetAttachmentsByFileTypeQuery, IEnumerable<FileAttachmentDto>>
{
    private readonly AttachmentContext _context;
    private readonly IFileAttachmentConverter _attachmentConverter;
    private readonly ICurrentUserService _currentUserService;

    public GetAttachmentsByFileTypeQueryHandler(AttachmentContext context,
        IFileAttachmentConverter attachmentConverter,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _attachmentConverter = attachmentConverter;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<FileAttachmentDto>> Handle(GetAttachmentsByFileTypeQuery request, CancellationToken cancellationToken)
    {
        var fileAttachments = await _context.OwnerFileAttachments
            .Include(x => x.FileAttachment).ThenInclude(x => x.Owners)
            .Include(x => x.FileAttachment)
            .Where(x => x.OwnerId == request.OwnerId
                && x.FileAttachment.FileAttachmentType == request.AttachementType
                && (_currentUserService.IsRoot() || x.FileAttachment.OrganizationId == _currentUserService.User.OrganizationId))
            .Select(x => x.FileAttachment)
            .ToListAsync();

        return fileAttachments.Select(_attachmentConverter.Convert);
    }
}
