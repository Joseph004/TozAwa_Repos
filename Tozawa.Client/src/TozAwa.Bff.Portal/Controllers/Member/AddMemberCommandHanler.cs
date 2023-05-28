

using MediatR;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.Services;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.Controllers;

public class AddMemberCommandHandler : AddHandler, IRequestHandler<AddMemberCommand, AddResponse<MemberDto>>
{
    private readonly ILanguageText _LanguageText;
    private readonly IMemberConverter _converter;
    private readonly ITozAwaAuthHttpClient _client;

    public AddMemberCommandHandler(ILanguageText LanguageText, ITozAwaAuthHttpClient client, IMediator mediator, ILogger<PatchMemberCommand> logger, ICurrentUserService currentUserService, IMemberConverter converter)
        : base(mediator, currentUserService, logger)
    {
        _LanguageText = LanguageText;
        _converter = converter;
        _client = client;
    }
    public async Task<AddResponse<MemberDto>> Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var translations = new List<ImportTranslationTextDto>();
            if (request.DescriptionTranslations.Any())
            {
                translations = request.DescriptionTranslations.Where(x => !string.IsNullOrEmpty(x.Text)).Select(x => new ImportTranslationTextDto
                {
                    LanguageId = x.LanguageId,
                    Text = x.Text
                }).ToList();
            }

            var descriptionTextId = await _LanguageText.Add(_logger, request, translations, request.Description, true);

            var httpRequest = new Models.Request.Backend.AddMemberRequest
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DescriptionId = descriptionTextId.HasValue ? descriptionTextId.Value : Guid.Empty
            };
            var uri = $"member";

            var entityAdded = await _client.Post<Models.Dtos.Backend.MemberDto>(uri, httpRequest);

            if (entityAdded == null)
            {
                return new AddResponse<MemberDto>(false, UpdateMessages.EntityCreatedError, System.Net.HttpStatusCode.InternalServerError, null);
            }

            await HandleAdd(UpdateEntityType.Member, entityAdded);

            return new AddResponse<MemberDto>(true, UpdateMessages.EntityCreatedSuccess, System.Net.HttpStatusCode.OK, _converter.Convert(entityAdded));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add member");
            return new AddResponse<MemberDto>(false, UpdateMessages.EntityCreatedError, System.Net.HttpStatusCode.InternalServerError, null);
        }
    }
}