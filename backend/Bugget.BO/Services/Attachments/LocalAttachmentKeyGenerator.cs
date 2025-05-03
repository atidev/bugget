using Bugget.BO.Interfaces;

namespace Bugget.BO.Services.Attachments
{
    /// <summary>
    /// Генерирует ключи для оригинала и превью вложения.
    /// </summary>
    public sealed class LocalAttachmentKeyGenerator : IAttachmentKeyGenerator
    {
        public string GetOriginalKey(string? organizationId, int reportId, int entityId, string extension)
        {
            // Убеждаемся, что расширение начинается с точки
            if (!extension.StartsWith('.'))
                extension = "." + extension;

            if (organizationId == null) 
            {
                // Формируем путь вида: {reportId}/{entityId}/{guid}{extension}
                return Path.Combine(reportId.ToString(), entityId.ToString(),
                                Guid.NewGuid().ToString() + extension)
                       .Replace(Path.DirectorySeparatorChar, '/');
            }
            else
            {
                // Формируем путь вида: {organizationId}/{reportId}/{entityId}/{guid}{extension}
                return Path.Combine(organizationId, reportId.ToString(), entityId.ToString(),
                                Guid.NewGuid().ToString() + extension)
                       .Replace(Path.DirectorySeparatorChar, '/');
            }
        }

        public string GetPreviewKey(string originalKey)
        {
            // Вставляем '-preview' перед расширением
            var idx = originalKey.LastIndexOf('.');
            if (idx < 0)
                return originalKey + "-preview";

            return string.Concat(originalKey.AsSpan(0, idx), "-preview", originalKey.AsSpan(idx));
        }
    }
}
