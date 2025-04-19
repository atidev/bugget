using JetBrains.Annotations;

namespace Monade.Errors;

[PublicAPI]
public record BadRequestError(string Error, string Reason) : Error;