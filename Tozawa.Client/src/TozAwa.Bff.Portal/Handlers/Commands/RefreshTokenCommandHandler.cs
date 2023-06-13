using System.IdentityModel.Tokens.Jwt;
using System.Net;
using FluentValidation;
using MediatR;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Handlers.Commands
{
    public class RefreshTokenCommand : IRequest<AddResponse<LoginResponseDto>>
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public RefreshTokenCommand(RefreshTokenDto refresh)
        {
            Token = refresh.Token;
            RefreshToken = refresh.RefreshToken;
        }
    }
    public class RefreshTokenCommandFluentValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandFluentValidator()
        {
            RuleFor(x => x.Token).NotNull().NotEmpty();

            RuleFor(x => x.RefreshToken).NotNull().NotEmpty();
        }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AddResponse<LoginResponseDto>>
    {
        private readonly ITozAwaAuthHttpClient _tozAwaAuthHttpClient;
        private readonly ITranslationService _translationService;
        private readonly IGoogleService _googleService;
        private readonly IUserTokenService _userTokenService;
        public RefreshTokenCommandHandler(ITozAwaAuthHttpClient tozAwaAuthHttpClient, ITranslationService translationService, IUserTokenService userTokenService, IGoogleService googleService)
        {
            _tozAwaAuthHttpClient = tozAwaAuthHttpClient;
            _translationService = translationService;
            _googleService = googleService;
            _userTokenService = userTokenService;
        }

        public async Task<AddResponse<LoginResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var validator = new RefreshTokenCommandFluentValidator();

            var requestValidate = validator.Validate(request);

            if (!requestValidate.IsValid)
            {
                return await Task.FromResult(new AddResponse<LoginResponseDto>(false, requestValidate.Errors.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).FirstOrDefault().ErrorMessage, HttpStatusCode.BadRequest, new LoginResponseDto
                {
                    ErrorMessage = requestValidate.Errors.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).FirstOrDefault().ErrorMessage,
                    LoginSuccess = false
                }));
            }
            var response = new LoginResponseDto();
            try
            {
                response = await _tozAwaAuthHttpClient.Post<LoginResponseDto>("token/refresh", request);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            if (response == null)
            {
                return await Task.FromResult(new AddResponse<LoginResponseDto>(false, requestValidate.Errors.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).FirstOrDefault().ErrorMessage, HttpStatusCode.BadRequest, new LoginResponseDto
                {
                    ErrorMessage = requestValidate.Errors.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).FirstOrDefault().ErrorMessage,
                    LoginSuccess = false
                }));
            }
            response.Token = _userTokenService.GenerateToken(response.Token);
            return await Task.FromResult(new AddResponse<LoginResponseDto>(true, await _translationService.GetHttpStatusText(HttpStatusCode.OK), HttpStatusCode.OK, response));
        }
    }
}

