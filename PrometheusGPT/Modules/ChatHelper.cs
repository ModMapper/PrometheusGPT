namespace PrometheusGPT.Modules;

using PrometheusAPI;
using PrometheusAPI.Models.Message;

using PrometheusGPT.Models;

using System.Text;

public static class ChatHelper
{

    public static async Task<ChatResult?> ChatAsync(Request request)
    {
        if (request.Messages == null) return null;
        var conv = await CreateConversationAsync();
        if(conv == null) return null;
        var chat = CreateChats(request.Messages);
        for(int i = 0; i < Config.RetryCount; i++) {
            try {
                return await conv.ChatAsync(chat);
            } catch { }
        }
        return null;
    }

    public static string? ParseText(string? text)
        => Prompt.ParseText(Escape.DecodeText(text));

    private static async Task<Conversation?> CreateConversationAsync() {
        for(int i = 0; i < Config.RetryCount; i++) {
            try {
                return await Conversation.CreateAsync();
            } catch {
                Console.WriteLine($"[경고][{DateTime.Now:hh:mm:ss}] 빙 대화 생성 실패!");
            }
            await Task.Delay(Config.RetryTime);
            await Task.Yield();
        }
        return null;
    }

    private static Chat CreateChats(Message[] messages)
    {
        // 프롬프트 메세지 생성
        var promptmsg = new ChatMessage() { Author = IMessage.AUTHOR_BOT };
        var usermsg = new ChatMessage() { Author = IMessage.AUTHOR_USER };

        // 새 채팅 생성
        Chat chat = new(usermsg) {
            Options = Options,
            AllowedTypes = AllowedTypes,
            SliceId = SliceId
        };
        chat.PrevMessages.Add(promptmsg);

        // 프롬프트 작성
        var promptFromat = Prompt.GetPromptFormat() ?? "{0}";
        var (prompt, conv) = Prompt.ConvertMessage(messages);
        
        // 검열 방지용 Escape 인코딩
        Escape.EncodeText(prompt);
        Escape.EncodeText(conv);

        // 메세지 작성
        promptmsg.Text = string.Format(promptFromat, prompt);
        prompt.Append(conv);
        usermsg.Text = string.Format(promptFromat, prompt);

        // 검열 방지 Escape 프롬프트
        var escape = Escape.GetPrompt();
        if(escape != null) {
            promptmsg.Text += escape;
            usermsg.Text += escape;
        }

        return chat;
    }

    private static List<string> Options = new List<string>() {
        //"nlu_direct_response_filter",
        "deepleo",
        "enable_debug_commands",
        "disable_emoji_spoken_text",
        //"responsible_ai_policy_235",
        //"enablemm",
        "h3imaginative",
        //"galileo",
        "clgalileo",
        //"cachewriteext",
        //"e2ecachewrite",
        "gencontentv3",
        "gencontentv5",
        "nodlcpcwrite",
        "nointernalsugg",
        //"responseos",
        //"soedgeca",
        //"enbfpr",
        //"dv3sugg",
    };

    private static List<string> AllowedTypes = new() {
        //"ActionRequest",
        "Chat",
        "InternalSearchQuery",
        "InternalSearchResult",
        //"Disengaged",
        //"InternalLoaderMessage",
        //"RenderCardRequest",
        //"AdsQuery",
        //"SemanticSerp",
        "GenerateContentQuery",
        "SearchQuery"
    };

    private static string[] SliceId = new[] { "귫뜛" };
}
