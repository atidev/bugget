using Monade;
using Bugget.BO.Errors;
using Bugget.Entities.BO.AttachmentBo;
using Bugget.Entities.Constants;

namespace Bugget.BO.Services.Attachments
{
    public static class AttachmentValidator
    {
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

        private static readonly char[] InvalidFileNameChars = ['<', '>', '"'];

        public static Error? Validate(in FileMeta meta)
        {
            if (meta.LengthBytes == 0)
                return BoErrors.AttachmentFileNotSelectedOrEmpty;

            if (meta.LengthBytes > MaxFileSize)
                return BoErrors.AttachmentFileTooLarge;

            if (!AttachmentConstants.AllowedMimes.Value.Contains(meta.TrustedMimeType, StringComparer.OrdinalIgnoreCase))
                return BoErrors.AttachmentTypeNotAllowed;

            if (meta.FileName.Any(c => InvalidFileNameChars.Contains(c)))
                return BoErrors.AttachmentFileNameInvalidChars;

            var ext = Path.GetExtension(meta.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext))
                return BoErrors.AttachmentFileExtensionNotFound;

            return null;
        }
    }
}