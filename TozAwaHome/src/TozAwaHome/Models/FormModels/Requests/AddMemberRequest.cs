
using System.Collections.Generic;

namespace TozAwaHome.Models.FormModels
{
    public class AddMemberRequest
    {
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Description { get; set; } = "";
        public List<TranslationRequest> DescriptionTranslations { get; set; } = new List<TranslationRequest>();
    }
}