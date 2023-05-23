namespace PrometheusAPI.Models.Message;

using PrometheusAPI.Models.Request;

using System.Text.Json.Serialization;

public class ContextMessage : IMessage {
    [JsonPropertyName("author")]
    public required string Author { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("contextType")]
    public string? ContextType { get; set; }

    [JsonPropertyName("messageType")]
    public string? MessageType { get; set; } = "Context";

    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    [JsonPropertyName("sourceUrl")]
    public string? SourceUrl { get; set; }
}
