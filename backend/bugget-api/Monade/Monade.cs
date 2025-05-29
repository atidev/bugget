using JetBrains.Annotations;

namespace Monade;

[PublicAPI]
public record struct MonadeStruct
{
    public Error? Error { get; init; }

    public bool IsSuccess => Error == null;
    public bool HasError => !IsSuccess;

    public MonadeStruct(Error error)
    {
        Error = error;
    }

    public static readonly MonadeStruct Success = new();
    public static implicit operator MonadeStruct(Error error) => new(error);
}

[PublicAPI]
public record struct MonadeStruct<TValue>
{
    public bool IsSuccess => Error == null;
    public bool HasError => !IsSuccess;
    public TValue? Value { get; init; }
    public Error? Error { get; init; }

    public MonadeStruct(TValue value)
    {
        Value = value;
    }

    public MonadeStruct(Error error)
    {
        Error = error;
    }

    public static implicit operator MonadeStruct<TValue>(TValue value) => new(value);
    public static implicit operator MonadeStruct<TValue>(Error error) => new(error);
}