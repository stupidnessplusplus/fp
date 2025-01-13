using CommandLine;
using CommandLine.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using TagsCloudApp;

namespace TagsCloudConsoleInterface;

public static class Program
{
    public static void Main(params string[] args)
    {
        var parser = new Parser(with =>
        {
            with.CaseInsensitiveEnumValues = true;
            with.HelpWriter = null;
        });

        var parserResult = parser.ParseArguments<ProgramConfig>(args);
        parserResult
            .WithParsed(Run)
            .WithNotParsed(_ => DisplayHelp(parserResult));
    }

    private static void Run(ProgramConfig config)
    {
        var ioConfig = config as IIOConfig;
        var app = new App(
            () => File.ReadAllText(ioConfig.InputPath),
            image => Save(image, ioConfig));

        try
        {
            app.Run(config, config, config, config, config, config);
            Console.WriteLine($"Image saved to '{ioConfig.OutputPath}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
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

    private static void Save(Bitmap image, IIOConfig ioConfig)
    {
        var outputDirectoryName = Path.GetDirectoryName(ioConfig.OutputPath);

        if (!string.IsNullOrEmpty(outputDirectoryName)
            && !Directory.Exists(outputDirectoryName))
        {
            Directory.CreateDirectory(outputDirectoryName!);
        }

        try
        {
            image.Save(ioConfig.OutputPath, ioConfig.ImageFormat);
        }
        catch (ExternalException ex)
        {
            throw new Exception($"Unable to save image to '{ioConfig.OutputPath}'.", ex);
        }
        finally
        {
            image.Dispose();
        }
    }
}
