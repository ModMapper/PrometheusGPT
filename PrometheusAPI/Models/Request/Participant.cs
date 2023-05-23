namespace PrometheusAPI.Models.Request;

using System.Text.Json.Serialization;

public struct Participant {
    [JsonPropertyName("id")]
    public string Id { get; set; }
}