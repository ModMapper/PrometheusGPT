namespace PrometheusAPI.Sockets;

using PrometheusAPI.Models;

using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;

public class SignalRSocket : IDisposable {
    private static readonly byte[] Packet_Ping = "{\"type\":6}"u8.ToArray();
    private const byte DELIMITER = 0x1E;

    private readonly CancellationTokenSource tokenSource;
    private readonly SemaphoreSlim readlock, writelock;
    private readonly Pipe readpipe, writepipe;
    private Task? fetchtask;

    public SignalRSocket(WebSocket socket) {
        Socket = socket;
        tokenSource = new();
        (readpipe, readlock) = (new(), new(1, 1));
        (writepipe, writelock) = (new(), new(1, 1));
    }

    protected WebSocket Socket { get; }

    public async Task SendAsync(SignalRData data, CancellationToken token = default) {
        await writelock.WaitAsync(token);
        try {
            WriteData(writepipe.Writer, data);
            ReadResult read = await writepipe.Reader.ReadAsync(token);
            foreach (var buf in read.Buffer) {
                await Socket.SendAsync(buf, WebSocketMessageType.Binary, false, token);
            }
            await Socket.SendAsync(Memory<byte>.Empty, WebSocketMessageType.Binary, true, token);
        } finally {
            writepipe.Reader.Complete();
            writepipe.Reset();
            writelock.Release();
        }

        static void WriteData(PipeWriter writer, SignalRData data) {
            try {
                JsonSerializer.Serialize(new Utf8JsonWriter(writer), data);
                writer.Write(stackalloc byte[] { DELIMITER });
                writer.Complete();
            } catch (Exception e) {
                writer.Complete(e);
            }
        }
    }

    public async Task PingAsync(CancellationToken token = default) {
        await Socket.SendAsync(Packet_Ping, WebSocketMessageType.Binary, true, token);
    }

    public async Task<SignalRData> ReceiveAsync(CancellationToken token) {
        await readlock.WaitAsync();
        try {
            fetchtask ??= Task.Factory.StartNew(FetchAsync, TaskCreationOptions.LongRunning);

            var reader = readpipe.Reader;
            SequencePosition? position;
            ReadResult read;
            while (true) {
                read = await reader.ReadAsync(token);
                position = read.Buffer.PositionOf(DELIMITER);
                if (position.HasValue) break;
                if (read.IsCompleted) throw new EndOfStreamException();
                reader.AdvanceTo(read.Buffer.Start);
                await Task.Yield();
            }

            try {
                var buffer = read.Buffer.Slice(0, position.Value);
                return Deserialize(buffer);
            } finally {
                reader.AdvanceTo(read.Buffer.GetPosition(1, position.Value));
            }
        } finally {
            readlock.Release();
        }

        static SignalRData Deserialize(ReadOnlySequence<byte> buffer) {
            var jsonwriter = new Utf8JsonReader(buffer);
            return JsonSerializer.Deserialize<SignalRData>(ref jsonwriter);
        }
    }

    private async Task FetchAsync() {
        ValueWebSocketReceiveResult read;
        PipeWriter writer = readpipe.Writer;
        try {
            do {
                do {
                    var buffer = writer.GetMemory();
                    read = await Socket.ReceiveAsync(buffer, tokenSource.Token);
                    writer.Advance(read.Count);
                } while (!read.EndOfMessage);
                await writer.FlushAsync(tokenSource.Token);
                await Task.Yield();
            } while (IsConnected());
            await writer.CompleteAsync();
        } catch(Exception e) {
            await writer.CompleteAsync(e);
        }
    }

    public async Task CloseAsync() {
        await readlock.WaitAsync();
        try {
            fetchtask ??= Task.CompletedTask;
        } finally {
            readlock.Release();
        }
        tokenSource.Cancel();
        await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, default);
        await fetchtask;
    }

    public bool IsConnected() {
        return Socket.State is WebSocketState.Open or WebSocketState.CloseSent or WebSocketState.CloseReceived;
    }

    public void Dispose() {
        tokenSource.Dispose();
        writelock.Dispose();
        readlock.Dispose();
    }
}
