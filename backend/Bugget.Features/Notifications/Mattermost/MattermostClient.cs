using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bugget.Features.Notifications.Mattermost.HttpModels;

namespace Bugget.Features.Notifications.Mattermost
{
    public class MattermostClient(IHttpClientFactory httpClientFactory)
    {
        private const string PostMessageUrl = "/api/v4/posts";
        private const string DirectChannelUrl = "/api/v4/channels/direct";

        private readonly HttpClient _httpClient = httpClientFactory.CreateClient(MattermostConstants.MattermostHttpClientKey);

        private readonly string? _botAccessToken =
            Environment.GetEnvironmentVariable(MattermostConstants.MattermostBotAccessTokenName)
            ?? throw new ApplicationException($"Не задан токен mattermost, env=[{MattermostConstants.MattermostBotAccessTokenName}]");

        private JsonSerializerOptions _jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        public async Task<MattermostMessageResponse?> SendMessageAsync(string recipientId, string text, bool isDirectMessage = true,
            string? threadId = null)
        {
            string channelId = recipientId;

            if (isDirectMessage)
            {
                var channelResponse = await CreateDirectChannelAsync(recipientId);
                channelId = channelResponse?.Id ?? throw new Exception("Не удалось создать или получить личный канал.");
            }

            var messageRequest = new CreateMattermostMessageRequest
            {
                ChannelId = channelId,
                Message = text
            };

            var request = new HttpRequestMessage(HttpMethod.Post, PostMessageUrl)
            {
                Content = JsonContent.Create(messageRequest, options: _jsonSerializerOptions)
            };

            request.Headers.Add("Authorization", $"Bearer {_botAccessToken}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<MattermostMessageResponse>(_jsonSerializerOptions);
        }

        private async Task<ChannelResponse?> CreateDirectChannelAsync(string userId)
        {
            var botUserId = await GetBotUserIdAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, DirectChannelUrl)
            {
                Content = JsonContent.Create(new[] { botUserId, userId }, options: _jsonSerializerOptions)
            };

            request.Headers.Add("Authorization", $"Bearer {_botAccessToken}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ChannelResponse>(_jsonSerializerOptions);
        }

        private async Task<string> GetBotUserIdAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v4/users/me");
            request.Headers.Add("Authorization", $"Bearer {_botAccessToken}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<UserResponse>(_jsonSerializerOptions);
            return user?.Id ?? throw new Exception("Не удалось получить идентификатор пользователя бота.");
        }
    }
}