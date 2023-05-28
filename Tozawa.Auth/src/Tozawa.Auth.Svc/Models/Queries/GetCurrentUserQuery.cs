using System;
using FluentValidation;
using MediatR;
using Tozawa.Auth.Svc.Models.Dtos;

namespace Tozawa.Auth.Svc.Models.Queries
{
    public class GetCurrentUserQuery : IRequest<CurrentUserDto>
    {
        public Guid Oid { get; set; }
        public GetCurrentUserQuery(Guid oid)
        {
            Oid = oid;
        }
    }

    
    public class GetCurrentUserQueryValidator : AbstractValidator<GetCurrentUserQuery>
    {
        public GetCurrentUserQueryValidator()
        {
            RuleFor(x => x.Oid).NotEmpty();
        }
    }
}