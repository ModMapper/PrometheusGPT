namespace PrometheusAPI;

using PrometheusAPI.Models.Message;
using PrometheusAPI.Models.Request;

using System;
using System.Collections.Generic;

public class Chat {
    public Chat() {
        Message = new ChatMessage() { Author = "user" };
    }

    public Chat(IMessage message) {
        Message = message;
    }

    public Chat(string chat) {
        Message = new ChatMessage() { Author = "user", Text = chat };
    }

    public IEnumerable<string>? AllowedTypes { get; set; }

    public IEnumerable<string>? Options { get; set; }

    public IEnumerable<string>? SliceId { get; set; }

    public IMessage Message { get; set; }

    public List<IMessage> PrevMessages { get; } = new();

    public string? TraceId { get; set; }

    public SydneyRequest GetArgument(Conversation conversation) {
        return new() {
            Source                  = "cib",
            AllowedMessageTypes     = AllowedTypes,
            OptionsSets             = Options,
            SliceIds                = SliceId,
            Verbosity               = "verbose",
            TraceId                 = TraceId ?? CreateTraceId(),
            IsStartOfSession        = conversation.IsStartOfSession,
            Message                 = Message,
            ConversationId          = conversation.ConversationId,
            Participant             = new() { Id = conversation.ClientId },
            ConversationSignature   = conversation.ConversationSignature,
            previousMessages        = 0 < PrevMessages.Count ? PrevMessages : null,
        };
    }

    public static string CreateTraceId() {
        Span<byte> buffer = stackalloc byte[16];
        Random.Shared.NextBytes(buffer);
        return Convert.ToHexString(buffer).ToLower();
    }
}
