namespace PrometheusAPI.Models.Response;

using PrometheusAPI.Models.Message;

using System.Text.Json;
using System.Text.Json.Serialization;

public struct SydneyResponse {
    /*
    [JsonPropertyName("cursor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Cursor { get; set; }
    */

    [JsonPropertyName("messages")]
    public BotMessage[]? Messages { get; set; }

    /*
    [JsonPropertyName("firstNewMessageIndex")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int FirstNewMessageIndex { get; set; }

    [JsonPropertyName("defaultChatName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DefaultChatName { get; set; }

    [JsonPropertyName("conversationId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ConversationId { get; set; }

    [JsonPropertyName("requestId")]
    public Guid RequestId { get; set; }

    [JsonPropertyName("conversationExpiryTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset conversationExpiryTime { get; set; }

    [JsonPropertyName("telemetry")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Telemetry { get; set; }
    */

    [JsonPropertyName("throttling")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Throttling? Throttling { get; set; }

    [JsonPropertyName("result")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResultType? Result { get; set; }
}
