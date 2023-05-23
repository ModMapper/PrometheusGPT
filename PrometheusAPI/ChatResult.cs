namespace PrometheusAPI;

using PrometheusAPI.Models.Message;
using PrometheusAPI.Models.Response;
using PrometheusAPI.Sockets;

using System;
using System.Text.Json;
using System.Threading.Tasks;

using static System.Runtime.InteropServices.JavaScript.JSType;

public class ChatResult {
    private readonly TaskCompletionSource<Throttling> throttlingSource;
    private readonly CancellationTokenSource tokenSource;
    private readonly SydneySocket socket;
    private readonly Task task;
    private bool completed;

    private TaskCompletionSource<BotMessage[]> messageSource;
    private BotMessage[] messages = Array.Empty<BotMessage>();
    private BotMessage[] lastsucessful = Array.Empty<BotMessage>();

    public ChatResult(SydneySocket socket) {
        throttlingSource = new();
        messageSource = new();
        tokenSource = new();
        this.socket = socket;
        task = Task.Run(ReceiveTask);
    }

    public string? Text {
        get {
            if (messages.Length == 0) return null;
            var msg = messages[^1];
            return msg.Text ?? msg.HiddenText ?? msg.SpokenText;
        }
    }

    public string? SucessfulText {
        get {
            if (lastsucessful.Length == 0) return null;
            var msg = lastsucessful[^1];
            return msg.Text ?? msg.HiddenText ?? msg.SpokenText;
        }
    }

    public bool IsCompleted => completed;

    public BotMessage[] Messages => messages;

    public BotMessage[] LastSucessful => lastsucessful;

    public Task WaitAsync(CancellationToken token = default)
        => task.WaitAsync(token);

    public async Task<string?> GetTextAsync(CancellationToken token = default) {
        var messages = await messageSource.Task;
        if (messages.Length == 0) return null;
        var msg = messages[^1];
        return msg.Text ?? msg.HiddenText ?? msg.SpokenText;
    }

    public Task<BotMessage[]> GetMessagesAsync(CancellationToken token = default)
        => messageSource.Task.WaitAsync(token);

    public Task<Throttling> GetThrottlingAsync(CancellationToken token = default)
        => throttlingSource.Task.WaitAsync(token);

    private async Task ReceiveTask() {
        try {
            while (true) {
                var data = await socket.ReceiveAsync(tokenSource.Token);
                switch (data.Type) {
                    case 1: // 데이터
                        if (data.Arguments == null || data.Arguments.Length == 0) break;
                        foreach (var args in data.Arguments) {
                            SetData(args.Deserialize<SydneyResponse>());
                        }
                        break;
                    case 2: // 최종 데이터
                        if (data.Item == null) break;
                        SetData(data.Item.Value.Deserialize<SydneyResponse>());
                        break;
                    case 3: // 종료
                        await Task.Yield();
                        _ = socket.CloseAsync();
                        completed = true;
                        throttlingSource.TrySetResult(default);
                        messageSource.TrySetResult(messages);
                        return;
                    //case 4: // 송신 데이터
                    //case 5: // 송신 최종 데이터..?
                    case 6: // 핑
                        await socket.PingAsync();
                        break;
                }
                await Task.Yield();
            }
        } catch (Exception e) {
            throttlingSource.TrySetException(e);
            messageSource.TrySetException(e);
            completed = true;
            throw;
        }
    }

    private void SetData(SydneyResponse data) {
        if (data.Throttling != null) {
            throttlingSource.TrySetResult(data.Throttling.Value);
        }
        if (data.Messages != null) {
            messages = data.Messages;
            CheckApology(data.Messages);
            Interlocked.Exchange(ref messageSource, new()).SetResult(data.Messages);
        }
    }

    private void CheckApology(BotMessage[] messages) {
        var result = messages.All((item) => !string.Equals("Apology", item.ContentOrigin));
        if(result) lastsucessful = messages;
    }
}
