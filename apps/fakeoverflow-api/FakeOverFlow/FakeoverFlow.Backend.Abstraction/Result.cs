using FakeoverFlow.Backend.Abstraction.Errors;

namespace FakeoverFlow.Backend.Abstraction;

public record Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }
    
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error ?? throw new ArgumentNullException(nameof(error)));

    public static implicit operator Result(Error error) => Failure(error);
}

public record Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, null) => Value = value;
    private Result(Error error) : base(false, error) { }
    
    public new static Result<T> Failure(Error error) => new(error);

    public new static Result<T> Success(T data) => new(data);

    
    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);
}