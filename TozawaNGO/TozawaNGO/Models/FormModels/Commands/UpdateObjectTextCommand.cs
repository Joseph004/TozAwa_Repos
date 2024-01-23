using System;
using TozawaNGO.Models.Enums;

namespace TozawaNGO.Models.FormModels
{
    public class UpdateObjectTextCommand(UpdateEntityType type, Guid id)
    {
        public UpdateEntityType EntityType { get; set; } = type;
        public Guid Id { get; set; } = id;
        public string Description { get; set; } = "";
        public Guid DescriptionTextId { get; set; }
    }
}