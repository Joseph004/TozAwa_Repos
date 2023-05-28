using FluentValidation.Results;

namespace Tozawa.Language.Svc.Validation
{
    public interface IValidationService
    {
        ValidationResult Validate<T>(T entity) where T : class;
    }
}
