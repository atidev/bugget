using Monade;
using Bugget.BO.Errors;
using Bugget.Entities.BO.AttachmentBo;

namespace Bugget.BO.Validators;

public static class AttachmentValidator
{
    private static readonly string[] AllowedExts = [".png", ".jpg", ".jpeg", ".webp", ".txt", ".log", ".json", ".csv", ".md"];
    private static readonly string[] BlockedExts = [".exe", ".js", ".scr", ".bat", ".cmd", ".zip"];
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB
    private static readonly char[] InvalidFileNameChars = ['<', '>', '"'];

    public static Error? Validate(FileMeta fileMeta)
    {
        if (fileMeta.LengthBytes == 0)
            return BoErrors.AttachmentFileNotSelectedOrEmpty;

        if (fileMeta.LengthBytes > MaxFileSize)
            return BoErrors.AttachmentFileTooLarge;

        var ext = Path.GetExtension(fileMeta.FileName).ToLowerInvariant();

        if (!AllowedExts.Any(a => string.Equals(a, ext, StringComparison.OrdinalIgnoreCase)))
            return BoErrors.AttachmentFileExtensionNotAllowed(ext);

        if (BlockedExts.Any(b => string.Equals(b, ext, StringComparison.OrdinalIgnoreCase)))
            return BoErrors.AttachmentFileExtensionBlocked(ext);

        if (fileMeta.FileName.Any(c => InvalidFileNameChars.Contains((char)c)))
            return BoErrors.AttachmentFileNameInvalidChars;

        return null;
    }
}
