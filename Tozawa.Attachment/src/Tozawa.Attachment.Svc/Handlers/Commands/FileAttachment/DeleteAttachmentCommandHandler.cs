using MediatR;
using Microsoft.EntityFrameworkCore;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Models.Queries;
using Tozawa.Attachment.Svc.Services;

namespace Tozawa.Attachment.Svc.Handlers.Commands;

public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand>
{
    private readonly AttachmentContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAttachmentCommandHandler(AttachmentContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }
    public async Task<Unit> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _context.FileAttachments
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (attachment == null)
        {
            throw new Exception($"Attachment with Id [{request.Id}] was not found.");
        }

        _context.FileAttachments.Remove(attachment);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
