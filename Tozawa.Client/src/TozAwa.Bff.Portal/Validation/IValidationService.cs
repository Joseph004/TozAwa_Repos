using FluentValidation.Results;

namespace Tozawa.Bff.Portal.Validation
{
    public interface IValidationService
    {
        ValidationResult Validate<T>(T entity) where T : class;
    }
}
