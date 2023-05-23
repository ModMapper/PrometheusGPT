namespace PrometheusAPI.Models;

using System.Text.Json.Serialization;

internal struct ConversationToken {
    [JsonPropertyName("conversationId")]
    public string? ConversationId { get; set; }

    [JsonPropertyName("conversationSignature")]
    public string? ConversationSignature { get; set; }

    [JsonPropertyName("clientId")]
    public string? ClientId { get; set; }

    [JsonPropertyName("result")]
    public ResultType Result { get; set; }
}
