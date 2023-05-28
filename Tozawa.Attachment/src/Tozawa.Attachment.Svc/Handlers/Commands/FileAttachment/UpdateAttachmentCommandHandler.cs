using MediatR;
using Microsoft.EntityFrameworkCore;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Context.Models;
using Tozawa.Attachment.Svc.Converters;
using Tozawa.Attachment.Svc.Models.Commands;
using Tozawa.Attachment.Svc.Models.Dtos;
using Tozawa.Attachment.Svc.Services;

namespace Tozawa.Attachment.Svc.Handlers.Commands;

public class UpdateAttachmentCommandHandler : IRequestHandler<UpdateAttachmentCommand, FileAttachmentDto>
{
    private readonly AttachmentContext _context;
    private readonly IFileAttachmentConverter _fileAttachmentConverter;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAttachmentCommandHandler(AttachmentContext context,
        IFileAttachmentConverter fileAttachmentConverter,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _fileAttachmentConverter = fileAttachmentConverter;
        _currentUserService = currentUserService;
    }

    public async Task<FileAttachmentDto> Handle(UpdateAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _context.FileAttachments
            .Include(x => x.Owners)
            .Include(x => x.FileAttachmentType)
            .FirstOrDefaultAsync(x => x.Id == request.Id
                && _currentUserService.User.OrganizationId == x.OrganizationId);
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
