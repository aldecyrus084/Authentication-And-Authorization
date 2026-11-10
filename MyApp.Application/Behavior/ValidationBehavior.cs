using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MyApp.Application.DTOs;

namespace MyApp.Application.Behavior
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next(cancellationToken);

            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = results.SelectMany(r => r.Errors).Where(e => e is not null).ToList();

            if (failures.Count == 0)
                return await next(cancellationToken);

            if (TryCreateFailureResponse(failures, out var failureResponse))
                return failureResponse;

            throw new ValidationException(failures);
        }

        private static bool TryCreateFailureResponse(IEnumerable<ValidationFailure> failures, out TResponse response)
        {
            response = default!;

            var responseType = typeof(TResponse);
            if (!responseType.IsGenericType || responseType.GetGenericTypeDefinition() != typeof(GenericResponseDTO<>))
                return false;

            var instance = Activator.CreateInstance(responseType)!;
            var message = string.Join(" ", failures.Select(f => f.ErrorMessage).Distinct());

            responseType.GetProperty(nameof(GenericResponseDTO<object>.isSuccess))!.SetValue(instance, false);
            responseType.GetProperty(nameof(GenericResponseDTO<object>.message))!.SetValue(instance, message);

            response = (TResponse)instance;
            return true;
        }
    }
}
