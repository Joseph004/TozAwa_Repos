using MediatR;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Attachment.Converters;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Attachment.Models.Queries;
using TozawaNGO.Context;
using TozawaNGO.Services;

namespace TozawaNGO.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentQueryHandler : IRequestHandler<GetAttachmentQuery, FileAttachmentDto>
{
    private readonly TozawangoDbContext _context;
    private readonly IFileAttachmentConverter _attachmentConverter;
    private readonly ICurrentUserService _currentUserService;

    public GetAttachmentQueryHandler(TozawangoDbContext context,
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
