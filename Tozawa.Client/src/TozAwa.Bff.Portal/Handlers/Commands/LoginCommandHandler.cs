using System.Net;
using FluentValidation;
using MediatR;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Handlers.Commands
{
    public class LoginCommand : IRequest<AddResponse<LoginResponseDto>>
    {
        public bool LoginAsRoot { get; set; } = false;
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public byte[] Content { get; set; }
    }
    public class LoginCommandFluentValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandFluentValidator()
        {
            RuleFor(x => x.LoginAsRoot).NotNull();

            RuleFor(x => x.Email)
            .Cascade(CascadeMode.Continue)
            .NotNull()
            .EmailAddress()
            .WithMessage("A valid email is required")
            .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
            .WithMessage("A valid email is required")
            .MustAsync(async (value, cancellationToken) => await IsUniqueAsync(value))
            .When(y => !y.LoginAsRoot);

            RuleFor(x => x.UserName)
            .NotNull()
            .WithMessage("Username cannot be empty")
            .MinimumLength(6).WithMessage("Username length must be at least 6.")
            .MaximumLength(30).WithMessage("Your password length must not exceed 30.")
            .When(y => y.LoginAsRoot);

            RuleFor(x => x.Content).NotNull();
        }
        private async Task<bool> IsUniqueAsync(string email)
        {
            await Task.Delay(1);
            return email.ToLower() != "test@test.com";
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AddResponse<LoginResponseDto>>
    {
        private readonly ITozAwaAuthHttpClient _tozAwaAuthHttpClient;
        private readonly ITranslationService _translationService;
        private readonly IGoogleService _googleService;
        public LoginCommandHandler(ITozAwaAuthHttpClient tozAwaAuthHttpClient, ITranslationService translationService, IGoogleService googleService)
        {
            _tozAwaAuthHttpClient = tozAwaAuthHttpClient;
            _translationService = translationService;
            _googleService = googleService;
        }

        public async Task<AddResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var validator = new LoginCommandFluentValidator();

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
                response = request.LoginAsRoot ? await _tozAwaAuthHttpClient.Post<LoginResponseDto>("authenticate/root", request) : await _tozAwaAuthHttpClient.Post<LoginResponseDto>("authenticate/signin", request);
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

            return await Task.FromResult(new AddResponse<LoginResponseDto>(true, await _translationService.GetHttpStatusText(HttpStatusCode.OK), HttpStatusCode.OK, response));
        }
    }


}

