
using System.Net;
using MediatR;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.Services;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.Controllers;

public partial class CheckObjectTextCommandHandler : IRequestHandler<CheckObjectTextCommand, AddResponse<CheckObjectTextResponse>>
{
    private readonly IMemberService _memberService;
    private readonly ILanguageService _languageService;
    private readonly ILogger<CheckObjectTextCommandHandler> _logger;

    public CheckObjectTextCommandHandler(ILanguageService LanguageService, IMemberService MemberService, ILogger<CheckObjectTextCommandHandler> logger)
    {
        _languageService = LanguageService;
        _logger = logger;
        _memberService = MemberService;
    }
    public async Task<AddResponse<CheckObjectTextResponse>> Handle(CheckObjectTextCommand request, CancellationToken cancellationToken)
    {
        try
        {
            CheckObjectTextResponse response = new();
            switch (request.EntityType)
            {
                case UpdateEntityType.Member:
                    var members = await _memberService.GetItems();
                    foreach (var item in members)
                    {
                        var description = _languageService.GetSync(item.DescriptionTextId);
                        if (description.Equals(request.Text, StringComparison.InvariantCultureIgnoreCase))
                        {
                            response.TextExist = true;
                        }
                    }
                    break;
                default:
                    break;
            }
            return new AddResponse<CheckObjectTextResponse>(true, UpdateMessages.Success, HttpStatusCode.OK, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get text");
            return new AddResponse<CheckObjectTextResponse>(false, UpdateMessages.EntityCreatedError, HttpStatusCode.InternalServerError, null);
        }

    }
}