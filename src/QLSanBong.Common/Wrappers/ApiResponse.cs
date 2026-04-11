namespace QLSanBong.Common.Wrappers;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Errors { get; set; }
    public T? Data { get; set; }

    public ApiResponse() { }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Thành công", string action = "Operation")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Description = action,
            Data = data
        };
    }

    public static ApiResponse<T> FailureResponse(string message, string action = "Error")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Description = action,
            Errors = new List<string> { message }
        };
    }

    public static ApiResponse<T> ValidationResponse(List<string> errors, string action = "Validation")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "Dữ liệu không hợp lệ",
            Description = action,
            Errors = errors
        };
    }
}