namespace Bugget.DA.Interfaces
{
    public interface IFileStorageClient
    {
        Task WriteAsync(string storageKey, Stream content, CancellationToken ct = default);
        Task DeleteAsync(string storageKey, CancellationToken ct = default);
        Task<Stream> ReadAsync(string storageKey, CancellationToken ct = default);
    }
}