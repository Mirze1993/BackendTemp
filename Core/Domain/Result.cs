namespace Domain;

public class Result
{
    public bool Success { get; set; } = true;
    public string ErrorMessage { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public static Result SuccessResult()
    {
        return new Result();
    }

    public static Result ErrorResult(string message)
    {
        return new Result() { ErrorMessage = message, Success = false };
    }

    public static Result ErrorResult(string message, string description)
    {
        return new Result() { ErrorMessage = message, Description = description, Success = false };
    }
}

public class Result<T>
{
    public bool Success { get; set; } = true;
    public string ErrorMessage { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public T? Value { get; set; }

    public static Result<T> SuccessResult(T t)
    {
        return new Result<T> { Value = t };
    }

    public static Result<T> ErrorResult(string message)
    {
        return new Result<T> { ErrorMessage = message, Success = false };
    }

    public static Result<T> ErrorResult(string message, string description)
    {
        return new Result<T> { ErrorMessage = message, Description = description, Success = false };
    }
}

public static class ExtensionResult
{
    public static Result<T> SuccessResult<T> (this object o)
    {
        return new Result<T> { Success = true, Value =(T) o };
    }
}

public class SocketResult
{
    public SocketResult(string dataType)
    {
        DataType = dataType;
    }

    public string DataType { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
    public string ErrorMessage { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public static SocketResult SuccessResult(string dataType = "generic")
    {
        return new SocketResult(dataType);
    }

    public static SocketResult ErrorResult(string message, string dataType = "generic")
    {
        return new SocketResult(dataType) { ErrorMessage = message, Success = false };
    }

    public static SocketResult ErrorResult(string message, string description, string dataType = "generic")
    {
        return new SocketResult(dataType) { ErrorMessage = message, Description = description, Success = false };
    }
}

public class SocketResult<T>
{
    public SocketResult(string dataType)
    {
        DataType = dataType;
    }

    public string DataType { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
    public string ErrorMessage { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public T? Value { get; set; }

    public static SocketResult<T> SuccessResult(T t, string dataType = "generic")
    {
        return new SocketResult<T>(dataType) { Value = t };
    }

    public static SocketResult<T> ErrorResult(string message, string dataType = "generic")
    {
        return new SocketResult<T>(dataType) { ErrorMessage = message, Success = false };
    }

    public static SocketResult<T> ErrorResult(string message, string description, string dataType = "generic")
    {
        return new SocketResult<T>(dataType) { ErrorMessage = message, Description = description, Success = false };
    }
}