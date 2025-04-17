using JetBrains.Annotations;

namespace Monade.Errors;

[PublicAPI]
public record InternalServerError(string Error, string Reason) : Error;