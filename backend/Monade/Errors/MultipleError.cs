using System.Collections.Generic;

namespace Monade.Errors;

public record MultipleError(string Error, string Reason, IEnumerable<MultipleErrorElement> ErrorList);
public record MultipleErrorElement(string Error, string Reason);