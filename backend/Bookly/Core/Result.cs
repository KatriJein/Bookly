namespace Core;

public readonly struct Result : IResult
{
    public bool IsSuccess { get; init; }
    public bool IsFailure { get; init; }

    private readonly string _error;
    
    public string Error
    {
        get => IsFailure ? _error : throw new InvalidOperationException("Невозможно обратиться к свойству Error успешного результата");
        internal init => _error = value;
    }

    public static Result Success()
    {
        return new Result() {  IsSuccess = true };
    }

    public static Result Failure(string error)
    {
        return new Result() { IsFailure = true, Error = error };
    }
}

public readonly struct Result<T> : IValueResult<T>
{
    public bool IsSuccess { get; init; }
    public bool IsFailure { get; init; }

    private readonly string _error;
    private readonly T _value;

    public static Result<T> Success(T value)
    {
        return new Result<T>() { IsSuccess = true, Value = value };
    }
    
    public static Result<T> Failure(string error)
    {
        return new Result<T>() { IsFailure = true, Error = error };
    }
    
    public string Error
    {
        get => IsFailure ? _error : throw new InvalidOperationException("Невозможно обратиться к свойству Error успешного результата");
        private init => _error = value;
    }

    public T Value
    {
        get => IsSuccess ? _value : throw new InvalidOperationException("Невозможно обратиться к свойству Value провального результата");
        private init => _value = value;
    }
}

public interface IResult
{
    bool IsSuccess { get; init; }
    bool IsFailure { get; init; }
    string Error { get; }
}

public interface IValueResult<out T> : IResult
{
    T Value { get; }
}