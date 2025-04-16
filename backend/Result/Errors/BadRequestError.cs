using JetBrains.Annotations;

namespace Result.Errors;

[PublicAPI]
public record BadRequestError(string Error, string Reason) : Error;