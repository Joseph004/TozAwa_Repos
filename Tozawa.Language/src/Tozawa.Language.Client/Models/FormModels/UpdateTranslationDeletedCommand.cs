using System;

namespace Tozawa.Language.Client.Models.FormModels
{
    public class UpdateTranslationDeletedCommand
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
    }
}
