using FluentValidation;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Models.FormModels
{
    public class AddFeatureRequest
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public string Description { get; set; } = "";
        public List<TranslationRequest> TextTranslations { get; set; } = [];
        public List<TranslationRequest> DescriptionTranslations { get; set; } = [];
    }
    public class AddFeatureRequestFluentValidator : AbstractValidator<AddFeatureRequest>
    {
        public AddFeatureRequestFluentValidator(ITranslationService translationService)
        {
            RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();

            RuleFor(x => x.Text)
            .NotEmpty()
            .NotNull();

            RuleFor(x => x.Description)
           .NotEmpty()
           .NotNull();
        }
    }
}