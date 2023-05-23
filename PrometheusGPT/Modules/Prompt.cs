namespace PrometheusGPT.Modules;

using PrometheusGPT.Models;

using System.Text;

public static class Prompt {
    public static string? GetPromptFormat() {
        try {
            return File.ReadAllText(Config.PromptFile);
        } catch {
            Console.WriteLine($"[오류][{DateTime.Now:hh:mm:ss}] 프롬포트 파일({Config.PromptFile})을 읽을 수 없습니다.");
        }
        return null;
    }

    public static (StringBuilder prompt, StringBuilder conv) ConvertMessage(Message[] messages) {
        StringBuilder prompt = new();
        StringBuilder conv = new();

        string? prevRole = default;
        foreach(var msg in messages) {
            switch(msg.Role) {
            case Message.ROLE_SYSTEM:   // 시스템 프롬프트
                prompt.AppendLine(msg.Content);
                prompt.AppendLine();
                break;

            case Message.ROLE_USER:         // 유저
                if(prevRole != msg.Role) {
                    conv.AppendLine(Config.RequestTag);
                }
                conv.AppendLine(Escape.EncodeText(msg.Content));
                conv.AppendLine();
                prevRole = msg.Role;
                break;

            case Message.ROLE_ASSISTANT:    // AI
                if (prevRole != msg.Role) {
                    conv.AppendLine(Config.ResponseTag);
                }
                conv.AppendLine(Escape.EncodeText(msg.Content));
                conv.AppendLine();
                prevRole = msg.Role;
                break;
            }
        }
        if(Config.PromptFile != null) {
            prompt.Append(Config.Prompt);
        }
        return (prompt, conv);
    }

    public static string? ParseText(string? text) {
        if (text == null) return null;
        int index = text.IndexOf(Config.RequestTag);
        return 0 <= index ? text[..index] :  text;
    }
}
