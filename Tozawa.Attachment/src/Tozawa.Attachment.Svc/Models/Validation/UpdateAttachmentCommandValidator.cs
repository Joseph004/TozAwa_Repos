﻿using FluentValidation;
using Tozawa.Attachment.Svc.Models.Commands;

namespace Tozawa.Attachment.Svc.Models.Validation
{
    
    public class UpdateAttachmentCommandValidator : AbstractValidator<UpdateAttachmentCommand>
    {
        public UpdateAttachmentCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.File).NotEmpty();
            RuleFor(x => x.OwnerIds).NotEmpty();
        }
    }
}