using MediatR;
using TozawaNGO.Attachment.Converters;
using TozawaNGO.Attachment.Models.Commands;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Context;

namespace TozawaNGO.Attachment.Handlers.Commands;

public class AddAttachmentCommandHandler : IRequestHandler<AddAttachmentCommand, FileAttachmentDto>
{
    private readonly TozawangoDbContext _context;
    private readonly IFileAttachmentCreator _fileAttachmentCreator;
    private readonly IFileAttachmentConverter _fileAttachmentConverter;

    public AddAttachmentCommandHandler(TozawangoDbContext context, 
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
