namespace PrometheusAPI;

using PrometheusAPI.Models;
using PrometheusAPI.Models.Request;
using PrometheusAPI.Sockets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class Conversation {
    const string USERAGENT = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Mobile Safari/537.36 Edg/113.0.1774.35";
    const string ACCEPT = "application/json";
    const string ACCEPTLANG = "ko,en;q=0.9,en-US;q=0.1";
    const string REFERER = @"https://www.bing.com/search?q=";

    const string URL = @"https://www.bing.com/turing/conversation/create";
    //const string URL = @"https://edgeservices.bing.com/edgesvc/turing/conversation/create";

    private static readonly Lazy<HttpClient> client = new(() => {
        return new(new SocketsHttpHandler() { UseCookies = false, });
    });

    public static async Task<Conversation> CreateAsync(string? user = null) {
        using var req = new HttpRequestMessage(HttpMethod.Get, URL);

        var header = req.Headers;
        header.Referrer = new(REFERER);
        header.UserAgent.ParseAdd(USERAGENT);
        header.Accept.ParseAdd(ACCEPT);
        header.AcceptLanguage.ParseAdd(ACCEPTLANG);
        if(user != null) header.Add("Cookie", "_U=" + user);

        using var res = await client.Value.SendAsync(req);
        var token = await res.Content.ReadFromJsonAsync<ConversationToken>();

        if (!token.Result.IsSuccess) {
            throw new InvalidDataException(token.Result.Message);
        }
        if (token.ConversationId == null || token.ConversationSignature == null || token.ClientId == null) {
            throw new InvalidDataException();
        }
        return new(token.ConversationId, token.ConversationSignature, token.ClientId);
    }

    private readonly SemaphoreSlim threadlock = new(1, 1);
    
    private Conversation(string conversationId, string conversationSignature, string clientId)
        => (ConversationId, ConversationSignature, ClientId) = (conversationId, conversationSignature, clientId);

    public bool IsStartOfSession { get; private set; } = true;

    public string ConversationId { get; }

    public string ConversationSignature { get; }

    public string ClientId { get; }

    public async Task<ChatResult> ChatAsync(Chat chat, CancellationToken token = default) {
        SydneySocket socket = new();
        await socket.ConnectAsync(token);
        await SendAsync(socket, chat.GetArgument(this), token);
        return new(socket);
    }

    public async Task<ChatResult> ChatAsync(string chat, CancellationToken token = default) {
        SydneySocket socket = new();
        await socket.ConnectAsync(token);
        await SendAsync(socket, new Chat(chat).GetArgument(this), token);
        return new(socket);
    }

    private async Task SendAsync(SydneySocket socket, SydneyRequest argument, CancellationToken token = default) {
        var data = JsonSerializer.SerializeToElement(argument);
        await threadlock.WaitAsync(token);
        try {
            await socket.SendAsync(new() {
                Type = 4,
                Target = "chat",
                InvocationId = "0",
                Arguments = new[] { data },
            }, token);
            IsStartOfSession = false;
        } finally {
            threadlock.Release();
        }
    }
}
