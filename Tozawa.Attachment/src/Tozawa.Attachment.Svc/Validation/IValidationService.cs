using FluentValidation.Results;

namespace Tozawa.Attachment.Svc.Validation
{
    public interface IValidationService
    {
        ValidationResult Validate<T>(T entity) where T : class;
    }
}
