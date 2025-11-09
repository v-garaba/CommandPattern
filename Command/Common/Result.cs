namespace Command.Common;

/// <summary>
/// Represents the result of an operation (Microsoft's recommended Result pattern).
/// </summary>
/// <typeparam name="T">The type of the success value</typeparam>
public readonly struct Result<T>
{
    private readonly T? _value;
    private readonly string? _error;

    public bool IsSuccess => _error is null;

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access value of failed result");
    public string Error => !IsSuccess ? _error! : throw new InvalidOperationException("Cannot access error of successful result");

    private Result(T value)
    {
        _value = value;
        _error = null;
    }

    private Result(string error)
    {
        _value = default;
        _error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(string error) => Failure(error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
        => IsSuccess ? onSuccess(Value) : onFailure(Error);

    public override string ToString()
    {
        return IsSuccess ? Value?.ToString() ?? string.Empty : Error;
    }
}

/// <summary>
/// Non-generic Result for operations that don't return a value.
/// </summary>
public readonly struct Result
{
    private readonly string? _error;

    public bool IsSuccess => _error is null;
    public string ErrorMessage => !IsSuccess ? _error! : throw new InvalidOperationException("Cannot access error of successful result");

    private Result(string? error)
    {
        _error = error;
    }

    public static Result Success() => new(null);
    public static Result Failure(string error) => new(error);

    public static implicit operator Result(string error) => Failure(error);

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<string, TResult> onFailure)
        => IsSuccess ? onSuccess() : onFailure(ErrorMessage);

    public override string ToString()
    {
        return IsSuccess ? "Success" : ErrorMessage;
    }
}