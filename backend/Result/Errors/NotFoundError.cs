using JetBrains.Annotations;

namespace Result.Errors;

[PublicAPI]
public record NotFoundError(string Error, string Reason) : Error;