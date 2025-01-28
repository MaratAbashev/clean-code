namespace MarkdownWebApi.Application.Assistants;

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public int StatusCode { get; }
    
    private protected Result()
    {
        IsSuccess = true;
        Error = string.Empty;
        StatusCode = 0;
    }
    
    private protected Result(string error, int statusCode)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Error message cannot be empty.", nameof(error));

        IsSuccess = false;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result Ok()
    {
        return new Result();
    }
    
    public static Result Fail(string error, int statusCode)
    {
        return new Result(error, statusCode);
    }

    public static Result FromException(Exception exception, int statusCode)
    {
        return Fail(exception.Message, statusCode);
    }
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base()
    {
        Value = value;
    }

    private Result(string error, int statusCode) : base(error, statusCode)
    {
    }
    
    public static Result<T> Ok(T value)
    {
        return new Result<T>(value);
    }
    
    public new static Result<T> Fail(string error, int statusCode)
    {
        return new Result<T>(error, statusCode);
    }
    
    public new static Result<T> FromExceptionFromException(Exception exception, int statusCode)
    {
        return Fail(exception.Message, statusCode);
    }
}