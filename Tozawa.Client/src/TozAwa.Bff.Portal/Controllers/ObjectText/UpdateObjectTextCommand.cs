

using MediatR;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.Controllers;

public record UpdateObjectTextCommand : IRequest<UpdateResponse>
{
#nullable enable
    public Guid Id { get; set; }
    public Guid? TextId { get; set; }
    public string? Text { get; set; }
    public UpdateEntityType EntityType { get; set; }
    public Guid? DescriptionTextId { get; set; }
    public string Description { get; set; } = "";
}
