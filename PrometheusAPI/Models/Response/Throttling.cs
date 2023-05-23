namespace PrometheusAPI.Models.Response;

using System.Text.Json.Serialization;

public struct Throttling {
    [JsonPropertyName("maxNumUserMessagesInConversation")]
    public int MaxMessages { get; set; }

    [JsonPropertyName("numUserMessagesInConversation")]
    public int UserMessages { get; set; }
}
