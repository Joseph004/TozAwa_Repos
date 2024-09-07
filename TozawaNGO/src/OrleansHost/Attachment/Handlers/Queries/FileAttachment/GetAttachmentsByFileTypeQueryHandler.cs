using MediatR;
using Microsoft.EntityFrameworkCore;
using OrleansHost.Attachment.Converters;
using Grains.Attachment.Models.Dtos;
using OrleansHost.Attachment.Models.Queries;
using Grains.Auth.Services;
using Grains.Context;
using Grains.Services;

namespace OrleansHost.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentsByFileTypeQueryHandler(TozawangoDbContext context,
    IFileAttachmentConverter attachmentConverter,
    ICurrentUserService currentUserService) : IRequestHandler<GetAttachmentsByFileTypeQuery, IEnumerable<Grains.Models.Dtos.FileAttachmentDto>>
{
    private readonly TozawangoDbContext _context = context;
    private readonly IFileAttachmentConverter _attachmentConverter = attachmentConverter;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<IEnumerable<Grains.Models.Dtos.FileAttachmentDto>> Handle(GetAttachmentsByFileTypeQuery request, CancellationToken cancellationToken)
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
