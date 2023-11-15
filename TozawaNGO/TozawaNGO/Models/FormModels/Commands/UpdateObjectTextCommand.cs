using System;
using TozawaNGO.Models.Enums;

namespace TozawaNGO.Models.FormModels
{
    public class UpdateObjectTextCommand
    {
        public UpdateObjectTextCommand(UpdateEntityType type, Guid id)
        {
            EntityType = type;
            Id = id;
        }
        public UpdateEntityType EntityType { get; set; }
        public Guid Id { get; set; }
        public string Description { get; set; } = "";
        public Guid DescriptionTextId { get; set; }
    }
}