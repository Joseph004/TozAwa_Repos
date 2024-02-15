using MediatR;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Attachment.Converters;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Attachment.Models.Queries;
using TozawaNGO.Context;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Services;

namespace TozawaNGO.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentQueryHandler(TozawangoDbContext context, IGoogleService googleService) : IRequestHandler<GetAttachmentQuery, AttachmentDownloadDto>
{
    private readonly TozawangoDbContext _context = context;
    private readonly IGoogleService _googleService = googleService;

    public async Task<AttachmentDownloadDto> Handle(GetAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _context.FileAttachments
            .Include(x => x.Owners)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (attachment == null)
        {
            throw new Exception($"Attachment with Id [{request.Id}] was not found.");
        }

        var stream = await _googleService.StreamFromGoogleFileByFileId(attachment.BlobId);
        var bytes = FileUtil.ReadAllBytesFromStream(stream);

        var attchmentResponse = new AttachmentDownloadDto
        {
            Content = bytes,
            MineType = attachment.MimeType,
            Name = attachment.Name
        };
        return attchmentResponse;
    }
}
