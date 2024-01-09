using MediatR;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Attachment.Converters;
using TozawaNGO.Attachment.Models;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Attachment.Models.Queries;
using TozawaNGO.Context;
using TozawaNGO.Models.Enums;
using TozawaNGO.Services;

namespace TozawaNGO.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentsQueryHandler : IRequestHandler<GetAttachmentsQuery, IEnumerable<TozawaNGO.Models.Dtos.FileAttachmentDto>>
{
    private readonly TozawangoDbContext _context;
    private readonly IFileAttachmentConverter _attachmentConverter;
    private readonly ICurrentUserService _currentUserService;

    public GetAttachmentsQueryHandler(TozawangoDbContext context,
        IFileAttachmentConverter attachmentConverter,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _attachmentConverter = attachmentConverter;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<TozawaNGO.Models.Dtos.FileAttachmentDto>> Handle(GetAttachmentsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<OwnerFileAttachment> query = _context.OwnerFileAttachments
            .Include(x => x.FileAttachment).ThenInclude(x => x.Owners)
            .Include(x => x.FileAttachment)
            .Where(x => x.OwnerId == request.OwnerId);

        query = query.Where(x => x.FileAttachment.FileAttachmentType != AttachmentType.Intern);

        if (request.FileAttachmentType != null)
        {
            query = query.Where(x => x.FileAttachment.FileAttachmentType == request.FileAttachmentType);
        }

        var fileAttachments = query.ToList().Select(x => x.FileAttachment).ToList();

        return await Task.FromResult(fileAttachments.Select(_attachmentConverter.Convert));
    }
}
