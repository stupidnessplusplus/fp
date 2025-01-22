using System.Drawing;
using FluentAssertions;
using NUnit.Framework.Interfaces;
using RectanglesCloudPositioning;
using RectanglesCloudPositioning.Configs;
using TagsCloudApp;
using TagsCloudApp.Configs;
using TagsCloudCreation.Configs;
using TagsCloudCreation.TagsDrawingDecorators;
using TagsCloudCreation.WordSizesGetters;
using WordsFiltration;
using WordsFiltration.Configs;

namespace TagsCloudApp_Tests;

[TestFixture]
internal class AppTests
{
    private const string ExpectedTestResultsDirectory = "expected";
    private const string FailedTestResultsDirectory = "failed_tests";

    private static IEqualityComparer<Bitmap> bitmapComparer = new BitmapEqualityComparer();

    private App app;

    private DrawingAlgorithmsConfig algorithmsConfig;
    private WordsSelectionConfig wordsSelectionConfig;
    private WordSizesGetterConfig wordSizesGetterConfig;
    private RectanglesPositioningConfig rectanglesPositioningConfig;
    private TagsColorConfig colorConfig;
    private TagsFontConfig fontConfig;

    private string inputText = null!;
    private Bitmap outputImage = null!;

    private string ExpectedTestResultPath => GetCurrentTestFilePath(ExpectedTestResultsDirectory);

    private string FailedTestResultPath => GetCurrentTestFilePath(FailedTestResultsDirectory);

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        if (Directory.Exists(FailedTestResultsDirectory))
        {
            foreach (var file in Directory.EnumerateFiles(FailedTestResultsDirectory))
            {
                File.Delete(file);
            }
        }
        else
        {
            Directory.CreateDirectory(FailedTestResultsDirectory);
        }
    }

    [SetUp]
    public void SetUp()
    {
        app = new App(() => inputText, image => outputImage = image);

        algorithmsConfig = new DrawingAlgorithmsConfig(
            typeof(SmoothFrequencyProportionalWordSizesGetter),
            typeof(SpiralCircularCloudLayouter),
            []);

        wordsSelectionConfig = new WordsSelectionConfig(null, null);
        wordSizesGetterConfig = new WordSizesGetterConfig(40, 1);
        rectanglesPositioningConfig = new RectanglesPositioningConfig(Point.Empty, 360, null);
        colorConfig = new TagsColorConfig(
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(255, 255, 255));
        fontConfig = new TagsFontConfig("Arial", FontStyle.Regular);
    }

    [TearDown]
    public void TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome == ResultState.Failure)
        {
            outputImage?.Save(FailedTestResultPath);
        }

        outputImage?.Dispose();
    }

    [TestCase("")]
    [TestCase("abc")]
    [TestCase("abc abc abc")]
    [TestCase("abc def ghi")]
    public void Run_CreatesImageWithWordsFromText(string inputText)
    {
        this.inputText = inputText;

        app.Run(algorithmsConfig, wordsSelectionConfig, wordSizesGetterConfig,
            rectanglesPositioningConfig, colorConfig, fontConfig);

        using var expectedImage = GetExpectedImageOrFail();
        outputImage.Should().Be(expectedImage, bitmapComparer);
    }

    [TestCase(10, 1, 24, 12)]
    [TestCase(30, 1, 64, 32)]
    [TestCase(10, 3, 32, 16)]
    public void Run_CreatedImageSizeDependsOnMinSizeAndScale(
        int minSize,
        double scale,
        int expectedWidth,
        int expectedHeight)
    {
        inputText = "abc abc abc";
        var expectedSize = new Size(expectedWidth, expectedHeight);

        algorithmsConfig = algorithmsConfig with
        {
            WordSizesGetterType = typeof(FrequencyProportionalWordSizesGetter),
        };

        wordSizesGetterConfig = new WordSizesGetterConfig(minSize, scale);

        app.Run(algorithmsConfig, wordsSelectionConfig, wordSizesGetterConfig,
            rectanglesPositioningConfig, colorConfig, fontConfig);

        outputImage.Size.Should().Be(expectedSize);
    }

    [Test]
    public void Run_CanSetColorInConfig()
    {
        inputText = "abc def ghi";

        algorithmsConfig = algorithmsConfig with
        {
            TagsDecoratorTypes = [typeof(GradientTagsDecorator)],
        };

        colorConfig = new TagsColorConfig(
            Color.FromArgb(255, 191, 127),
            Color.FromArgb(127, 63, 255),
            Color.FromArgb(63, 31, 63));

        app.Run(algorithmsConfig, wordsSelectionConfig, wordSizesGetterConfig,
            rectanglesPositioningConfig, colorConfig, fontConfig);

        using var expectedImage = GetExpectedImageOrFail();
        outputImage.Should().Be(expectedImage, bitmapComparer);
    }

    [Test]
    public void Run_CanSetsFontInConfig()
    {
        inputText = "abc";

        fontConfig = new TagsFontConfig("Constantia", FontStyle.Strikeout);

        app.Run(algorithmsConfig, wordsSelectionConfig, wordSizesGetterConfig,
            rectanglesPositioningConfig, colorConfig, fontConfig);

        using var expectedImage = GetExpectedImageOrFail();
        outputImage.Should().Be(expectedImage, bitmapComparer);
    }

    [Test]
    public void Run_CanExcludeWordsInConfig()
    {
        var word = "abc";
        inputText = word;
        var expectedSize = new Size(1, 1);

        wordsSelectionConfig = new WordsSelectionConfig([word], null);

        app.Run(algorithmsConfig, wordsSelectionConfig, wordSizesGetterConfig,
            rectanglesPositioningConfig, colorConfig, fontConfig);

        outputImage.Size.Should().Be(expectedSize);
    }

    [Test]
    public void Run_CanChooseIncludedPartsOfSpeechInConfig()
    {
        inputText = "сиреневая сирень";

        wordsSelectionConfig = new WordsSelectionConfig(null, [PartOfSpeech.S]);

        app.Run(algorithmsConfig, wordsSelectionConfig, wordSizesGetterConfig,
            rectanglesPositioningConfig, colorConfig, fontConfig);

        using var expectedImage = GetExpectedImageOrFail();
        outputImage.Should().Be(expectedImage, bitmapComparer);
    }

    [Test]
    public void Run_CanSetRadiusFormulaInConfig()
    {
        inputText = "abc def ghi jkl mno pqr stu vwx yza bcd efg hij klm nop qrs tuv wxy";

        algorithmsConfig = algorithmsConfig with
        {
            RectanglesLayouterType = typeof(ShapedCloudLayouter),
        };

        rectanglesPositioningConfig = rectanglesPositioningConfig with
        {
            RadiusEquation = angle => Math.Sin(5 * angle + Math.PI),
        };

        app.Run(algorithmsConfig, wordsSelectionConfig, wordSizesGetterConfig,
            rectanglesPositioningConfig, colorConfig, fontConfig);

        using var expectedImage = GetExpectedImageOrFail();
        outputImage.Should().Be(expectedImage, bitmapComparer);
    }

    private Bitmap GetExpectedImageOrFail()
    {
        try
        {
            return (Bitmap)Bitmap.FromFile(ExpectedTestResultPath);
        }
        catch
        {
            Assert.Fail();
            return null!;
        }
    }

    private string GetCurrentTestFilePath(string directory)
    {
        var testName = TestContext.CurrentContext.Test.Name.Replace("\"", "'");
        var fileName = $"{testName}.png";
        return Path.Combine(directory, fileName);
    }
}
