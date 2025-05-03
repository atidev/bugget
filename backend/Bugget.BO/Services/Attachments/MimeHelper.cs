using System.Buffers;
using HeyRed.Mime;

namespace Bugget.BO.Services.Attachments;

public static class MimeHelper
{
    private const int DetectionBufferSize = 4 * 1024;

    public static string GuessMime(Stream stream)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        long originalPosition = 0;
        if (stream.CanSeek)
            originalPosition = stream.Position;

        byte[] buffer = ArrayPool<byte>.Shared.Rent(DetectionBufferSize);
        try
        {
            int bytesRead = stream.Read(buffer, 0, DetectionBufferSize);

            if (stream.CanSeek)
                stream.Position = originalPosition;
            
            var mime = MimeGuesser.GuessMimeType(buffer[..bytesRead]);
            return string.IsNullOrWhiteSpace(mime)
                ? "application/octet-stream"
                : mime;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}