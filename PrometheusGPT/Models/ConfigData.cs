namespace PrometheusGPT.Models;

using System.Text.Json.Serialization;

public class ConfigData {
    [JsonPropertyName("listenUrl")]
    public string? ListenUrl { get; set; }

    [JsonPropertyName("retryCount")]
    public int RetryCount { get; set; } = 3;

    [JsonPropertyName("retryTime")]
    public int RetryTime { get; set; } = 1500;

    [JsonPropertyName("requestTag")]
    public string RequestTag { get; set; } = "[Request]";

    [JsonPropertyName("responseTag")]
    public string ResponseTag { get; set; } = "[Response]";

    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    [JsonPropertyName("promptFile")]
    public string PromptFile { get; set; } = "prompt.txt";

    [JsonPropertyName("escape")]
    public EscapeData Escape { get; set; }


    public struct EscapeData {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("spaceChar")]
        public char? SpaceChar { get; set; }

        [JsonPropertyName("spacePrompt")]
        public string? SpacePrompt { get; set; }

        [JsonPropertyName("newlineChar")]
        public char? NewlineChar { get; set; }

        [JsonPropertyName("newlinePrompt")]
        public string? NewlinePrompt { get; set; }

        [JsonPropertyName("ignoreChar")]
        public char? IgnoreChar { get; set; }

        [JsonPropertyName("ignorePrompt")]
        public string? IgnorePrompt { get; set; }

        [JsonPropertyName("ignoreInsert")]
        public int IgnoreInsert { get; set; }
    }
}
