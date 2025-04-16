using JetBrains.Annotations;

namespace Result.Errors;

[PublicAPI]
public record InternalServerError(string Error, string Reason) : Error;