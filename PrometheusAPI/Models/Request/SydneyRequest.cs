namespace PrometheusAPI.Models.Request;

using PrometheusAPI.Models.Message;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public struct SydneyRequest {
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("optionsSets")]
    public IEnumerable<string>? OptionsSets { get; set; }

    [JsonPropertyName("allowedMessageTypes")]
    public IEnumerable<string>? AllowedMessageTypes { get; set; }

    [JsonPropertyName("sliceIds")]
    public IEnumerable<string>? SliceIds { get; set; }

    [JsonPropertyName("verbosity")]
    public string? Verbosity { get; set; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("isStartOfSession")]
    public required bool IsStartOfSession { get; set; }

    [JsonPropertyName("message")]
    public object Message { get; set; }

    [JsonPropertyName("conversationSignature")]
    public required string ConversationSignature { get; set; }

    [JsonPropertyName("participant")]
    public required Participant Participant { get; set; }

    [JsonPropertyName("conversationId")]
    public required string ConversationId { get; set; }

    [JsonPropertyName("previousMessages")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<object>? previousMessages { get; set; }
}
