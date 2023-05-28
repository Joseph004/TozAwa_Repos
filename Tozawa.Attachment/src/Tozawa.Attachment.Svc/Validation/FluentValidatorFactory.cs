using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Tozawa.Attachment.Svc.Validation
{
    public class FluentValidatorFactory : IValidatorFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FluentValidatorFactory(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;
        public IValidator<T> GetValidator<T>() => (IValidator<T>)GetValidator(typeof(T));

        public IValidator GetValidator(Type type)
        {
            try
            {
                var validator = CreateInstance(typeof(IValidator<>).MakeGenericType(type));
                if (validator != null)
                {
                    return validator;
                }
                var baseType = type.GetTypeInfo().BaseType;
                if (baseType == null)
                {
                    throw new Exception();
                }

                try
                {
                    return CreateInstance(typeof(IValidator<>).MakeGenericType(baseType));
                }
                catch (Exception)
                {
                    return new GenericValidator();
                }

            }
            catch (Exception)
            {
                var baseType = type.GetTypeInfo().BaseType;
                if (baseType == null)
                {
                    throw;
                }

                try
                {
                    return CreateInstance(typeof(IValidator<>).MakeGenericType(baseType));
                }
                catch (Exception)
                {
                    return new GenericValidator();
                }

            }
        }

        private IValidator CreateInstance(Type validatorType) => _httpContextAccessor.HttpContext.RequestServices.GetService(validatorType) as IValidator;
    }

    public class GenericValidator : AbstractValidator<object>
    {

    }
}
