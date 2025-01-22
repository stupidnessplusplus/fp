using CommandLine;
using FluentResults;
using RectanglesCloudPositioning.Configs;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using TagsCloudApp;
using TagsCloudApp.Configs;
using TagsCloudCreation.Configs;
using WordsFiltration;
using WordsFiltration.Configs;
using Error = FluentResults.Error;

namespace TagsCloudConsoleInterface.Configs;

internal class ProgramConfig
{
    [Option('i', "in", Required = true,
        HelpText = "Sets the path to the input file.")]
    public string InputPath { get; private set; } = null!;

    [Option('o', "out", Required = true,
        HelpText = "Sets the path to the output file.")]
    public string OutputPath { get; private set; } = null!;

    [Option("image-format", Required = false, Default = OutputImageFormat.Png,
        HelpText = "Specifies the output file format.")]
    public OutputImageFormat OutputFormat { get; private set; }

    [Option('s', "sizing", Required = false, Default = WordSizingMethod.SmoothFrequency,
        HelpText = "Specifies the word sizing method.")]
    public WordSizingMethod WordSizingMethod { get; private set; }

    [Option('l', "layouter", Required = false, Default = RectanglesLayouter.Circle,
        HelpText = "Specifies the tags layout type.")]
    public RectanglesLayouter RectanglesLayouter { get; private set; }

    [Option('d', "drawing-settings", Required = false, Default = new DrawingSetting[0],
        HelpText = $"Specifies additional tags processing methods. Valid values: {nameof(DrawingSetting.Gradient)}")]
    public IEnumerable<DrawingSetting> DrawingSettings {get; private set; } = null!;

    [Option("exclude-words", Required = false, Default = null,
        HelpText = "Sets the path to the file with excluded words.")]
    public string? ExcludedWordsPath { get; private set; }

    [Option("pos", Required = false, Default = new PartOfSpeech[] { PartOfSpeech.A, PartOfSpeech.S, PartOfSpeech.V },
        HelpText = "Specifies included parts of speech. "
            + "Valid values: Unknown, A, ADV, ADVPRO, ANUM, APRO, COM, CONJ, INTJ, NUM, PART, PR, S, SPRO, V")]
    public IEnumerable<PartOfSpeech> IncludedPartsOfSpeech { get; private set; } = null!;

    [Option("min-word-size", Required = false, Default = 10,
        HelpText = "Sets the min word size.")]
    public int MinSize { get; private set; }

    [Option("word-size-scale", Required = false, Default = 1,
        HelpText = "Sets the word size scale. Difference between the word sizes will be multiplied by this value.")]
    public double Scale { get; private set; }

    public Point Center => Point.Empty;

    [Option("rays-count", Required = false, Default = 360,
        HelpText = "Sets the number of rays from the center of the tag cloud. Words will be placed along the rays.")]
    public int RaysCount { get; private set; }

    [Option("radius", Required = false, Default = "1",
        HelpText = "Sets the right side of the radius equation.")]
    public string RadiusEquationString { get; private set; } = null!;

    [Option("background", Required = false, Default = "#FFF",
        HelpText = "Sets the background color.")]
    public string BackgroundColorHex { get; private set; } = null!;

    [Option("main-color", Required = false, Default = "#000",
        HelpText = "Sets the main color of words.")]
    public string MainColorHex { get; private set; } = null!;

    [Option("secondary-color", Required = false, Default = "#000",
        HelpText = "Sets the secondary color of words.")]
    public string SecondaryColorHex { get; private set; } = null!;

    [Option("font", Required = false, Default = "Arial",
        HelpText = "Sets the font of words.")]
    public string FontName { get; private set; } = null!;

    [Option("font-style", Required = false, Default = FontStyle.Regular,
        HelpText = "Sets the font style of words.")]
    public FontStyle FontStyle { get; private set; }

    public Result<IOConfig> GetIOConfig()
    {
        var imageFormatResult = OutputFormat switch
        {
            OutputImageFormat.Png => Result.Ok(ImageFormat.Png),
            OutputImageFormat.Jpeg => Result.Ok(ImageFormat.Jpeg),
            OutputImageFormat.Bmp => Result.Ok(ImageFormat.Bmp),
            _ => Result.Fail("Unknown image format."),
        };

        return ValidateFilePath(InputPath)
            .Bind(() => ValidateFilePath(OutputPath))
            .Bind(() => Result
                .FailIf(!File.Exists(InputPath), new Error($"Input file not found: '{InputPath}'.")))
            .Bind(() => imageFormatResult)
            .Bind(imageFormat => Result
                .Ok(new IOConfig(InputPath, OutputPath, imageFormat)));
    }

    public Result<DrawingAlgorithmsConfig> GetDrawingAlgorithmsConfig()
    {
        return DrawingAlgorithmsConfig
            .FromEnums(WordSizingMethod, RectanglesLayouter, DrawingSettings.ToArray());
    }

    public Result<WordsSelectionConfig> GetWordsSelectionConfig()
    {
        var excludedWordsResult = ExcludedWordsPath == null
            ? Result.Ok(default(string[]))
            : ValidateFilePath(ExcludedWordsPath)
                .Bind(() => Result
                    .FailIf(
                        !File.Exists(ExcludedWordsPath),
                        new Error($"Excluded words file not found: '{ExcludedWordsPath}'.")))
                .Bind(() => Result
                    .Try<string[]?>(() => File.ReadAllText(ExcludedWordsPath).Split()));

        return excludedWordsResult
            .Bind(excludedWords => Result
                .Ok(new WordsSelectionConfig(excludedWords, IncludedPartsOfSpeech?.ToArray())));
    }

    public Result<WordSizesGetterConfig> GetWordSizesGetterConfig()
    {
        return Result
            .FailIf(MinSize <= 0, new Error($"Min word size should be a positive number."))
            .Bind(() => Result
                .FailIf(Scale <= 0, new Error($"Word size scale should be a positive number.")))
            .Bind(() => Result
                .Ok(new WordSizesGetterConfig(MinSize, Scale)));
    }

    public Result<RectanglesPositioningConfig> GetRectanglesPositioningConfig()
    {
        return Result
            .FailIf(RaysCount <= 0, new Error($"Rays count should be a positive number."))
            .Bind(() => RadiusEquationParser.ParseRadiusEquation(RadiusEquationString))
            .Bind(radiusEquation => Result
                .Ok(new RectanglesPositioningConfig(Center, RaysCount, radiusEquation)));
    }

    public Result<TagsColorConfig> GetTagsColorConfig()
    {
        return Result
            .Merge(
                ParseColorHex(MainColorHex),
                ParseColorHex(SecondaryColorHex),
                ParseColorHex(BackgroundColorHex))
            .Bind(colors => Result.Ok(colors.ToArray()))
            .Bind(colors => Result
                .Ok(new TagsColorConfig(colors[0], colors[1], colors[2])));
    }

    public Result<TagsFontConfig> GetTagsFontConfig()
    {
        using var installedFonts = new InstalledFontCollection();
        var hasFont = installedFonts.Families
            .Any(fontFamily => fontFamily.Name.Equals(FontName, StringComparison.InvariantCultureIgnoreCase));

        return Result
            .FailIf(!hasFont, new Error($"Font '{FontName}' does not exist."))
            .Bind(() => Result.Ok(new TagsFontConfig(FontName, FontStyle)));
    }

    private Result ValidateFilePath(string path)
    {
        var isInvalidPath = path == null
            || Path.GetInvalidFileNameChars().Any(path.Contains);

        return Result.FailIf(isInvalidPath, new Error($"Invalid path: '{path}'."));
    }

    private Result<Color> ParseColorHex(string? hex)
    {
        return Result
            .FailIf(hex == null, new Error($"Color hex is null."))
            .Bind(() => Result
                .Try(
                    () => ColorTranslator.FromHtml(hex!),
                    ex => new Error($"Unable to parse color: '{hex}'.")));
    }
}
