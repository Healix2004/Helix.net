using FluentValidation;
using Helix.Core.Bases;
using MediatR;
using System.Net;
using System.Reflection;

namespace Helix.Core.Bases.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                if (IsResponseType(typeof(TResponse)))
                {
                    return CreateValidationErrorResponse<TResponse>(failures);
                }

                throw new ValidationException(failures);
            }

            return await next();
        }

        private static bool IsResponseType(Type type)
        {
            return type.IsGenericType && 
                   type.GetGenericTypeDefinition() == typeof(Response<>);
        }

        private static TResponse CreateValidationErrorResponse<TResponse>(List<FluentValidation.Results.ValidationFailure> failures)
        {
            var responseType = typeof(TResponse).GetGenericArguments()[0];
            var errorMessages = failures.Select(f => f.ErrorMessage).ToList();
            var combinedMessage = string.Join("; ", errorMessages);

            // Create Response<T> instance using the constructor that takes (string message, bool succeeded)
            var responseGenericType = typeof(Response<>).MakeGenericType(responseType);
            var responseInstance = Activator.CreateInstance(
                responseGenericType,
                combinedMessage,
                false);

            if (responseInstance != null)
            {
                // Set Errors property
                var errorsProperty = responseGenericType.GetProperty("Errors");
                if (errorsProperty != null && errorsProperty.CanWrite)
                {
                    errorsProperty.SetValue(responseInstance, errorMessages);
                }

                // Set StatusCode property
                var statusCodeProperty = responseGenericType.GetProperty("StatusCode");
                if (statusCodeProperty != null && statusCodeProperty.CanWrite)
                {
                    statusCodeProperty.SetValue(responseInstance, HttpStatusCode.BadRequest);
                }

                return (TResponse)responseInstance;
            }

            throw new InvalidOperationException("Failed to create validation error response");
        }
    }
}

