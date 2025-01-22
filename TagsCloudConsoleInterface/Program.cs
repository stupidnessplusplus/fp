using CommandLine;
using CommandLine.Text;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.Drawing;
using TagsCloudApp;
using TagsCloudConsoleInterface.Configs;
using Error = FluentResults.Error;

namespace TagsCloudConsoleInterface;

public static class Program
{
    public static void Main(params string[] args)
    {
        Result.Setup(settings =>
        {
            settings.Logger = new ConsoleResultLogger();
            settings.DefaultTryCatchHandler = ex => new Error(ex.Message);
        });

        var parser = new Parser(settings =>
        {
            settings.CaseInsensitiveEnumValues = true;
            settings.HelpWriter = null;
        });

        var parserResult = parser.ParseArguments<ProgramConfig>(args);
        parserResult
            .WithParsed(Run)
            .WithNotParsed(_ => DisplayHelp(parserResult));
    }

    private static void Run(ProgramConfig programConfig)
    {
        var ioConfigResult = programConfig.GetIOConfig();
        var drawingAlgorithmsConfigResult = programConfig.GetDrawingAlgorithmsConfig();
        var wordsSelectionConfigResult = programConfig.GetWordsSelectionConfig();
        var wordSizesGetterConfigResult = programConfig.GetWordSizesGetterConfig();
        var rectanglesPositioningConfigResult = programConfig.GetRectanglesPositioningConfig();
        var tagsColorConfigResult = programConfig.GetTagsColorConfig();
        var tagsFontConfigResult = programConfig.GetTagsFontConfig();

        Result
            .Merge(
                ioConfigResult,
                drawingAlgorithmsConfigResult,
                wordsSelectionConfigResult,
                wordSizesGetterConfigResult,
                rectanglesPositioningConfigResult,
                tagsColorConfigResult,
                tagsFontConfigResult)
            .LogIfFailed(nameof(Program), null, LogLevel.Error)
            .Bind(() => new App(
                () => File.ReadAllText(ioConfigResult.Value.InputPath),
                image => Save(image, ioConfigResult.Value))
                .Run(
                    drawingAlgorithmsConfigResult.Value,
                    wordsSelectionConfigResult.Value,
                    wordSizesGetterConfigResult.Value,
                    rectanglesPositioningConfigResult.Value,
                    tagsColorConfigResult.Value,
                    tagsFontConfigResult.Value)
                .LogIfSuccess(
                    nameof(Program),
                    $"Image saved to '{ioConfigResult.Value.OutputPath}'.",
                    LogLevel.Information));
    }

    private static void DisplayHelp<T>(ParserResult<T> result)
    {
        var helpText = HelpText.AutoBuild(
            result,
            argHelpText =>
            {
                argHelpText.AddDashesToOption = true;
                argHelpText.AddEnumValuesToHelpText = true;
                return argHelpText;
            });

        Console.WriteLine(helpText);
    }

    private static Result Save(Bitmap image, IOConfig ioConfig)
    {
        var saveResult = Result
            .Try(() => Path.GetDirectoryName(ioConfig.OutputPath))
            .Bind(EnsureDirectoryExists)
            .Bind(() => Result.Try(
                () => image.Save(ioConfig.OutputPath, ioConfig.ImageFormat),
                ex => new Error($"Unable to save image to '{ioConfig.OutputPath}'. {ex?.Message}")));

        image.Dispose();
        return saveResult;
    }

    private static Result EnsureDirectoryExists(string? directoryPath)
    {
        return string.IsNullOrEmpty(directoryPath)
            || Directory.Exists(directoryPath)
            ? Result.Ok()
            : Result.Try(() => Directory.CreateDirectory(directoryPath)).ToResult();
    }
}
