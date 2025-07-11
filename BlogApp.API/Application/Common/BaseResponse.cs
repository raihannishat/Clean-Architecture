namespace BlogApp.API.Application.Common;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public int StatusCode { get; set; } = 200;

    public static BaseResponse<T> Success(T data, string message = "Operation completed successfully")
    {
        return new BaseResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            StatusCode = 200
        };
    }

    public static BaseResponse<T> Failure(string message, int statusCode = 400)
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static BaseResponse<T> Failure(List<string> errors, string message = "Operation failed", int statusCode = 400)
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }

    public static BaseResponse<T> NotFound(string message = "Resource not found")
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = 404
        };
    }

    public static BaseResponse<T> Unauthorized(string message = "Unauthorized access")
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = 401
        };
    }

    public static BaseResponse<T> Forbidden(string message = "Access forbidden")
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = 403
        };
    }

    public static BaseResponse<T> ValidationError(List<string> errors, string message = "Validation failed")
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors,
            StatusCode = 400
        };
    }
}

public class BaseResponse : BaseResponse<object>
{
    public static BaseResponse Success(string message = "Operation completed successfully")
    {
        return new BaseResponse
        {
            IsSuccess = true,
            Message = message,
            StatusCode = 200
        };
    }

    public static BaseResponse Failure(string message, int statusCode = 400)
    {
        return new BaseResponse
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static BaseResponse Failure(List<string> errors, string message = "Operation failed", int statusCode = 400)
    {
        return new BaseResponse
        {
            IsSuccess = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }
} 