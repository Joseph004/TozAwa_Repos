using FluentValidation;
using MediatR;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Handlers.Queries
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

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
    {
        private readonly ITozAwaAuthHttpClient _tozAwaAuthHttpClient;
        public GetCurrentUserQueryHandler(ITozAwaAuthHttpClient tozAwaAuthHttpClient)
        {
            _tozAwaAuthHttpClient = tozAwaAuthHttpClient;
        }

        public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var response = await _tozAwaAuthHttpClient.Get<CurrentUserDto>($"authenticate/current/{request.Oid}");
            return await Task.FromResult(response ?? new CurrentUserDto());
        }
    }
}