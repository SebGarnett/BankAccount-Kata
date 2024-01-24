using Kata_Bank_Account.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kata_Bank_Account.API.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        public ApiExceptionFilterAttribute()
        {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                {typeof(Application.Common.Exceptions.ValidationException), HandleValidationException},
                {typeof(BadHttpRequestException), HandleBadHttpRequestException},
                {typeof(AccountNotDeletedException), HandleAccountNotDeletedException}
            };
        }

       

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            var type = context.Exception.GetType();
            if (!_exceptionHandlers.ContainsKey(type)) return;
            _exceptionHandlers[type].Invoke(context);
        }

        private static void HandleValidationException(ExceptionContext context)
        {
            var exception = (Application.Common.Exceptions.ValidationException)context.Exception;

            var details = new ValidationProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail = exception.Message
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private static void HandleBadHttpRequestException(ExceptionContext context)
        {
            var exception = (BadHttpRequestException)context.Exception;

            var details = new ValidationProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail = exception.Message
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private static void HandleAccountNotDeletedException(ExceptionContext context)
        {
            var exception = (AccountNotDeletedException)context.Exception;

            var details = new ValidationProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail = exception.Message
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }
    }
}
