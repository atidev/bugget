using Bugget.BO.Interfaces;

namespace Bugget.BO.Services.Attachments
{
    /// <summary>
    /// Генерирует пути для локального хранения вложений.
    /// </summary>
    public sealed class LocalAttachmentKeyGenerator : IAttachmentKeyGenerator
    {
        public string GetTempKey(string? organizationId, int reportId, int entityId, string extension)
        {
            if (!extension.StartsWith('.'))
                extension = "." + extension;

            if (organizationId == null)
            {
                // Формируем путь вида: temp/{reportId}/{entityId}/{guid}{extension}
                return Path.Combine("temp", reportId.ToString(), entityId.ToString(),
                                Guid.NewGuid().ToString() + extension)
                       .Replace(Path.DirectorySeparatorChar, '/');
            }
            else
            {
                // Формируем путь вида: temp/{organizationId}/{reportId}/{entityId}/{guid}{extension}
                return Path.Combine("temp", organizationId, reportId.ToString(), entityId.ToString(),
                                Guid.NewGuid().ToString() + extension)
                       .Replace(Path.DirectorySeparatorChar, '/');
            }
        }

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
