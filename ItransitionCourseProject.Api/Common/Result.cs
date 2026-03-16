namespace ItransitionCourseProject.Api.Common;

public class Result<T>
{
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public Error Error { get; set; }
    
    private Result(int statusCode, T data, Error error)
    {
        StatusCode = statusCode;
        Data = data;
        Error = error;
    }
    
    public static Result<T> Success(int statusCode, T data) => new(statusCode, data, Error.None);
    
    public static Result<T> Failure(int statusCode, Error error) => new(statusCode, default!, error);
}