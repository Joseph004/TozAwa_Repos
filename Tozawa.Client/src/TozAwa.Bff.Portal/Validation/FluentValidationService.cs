using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;


namespace Tozawa.Bff.Portal.Validation
{
    public class FluentValidationService : IValidationService
    {
        private readonly IValidatorFactory _validatorFactory;

        public FluentValidationService(IValidatorFactory validatorFactory) => _validatorFactory = validatorFactory;

        public ValidationResult Validate<T>(T entity) where T : class
        {
            if (!(_validatorFactory.GetValidator(entity.GetType()) is IValidator<T> validator))
            {
                return new ValidationResult();
            }
            var result = validator.Validate(entity);
            if (!result.IsValid)
            {
                throw new ArgumentNullException(JsonConvert.SerializeObject(result.Errors.ToDictionary(x => x.PropertyName, x => x.ErrorMessage)));
            }
            return result;
        }

    }
}
