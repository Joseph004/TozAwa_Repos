using MediatR;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Converters;
using Tozawa.Attachment.Svc.Models.Commands;
using Tozawa.Attachment.Svc.Models.Dtos;

namespace Tozawa.Attachment.Svc.Handlers.Commands;

public class AddAttachmentCommandHandler : IRequestHandler<AddAttachmentCommand, FileAttachmentDto>
{
    private readonly AttachmentContext _context;
    private readonly IFileAttachmentCreator _fileAttachmentCreator;
    private readonly IFileAttachmentConverter _fileAttachmentConverter;

    public AddAttachmentCommandHandler(AttachmentContext context, 
        IFileAttachmentCreator fileAttachmentCreator, 
        IFileAttachmentConverter fileAttachmentConverter)
    {
        _context = context;
        _fileAttachmentCreator = fileAttachmentCreator;
        _fileAttachmentConverter = fileAttachmentConverter;
    }

    public async Task<FileAttachmentDto> Handle(AddAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _fileAttachmentCreator.Create(request);
        
        _context.FileAttachments.Add(attachment);
        await _context.SaveChangesAsync();
        
        return _fileAttachmentConverter.Convert(attachment);
    }
}
