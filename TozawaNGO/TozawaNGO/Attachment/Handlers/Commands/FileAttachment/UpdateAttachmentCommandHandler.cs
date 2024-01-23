using MediatR;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Attachment.Converters;
using TozawaNGO.Attachment.Models;
using TozawaNGO.Attachment.Models.Commands;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Context;
using TozawaNGO.Services;

namespace TozawaNGO.Attachment.Handlers.Commands;

public class UpdateAttachmentCommandHandler(TozawangoDbContext context,
    IFileAttachmentConverter fileAttachmentConverter,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateAttachmentCommand, TozawaNGO.Models.Dtos.FileAttachmentDto>
{
    private readonly TozawangoDbContext _context = context;
    private readonly IFileAttachmentConverter _fileAttachmentConverter = fileAttachmentConverter;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<TozawaNGO.Models.Dtos.FileAttachmentDto> Handle(UpdateAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _context.FileAttachments
            .Include(x => x.Owners)
            .Include(x => x.FileAttachmentType)
            .FirstOrDefaultAsync(x => x.Id == request.Id);
        if (attachment == null)
        {
            throw new Exception($"Attachment with Id [{request.Id}] was not found.");
        }

        await Update(attachment, request);

        await _context.SaveChangesAsync();

        return _fileAttachmentConverter.Convert(attachment);
    }

    private async Task Update(FileAttachment fileAttachment, UpdateAttachmentCommand request)
    {
        var (byteArray, name, mimeType) = await FormFileConverter.GetConvertedByteByteArray(request.File);
        var isImage = FormFileConverter.IsImage(mimeType, byteArray);
        
        fileAttachment.MimeType = mimeType;
        fileAttachment.Name = name;
        fileAttachment.Size = byteArray.Length;
        fileAttachment.MetaData = request.MetaData;
        fileAttachment.Owners.RemoveAll(x => !request.OwnerIds.Contains(x.OwnerId));
        fileAttachment.FileAttachmentType = request.FileAttachmentType;
        fileAttachment.Owners.AddRange(request.OwnerIds.Where(x => fileAttachment.Owners.All(y => y.OwnerId != x)).Select(x =>
            new OwnerFileAttachment
            {
                FileAttachment = fileAttachment,
                OwnerId = x,
                FileAttachmentId = fileAttachment.Id
            }
            ));
    }
}
