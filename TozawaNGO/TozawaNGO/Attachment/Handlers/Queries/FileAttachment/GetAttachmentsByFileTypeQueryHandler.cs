using MediatR;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Attachment.Converters;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Attachment.Models.Queries;
using TozawaNGO.Context;
using TozawaNGO.Services;

namespace TozawaNGO.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentsByFileTypeQueryHandler : IRequestHandler<GetAttachmentsByFileTypeQuery, IEnumerable<FileAttachmentDto>>
{
    private readonly TozawangoDbContext _context;
    private readonly IFileAttachmentConverter _attachmentConverter;
    private readonly ICurrentUserService _currentUserService;

    public GetAttachmentsByFileTypeQueryHandler(TozawangoDbContext context,
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
                && x.FileAttachment.FileAttachmentType == request.AttachementType)
            .Select(x => x.FileAttachment)
            .ToListAsync();

        return fileAttachments.Select(_attachmentConverter.Convert);
    }
}
