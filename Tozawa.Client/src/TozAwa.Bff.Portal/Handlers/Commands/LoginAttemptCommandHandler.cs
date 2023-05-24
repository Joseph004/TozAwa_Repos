using System.Net;
using FluentValidation;
using MediatR;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Handlers.Commands
{
    public class LoginAttemptCommand : IRequest<AddResponse<LoginResponseDto>>
    {
        public string UserName { get; set; } = "";
    }

    public class LoginAttemptCommandHandler : IRequestHandler<LoginAttemptCommand, AddResponse<LoginResponseDto>>
    {
        private readonly ITozAwaAuthHttpClient _tozAwaAuthHttpClient;
        private readonly ITranslationService _translationService;
        public LoginAttemptCommandHandler(ITozAwaAuthHttpClient tozAwaAuthHttpClient, ITranslationService translationService)
        {
            _tozAwaAuthHttpClient = tozAwaAuthHttpClient;
            _translationService = translationService;
        }

        public async Task<AddResponse<LoginResponseDto>> Handle(LoginAttemptCommand request, CancellationToken cancellationToken)
        {
            var response = await _tozAwaAuthHttpClient.Post<LoginResponseDto>($"authenticate/root/{request.UserName}", request);

            if (response == null)
            {
                return await Task.FromResult(new AddResponse<LoginResponseDto>(false, UpdateMessages.Error, HttpStatusCode.BadRequest, new LoginResponseDto
                {
                    ErrorMessage = UpdateMessages.Error,
                    LoginSuccess = false
                }));
            }

            return await Task.FromResult(new AddResponse<LoginResponseDto>(true, await _translationService.GetHttpStatusText(HttpStatusCode.OK), HttpStatusCode.OK, response));
        }
    }


}

