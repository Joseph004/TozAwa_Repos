
using MediatR;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.Controllers;

public record CheckObjectTextCommand : IRequest<AddResponse<CheckObjectTextResponse>>
{
    public string Text { get; set; } = "";
    public UpdateEntityType EntityType { get; set; }
}