namespace PrometheusAPI.Models;

using System.Text.Json.Serialization;

public struct ResultType {
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("serviceVersion")]
    public string ServiceVersion { get; set; }

    public const string VALUE_SUCCESS = "Success";

    public bool IsSuccess
        => Value.Equals(VALUE_SUCCESS, StringComparison.OrdinalIgnoreCase);
}
