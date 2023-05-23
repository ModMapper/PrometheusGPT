namespace PrometheusGPT.Modules;

using System.Text;

public static class Escape {
    public static bool Enabled => Config.Escape.Enabled;

    public static string? GetPrompt() {
        if (!Config.Escape.Enabled) return null;
        StringBuilder sb = new();
        if (Config.Escape.IgnoreChar != null && Config.Escape.IgnorePrompt != null) {
            sb.AppendFormat(Config.Escape.IgnorePrompt, Config.Escape.IgnoreChar, Config.Escape.IgnoreInsert);
        }
        if (Config.Escape.SpaceChar != null && Config.Escape.SpacePrompt != null) {
            sb.AppendFormat(Config.Escape.SpacePrompt, Config.Escape.SpaceChar);
        }
        if (Config.Escape.NewlineChar != null && Config.Escape.NewlinePrompt != null) {
            sb.AppendFormat(Config.Escape.NewlinePrompt, Config.Escape.NewlineChar);
        }
        return sb.ToString();
    }

    public static void EncodeText(StringBuilder text) {
        if (!Config.Escape.Enabled) return;
        if (Config.Escape.SpaceChar != null) {
            text.Replace(' ', Config.Escape.SpaceChar.Value);
        }
        if (Config.Escape.NewlineChar != null) {
            text.Replace("\r\n", Config.Escape.NewlineChar.Value.ToString());
            text.Replace('\r', Config.Escape.NewlineChar.Value);
            text.Replace('\n', Config.Escape.NewlineChar.Value);
        }
    }

    public static string? EncodeText(string? text) {
        if (text == null) return null;
        if (!Config.Escape.Enabled) return text;
        if (Config.Escape.IgnoreChar != null && 0 < Config.Escape.IgnoreInsert) {
            StringBuilder sb = new();
            ReadOnlySpan<char> span = text;
            while(Config.Escape.IgnoreInsert < span.Length) {
                sb.Append(span[..Config.Escape.IgnoreInsert]);
                sb.Append(Config.Escape.IgnoreChar);
                span = span[Config.Escape.IgnoreInsert..];
            }
            sb.Append(span);
            return sb.ToString();
        }
        return text;
    }

    public static string? DecodeText(string? text) {
        if (text == null) return null;
        if (!Config.Escape.Enabled) return text;
        if (Config.Escape.IgnoreChar != null) {
            text = text.Replace(Config.Escape.IgnoreChar.Value.ToString(), "");
        }
        if (Config.Escape.NewlineChar != null) {
            text = text.Replace(Config.Escape.NewlineChar.Value, '\n');
        }
        if (Config.Escape.SpaceChar != null) {
            text = text.Replace(Config.Escape.SpaceChar.Value, ' ');
        }
        return text;
    }
}
