using System.Text.Json.Serialization;

namespace Bugget.Features.Notifications.Mattermost.HttpModels;

public sealed class CreateMattermostMessageRequest
{
    /// <summary>
    /// Канал в который необходимо отправить сообщение
    /// </summary>
    public required string ChannelId { get; init; }
        
    /// <summary>
    /// Тело сообщения
    /// </summary>
    public required string Message { get; init; }
        
    /// <summary>
    /// При передачи MessageId, сообщение попадет в тред, а не в канал
    /// </summary>
    [JsonPropertyName("root_id")]
    public string? MessageId { get; init; }
}