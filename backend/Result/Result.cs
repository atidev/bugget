using JetBrains.Annotations;

namespace Result;

[PublicAPI]
public record struct Result : IResult
{
    public Error? Error { get; init; }

    public bool IsSuccess => Error == null;
    public bool HasError => !IsSuccess;

    public Result(Error error)
    {
        Error = error;
    }

    public static readonly Result Success = new();
    public static implicit operator Result(Error error) => new(error);
}

[PublicAPI]
public record struct Result<TValue> : IResult
{
    public bool IsSuccess => Error == null;
    public bool HasError => !IsSuccess;
    public TValue? Value { get; init; }
    public Error? Error { get; init; }

    public Result(TValue value)
    {
        Value = value;
    }

    public Result(Error error)
    {
        Error = error;
    }

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(Error error) => new(error);
}