using System;
using FluentValidation;
using MediatR;
using OrleansHost.Auth.Models.Dtos;

namespace OrleansHost.Auth.Models.Queries
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