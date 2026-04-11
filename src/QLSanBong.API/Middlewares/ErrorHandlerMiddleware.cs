using QLSanBong.Common.Exceptions;
using QLSanBong.Common.Wrappers;
using System.Net;
using System.Text.Json;

namespace QLSanBong.API.Middlewares;

public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception error)
        {
            await HandleExceptionAsync(context, error);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception error)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var statusCode = HttpStatusCode.InternalServerError;
        var message = error.Message;
        var action = "SystemError";
        List<string>? errors = null;

        switch (error)
        {
            case ValidationException e:
                statusCode = HttpStatusCode.BadRequest;
                message = "Dữ liệu không hợp lệ";
                action = "ValidationError";
                errors = e.Errors;
                break;

            case ApiException:
                statusCode = HttpStatusCode.BadRequest;
                action = "LogicError";
                break;

            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                action = "NotFound";
                break;

            default:
                logger.LogError(error, error.Message);
                message = "Đã có lỗi hệ thống xảy ra. Vui lòng thử lại sau.";
                break;
        }

        response.StatusCode = (int)statusCode;

        // Sử dụng cấu trúc ApiResponse mới
        var responseModel = errors != null
            ? ApiResponse<string>.ValidationResponse(errors, action)
            : ApiResponse<string>.FailureResponse(message, action);

        var result = JsonSerializer.Serialize(responseModel, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(result);
    }
}