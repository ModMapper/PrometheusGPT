namespace PrometheusAPI.Sockets;

using System.Net.WebSockets;
using System.Threading.Tasks;

public class SydneySocket : SignalRSocket {
    private const string USERAGENT = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.64";
    private const string URL = @"wss://sydney.bing.com/sydney/ChatHub";

    private static readonly byte[] Packet_Handshake = "{\"protocol\":\"json\",\"version\":1}"u8.ToArray();

    public SydneySocket() : base(new ClientWebSocket()) {
        Socket.Options.SetRequestHeader("User-Agent", USERAGENT);
    }

    protected new ClientWebSocket Socket => (ClientWebSocket)base.Socket;

    public async Task ConnectAsync(CancellationToken token = default) {
        await Socket.ConnectAsync(new(URL), token);
        await Socket.SendAsync(Packet_Handshake, WebSocketMessageType.Binary, true, token);
        await ReceiveAsync(token);
        await PingAsync(token);
    }
}
