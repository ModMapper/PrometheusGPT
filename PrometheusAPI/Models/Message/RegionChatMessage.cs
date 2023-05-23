namespace PrometheusAPI.Models.Message;

using System.Text.Json;
using System.Text.Json.Serialization;

public class RegionChatMessage : ChatMessage {
    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("market")]
    public string? Market { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("locationHints")]
    public IEnumerable<JsonElement>? LocationHints { get; set; }
}
