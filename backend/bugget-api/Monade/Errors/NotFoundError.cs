using JetBrains.Annotations;

namespace Monade.Errors;

[PublicAPI]
public record NotFoundError(string Error, string Reason) : Error;