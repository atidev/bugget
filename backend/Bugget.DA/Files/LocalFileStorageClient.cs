using Bugget.DA.Interfaces;
using Bugget.Entities.Options;
using Microsoft.Extensions.Options;

namespace Bugget.DA.Files
{
    /// <summary>
    /// Локальное файловое хранилище: запись, чтение, удаление.
    /// </summary>
    public sealed class LocalFileStorageClient(IOptions<FileStorageOptions> fileStorageOptions) : IFileStorageClient
    {
        private readonly string _baseDirectory = fileStorageOptions.Value.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, '/');

        public async Task WriteAsync(string storageKey, Stream content, CancellationToken ct = default)
        {
            var path = Path.Combine(_baseDirectory, storageKey.Replace('/', Path.DirectorySeparatorChar));
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            // Сбрасываем позицию, если можно
            if (content.CanSeek)
                content.Position = 0;

            await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await content.CopyToAsync(fs, ct);
        }

        public Task<Stream> ReadAsync(string storageKey, CancellationToken ct = default)
        {
            var path = Path.Combine(_baseDirectory, storageKey.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(path))
                throw new FileNotFoundException("File not found", storageKey);

            Stream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult(fs);
        }

        public Task DeleteAsync(string storageKey, CancellationToken ct = default)
        {
            var path = Path.Combine(_baseDirectory, storageKey.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(path))
                File.Delete(path);
            return Task.CompletedTask;
        }
    }
}