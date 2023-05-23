namespace PrometheusGPT.Modules;

using PrometheusGPT.Models;

using System.Text.Json;

using static PrometheusGPT.Models.ConfigData;

public static class Config
{
    private const string CONFIG_FILE = "config.json";
    private static readonly ConfigData config = Load();

    private static ConfigData Load()
    {
        ConfigData? data = null;
        try
        {
            using (var fs = File.OpenRead(CONFIG_FILE))
                data = JsonSerializer.Deserialize<ConfigData>(fs);
        }
        catch { }
        if (data == null)
        {
            Console.WriteLine($"설정 파일 ({CONFIG_FILE})이 잘못되었습니다.");
            return new();
        }
        return data;
    }


    public static string? ListenUrl => config.ListenUrl;

    public static int RetryCount => config.RetryCount;

    public static int RetryTime => config.RetryTime;

    public static string RequestTag => config.RequestTag;

    public static string ResponseTag => config.ResponseTag;

    public static string? Prompt => config.Prompt;

    public static string PromptFile => config.PromptFile;

    public static EscapeData Escape => config.Escape;

}
