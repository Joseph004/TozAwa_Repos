﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Context.Models;
using Tozawa.Attachment.Svc.Converters;
using Tozawa.Attachment.Svc.Helpers;
using Tozawa.Attachment.Svc.Models.Dtos;
using Tozawa.Attachment.Svc.Models.Queries;
using Tozawa.Attachment.Svc.Services;

namespace Tozawa.Attachment.Svc.Handlers.Queries.FileAttachment;

public class GetAttachmentsQueryHandler : IRequestHandler<GetAttachmentsQuery, IEnumerable<FileAttachmentDto>>
{
    private readonly AttachmentContext _context;
    private readonly IFileAttachmentConverter _attachmentConverter;
    private readonly ICurrentUserService _currentUserService;

    public GetAttachmentsQueryHandler(AttachmentContext context,
        IFileAttachmentConverter attachmentConverter,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _attachmentConverter = attachmentConverter;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<FileAttachmentDto>> Handle(GetAttachmentsQuery request, CancellationToken cancellationToken)
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

        return fileAttachments.Select(_attachmentConverter.Convert);
    }
}
