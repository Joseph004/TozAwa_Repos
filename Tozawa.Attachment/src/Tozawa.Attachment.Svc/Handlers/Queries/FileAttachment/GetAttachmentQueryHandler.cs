using MediatR;
using Microsoft.EntityFrameworkCore;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Converters;
using Tozawa.Attachment.Svc.Models.Dtos;
using Tozawa.Attachment.Svc.Models.Queries;
using Tozawa.Attachment.Svc.Services;

namespace Tozawa.Attachment.Svc.Handlers.Queries.FileAttachment;

public class GetAttachmentQueryHandler : IRequestHandler<GetAttachmentQuery, FileAttachmentDto>
{
    private readonly AttachmentContext _context;
    private readonly IFileAttachmentConverter _attachmentConverter;
    private readonly ICurrentUserService _currentUserService;

    public GetAttachmentQueryHandler(AttachmentContext context,
        IFileAttachmentConverter attachmentConverter,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _attachmentConverter = attachmentConverter;
        _currentUserService = currentUserService;
    }

    public async Task<FileAttachmentDto> Handle(GetAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _context.FileAttachments
            .Include(x => x.Owners)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (attachment == null)
        {
            throw new Exception($"Attachment with Id [{request.Id}] was not found.");
        }

        return _attachmentConverter.Convert(attachment);
    }
}
