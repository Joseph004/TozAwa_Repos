﻿using MediatR.Pipeline;

namespace Tozawa.Attachment.Svc.Validation
{
    public class ValidationRequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : class
    {
        private readonly IValidationService _validationService;

        public ValidationRequestPreProcessor(IValidationService validationService)
        {
            _validationService = validationService;
        }
        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            _validationService.Validate(request);
            return Task.CompletedTask;
        }
    }
}
