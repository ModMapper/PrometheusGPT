namespace PrometheusAPI.Models.Message;

using System.Text.Json.Serialization;

public class ChatMessage : IMessage {

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("author")]
    public required string Author { get; set; }

    [JsonPropertyName("inputMethod")]
    public string? inputMethod { get; set; } = "Keyboard";

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("messageType")]
    public string? MessageType { get; set; } = "Chat";
}
