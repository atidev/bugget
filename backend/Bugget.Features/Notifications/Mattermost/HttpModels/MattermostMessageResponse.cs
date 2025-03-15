using System.Text.Json.Serialization;

namespace Bugget.Features.Notifications.Mattermost.HttpModels;

public sealed class MattermostMessageResponse
{
    /// <summary>
    /// Ошибка
    /// </summary>
    [JsonPropertyName("message")]
    public string Error { get; set; }
    
    /// <summary>
    /// Id текущего сообщения (необходимо для редактирования, отправки в тред)
    /// </summary>
    [JsonPropertyName("id")]
    public string MessageId { get; set; }

    /// <summary>
    /// Статус код приходит при возникновении ошибки
    /// </summary>
    public int? StatusCode { get; set; }
}