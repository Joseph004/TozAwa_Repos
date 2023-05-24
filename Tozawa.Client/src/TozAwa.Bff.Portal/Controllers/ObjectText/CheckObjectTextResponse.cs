
using MediatR;
using Tozawa.Bff.Portal.Models.ResponseRequests;

namespace Tozawa.Bff.Portal.Controllers;

public record CheckObjectTextResponse : IRequest<AddResponse<CheckObjectTextResponse>>
{
    public bool TextExist { get; set; } = false;
}