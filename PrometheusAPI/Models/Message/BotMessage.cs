namespace PrometheusAPI.Models.Message;

using System.Text.Json.Serialization;

public class BotMessage : IMessage {
    [JsonPropertyName("hiddenText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? HiddenText { get; set; }

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; set; }

    [JsonPropertyName("spokenText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SpokenText { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; } = "bot";

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    /*
    [JsonPropertyName("messageId")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("offense")]
    public string? Offense { get; set; }

    [JsonPropertyName("adaptiveCards")]
    public IMessage[]? AdaptiveCards { get; set; }

    [JsonPropertyName("sourceAttributions")]
    public JsonElement[]? SourceAttributions { get; set; }

    [JsonPropertyName("feedback")]
    public JsonElement? Feedback { get; set; }
    */
    
    [JsonPropertyName("contentOrigin")]
    public string? ContentOrigin { get; set; }
    
    /*
    [JsonPropertyName("privacy")]
    public JsonElement? Privacy { get; set; }

    [JsonPropertyName("suggestedResponses")]
    public IMessage[]? suggestedResponses { get; set; }
    */
}