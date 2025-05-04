using System.Buffers;
using HeyRed.Mime;

public static class MimeHelper
{
    private const int DetectionBufferSize = 4 * 1024;

    public static async Task<string> GuessMimeAsync(Stream stream, CancellationToken ct = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        // Сохраняем позицию, если можем
        long origPos = stream.CanSeek ? stream.Position : 0;

        // Читаем в арендованный буфер
        var buffer = ArrayPool<byte>.Shared.Rent(DetectionBufferSize);
        try
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, DetectionBufferSize, ct);

            // Сбрасываем позицию, если это возможно
            if (stream.CanSeek)
                stream.Position = origPos;

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