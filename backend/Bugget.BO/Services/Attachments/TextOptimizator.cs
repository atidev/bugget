using System.IO.Compression;

namespace Bugget.BO.Services.Attachments
{    public sealed class TextOptimizator
    {
        public async Task<Stream> CompressAsync(
            Stream originalStream,
            CancellationToken ct = default)
        {
            if (originalStream.CanSeek)
                originalStream.Position = 0;

            var compressed = new MemoryStream();
            await using (var gzip = new GZipStream(compressed, CompressionLevel.Optimal, leaveOpen: true))
            {
                await originalStream.CopyToAsync(gzip, ct);
            }
            compressed.Position = 0;
            return compressed;
        }
    }
}