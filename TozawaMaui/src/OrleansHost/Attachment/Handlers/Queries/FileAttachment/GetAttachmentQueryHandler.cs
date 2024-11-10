using MediatR;
using Microsoft.EntityFrameworkCore;
using OrleansHost.Attachment.Converters;
using Grains.Attachment.Models.Dtos;
using OrleansHost.Attachment.Models.Queries;
using Grains.Context;
using Grains.Models.Dtos;
using Grains.Services;

namespace OrleansHost.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentQueryHandler(TozawangoDbContext context, IGoogleService googleService) : IRequestHandler<GetAttachmentQuery, AttachmentDownloadDto>
{
    private readonly TozawangoDbContext _context = context;
    private readonly IGoogleService _googleService = googleService;

    public async Task<AttachmentDownloadDto> Handle(GetAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _context.FileAttachments
            .Include(x => x.Owners)
            .FirstOrDefaultAsync(x => x.Id == request.Id) ?? throw new Exception($"Attachment with Id [{request.Id}] was not found.");
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
