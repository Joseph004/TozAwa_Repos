using MediatR;
using Microsoft.EntityFrameworkCore;
using OrleansHost.Attachment.Converters;
using OrleansHost.Attachment.Models;
using OrleansHost.Attachment.Models.Dtos;
using OrleansHost.Attachment.Models.Queries;
using OrleansHost.Auth.Services;
using OrleansHost.Context;
using OrleansHost.Models.Enums;
using OrleansHost.Services;

namespace OrleansHost.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentsQueryHandler(TozawangoDbContext context,
    IFileAttachmentConverter attachmentConverter,
    ICurrentUserService currentUserService) : IRequestHandler<GetAttachmentsQuery, IEnumerable<OrleansHost.Models.Dtos.FileAttachmentDto>>
{
    private readonly TozawangoDbContext _context = context;
    private readonly IFileAttachmentConverter _attachmentConverter = attachmentConverter;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<IEnumerable<OrleansHost.Models.Dtos.FileAttachmentDto>> Handle(GetAttachmentsQuery request, CancellationToken cancellationToken)
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
