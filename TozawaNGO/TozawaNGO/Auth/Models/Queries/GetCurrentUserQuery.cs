using System;
using FluentValidation;
using MediatR;
using TozawaNGO.Auth.Models.Dtos;

namespace TozawaNGO.Auth.Models.Queries
{
    public class GetCurrentUserQuery(Guid oid) : IRequest<CurrentUserDto>
    {
        public Guid Oid { get; set; } = oid;
    }


    public class GetCurrentUserQueryValidator : AbstractValidator<GetCurrentUserQuery>
    {
        public GetCurrentUserQueryValidator()
        {
            RuleFor(x => x.Oid).NotEmpty();
        }
    }
}