using PrometheusAPI;
using PrometheusAPI.Models.Message;
using PrometheusGPT.Models;
using PrometheusGPT.Modules;
using PrometheusGPT.Properties;

using System.Reflection.Metadata.Ecma335;

internal class Program {
    private static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        if(Config.ListenUrl != null) {
            builder.WebHost.UseUrls(Config.ListenUrl);
        }
        
        var app = builder.Build();
        app.MapGet("/", Index);
        app.MapGet("OpenAI", Status);
        app.MapPost("OpenAI", API);
        app.MapFallback(Fallback);
        app.Run();
    }

    public static void Fallback(HttpContext context)
        => context.Response.Redirect("/");

    public static async Task Index(HttpContext context) {
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(Resources.IndexPage);
    }

    public static async Task Status(HttpContext context) {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"status\": \"running\", \"message\": \"GET method is not allowed!\"}");
    }

    public static async Task API(HttpContext context, Request req) {
        ChatResult? result = null;
        for(int i = 0; i < Config.RetryCount; i++) {
            try {
                result = await ChatHelper.ChatAsync(req);
            } catch { }
            try {
                if (result != null) {
                    if (!req.Stream) {
                        await SendChatPlainAsync(context, result);
                    } else {
                        await SendChatStreamAsync(context, result);
                    }
                } else {
                    if (!req.Stream) {
                        await SendChatEmptyAsync(context);
                    }
                }
                return;
            } catch { }
        }
    }

    private static async Task SendChatEmptyAsync(HttpContext context) {
        Response response = new() {
            //Id = Guid.NewGuid().ToString(),
            Type = "chat.completion",
            Created = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Usage = new() { Prompt = 0, Completion = 1, Total = 1 },
        };

        response.Choices = new MessageResult[] { new() {
            Index = 0,
            Message = new() {
                Role = Message.ROLE_ASSISTANT,
                Content = "",
            },
            Reason = "stop", } };

        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task SendChatPlainAsync(HttpContext context, ChatResult result) {
        Response response = new() {
            //Id      = Guid.NewGuid().ToString(),
            Type    = "chat.completion",
            Created = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Usage   = new() { Prompt = 0, Completion = 1, Total = 1 },
        };

        try {
            do {
                var text = await result.GetTextAsync();
                Console.WriteLine($"[정보][{DateTime.Now:hh:mm:ss}] 대화 생성됨");
                Console.WriteLine(ChatHelper.ParseText(text));
                Console.WriteLine();
                Console.WriteLine();
                await Task.Yield();
            } while (!result.IsCompleted);
            await result.WaitAsync();
        } catch { }
        var resultText = ChatHelper.ParseText(result.SucessfulText ?? result.Text) ?? string.Empty;
        if (resultText.Length == 0) throw new Exception();

        response.Choices = new MessageResult[] { new() {
            Index = 0,
            Message = new() {
                Role = Message.ROLE_ASSISTANT,
                Content = resultText,
            },
            Reason = "stop", } };

        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task SendChatStreamAsync(HttpContext context, ChatResult result) {
        try {
            context.Response.ContentType = "text/event-stream";
            using (StreamWriter writer = new(context.Response.Body)) {
                int index = 0;
                do {
                    var text = await result.GetTextAsync();
                    var stext = ChatHelper.ParseText(result.SucessfulText);
                    Console.WriteLine($"[정보][{DateTime.Now:hh:mm:ss}] 대화 생성됨");
                    Console.WriteLine(ChatHelper.ParseText(text));
                    Console.WriteLine();
                    Console.WriteLine();

                    if (stext == null) return;
                    await writer.WriteAsync("data: ");
                    await writer.WriteLineAsync(stext[..index]);
                    await writer.WriteLineAsync();
                    await context.Response.Body.FlushAsync();
                    index = stext.Length;
                    await Task.Yield();
                } while (!result.IsCompleted);
            }
        } catch { }
    }
}