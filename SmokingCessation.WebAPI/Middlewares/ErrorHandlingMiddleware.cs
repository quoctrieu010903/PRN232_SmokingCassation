using System.Net;
using System.Text.Json;
using SmokingCessation.Core.CustomExceptionss;
using SmokingCessation.Core.Constants;
using System.ComponentModel.DataAnnotations;
using SmokingCessation.Application.Exceptions;

namespace SmokingCessation.WebAPI.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            object errorResponse;

            switch (ex)
            {
                case ErrorException errorEx:
                    response.StatusCode = errorEx.StatusCode;
                    errorResponse = new
                    {
                        statusCode = errorEx.StatusCode,
                        errorCode = errorEx.ErrorDetail.ErrorCode,
                        errorMessage = errorEx.ErrorDetail.ErrorMessage
                    };
                    break;

                case UnauthorizedException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse = new
                    {
                        statusCode = (int)HttpStatusCode.Unauthorized,
                        errorCode = ResponseCodeConstants.UNAUTHORIZED,
                        errorMessage = MessageConstants.UNAUTHORIZED
                    };
                    break;

                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new
                    {
                        statusCode = (int)HttpStatusCode.NotFound,
                        errorCode = ResponseCodeConstants.NOT_FOUND,
                        errorMessage = MessageConstants.NOT_FOUND
                    };
                    break;

                case ValidationException validationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new
                    {
                        statusCode = (int)HttpStatusCode.BadRequest,
                        errorCode = ResponseCodeConstants.INVALID_INPUT,
                        errorMessage = validationEx.Message
                    };
                    break;

                default:
                    _logger.LogError(ex, "An unhandled exception occurred");
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new
                    {
                        statusCode = (int)HttpStatusCode.InternalServerError,
                        errorCode = ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                        errorMessage = MessageConstants.INTERNAL_ERROR
                    };
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }
    }
} 