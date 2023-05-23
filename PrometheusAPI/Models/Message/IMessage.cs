namespace PrometheusAPI.Models.Message;

using System.Text.Json.Serialization;

public interface IMessage {

    [JsonPropertyName("author")]
    public string Author { get; set; }

    public const string AUTHOR_USER = "user";
    public const string AUTHOR_BOT = "bot";
}