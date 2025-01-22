using FluentResults;
using Microsoft.Extensions.Logging;

namespace TagsCloudConsoleInterface;

public class ConsoleResultLogger : IResultLogger
{
    public void Log<TContext>(string content, ResultBase result, LogLevel logLevel)
    {
        Log(nameof(TContext), content, result, logLevel);
    }

    public void Log(string context, string content, ResultBase result, LogLevel logLevel)
    {
        content ??= string.Join(' ', result.Reasons.Select(reason => reason.Message));
        Log(context, content, logLevel);
    }

    private void Log(string context, string content, LogLevel logLevel)
    {
        if (logLevel == LogLevel.None)
        {
            return;
        }

        var defaultColor = Console.ForegroundColor;
        var logLevelColor = logLevel switch
        {
            LogLevel.Trace => defaultColor,
            LogLevel.Debug => defaultColor,
            LogLevel.Information => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.Cyan,
            _ => defaultColor,
        };

        Console.ForegroundColor = logLevelColor;
        Console.Write($"[{logLevel}]");
        Console.ForegroundColor = defaultColor;
        Console.WriteLine($" <{context}> {content}");
    }
}
