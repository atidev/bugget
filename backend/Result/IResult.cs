namespace Result;

public interface IResult
{
    bool HasError { get; }
    Error? Error { get; }
}