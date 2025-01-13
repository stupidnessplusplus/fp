using System.Drawing;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework.Interfaces;
using RectanglesCloudPositioning.Configs;
using TagsCloudApp;
using TagsCloudApp.Configs;
using TagsCloudCreation.Configs;
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

    private IDrawingAlgorithmsConfig algorithmsConfig;
    private IWordsSelectionConfig wordsSelectionConfig;
    private IWordSizesGetterConfig wordSizesGetterConfig;
    private IRectanglesPositioningConfig rectanglesPositioningConfig;
    private ITagsColorConfig colorConfig;
    private ITagsFontConfig fontConfig;

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

        algorithmsConfig = A.Fake<IDrawingAlgorithmsConfig>();
        A.CallTo(() => algorithmsConfig.WordSizingMethod).Returns(WordSizingMethod.SmoothFrequency);
        A.CallTo(() => algorithmsConfig.RectanglesLayouter).Returns(RectanglesLayouter.Circle);
        A.CallTo(() => algorithmsConfig.DrawingSettings).Returns([]);

        wordsSelectionConfig = A.Fake<IWordsSelectionConfig>();
        A.CallTo(() => wordsSelectionConfig.ExcludedWords).Returns(null);
        A.CallTo(() => wordsSelectionConfig.IncludedPartsOfSpeech).Returns(null);

        wordSizesGetterConfig = A.Fake<IWordSizesGetterConfig>();
        A.CallTo(() => wordSizesGetterConfig.MinSize).Returns(40);
        A.CallTo(() => wordSizesGetterConfig.Scale).Returns(1);

        rectanglesPositioningConfig = A.Fake<IRectanglesPositioningConfig>();
        A.CallTo(() => rectanglesPositioningConfig.Center).Returns(Point.Empty);
        A.CallTo(() => rectanglesPositioningConfig.RaysCount).Returns(360);
        A.CallTo(() => rectanglesPositioningConfig.RadiusEquation).Returns(null!);

        colorConfig = A.Fake<ITagsColorConfig>();
        A.CallTo(() => colorConfig.BackgroundColor).Returns(Color.FromArgb(255, 255, 255));
        A.CallTo(() => colorConfig.MainColor).Returns(Color.FromArgb(0, 0, 0));
        A.CallTo(() => colorConfig.SecondaryColor).Returns(default);

        fontConfig = A.Fake<ITagsFontConfig>();
        A.CallTo(() => fontConfig.FontName).Returns("Arial");
        A.CallTo(() => fontConfig.FontStyle).Returns(FontStyle.Regular);
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

        A.CallTo(() => algorithmsConfig.WordSizingMethod).Returns(WordSizingMethod.Frequency);
        A.CallTo(() => wordSizesGetterConfig.MinSize).Returns(minSize);
        A.CallTo(() => wordSizesGetterConfig.Scale).Returns(scale);

        app.Run(algorithmsConfig, wordsSelectionConfig, wordSizesGetterConfig,
            rectanglesPositioningConfig, colorConfig, fontConfig);

        outputImage.Size.Should().Be(expectedSize);
    }

    [Test]
    public void Run_CanSetColorInConfig()
    {
        inputText = "abc def ghi";

        A.CallTo(() => algorithmsConfig.DrawingSettings).Returns([DrawingSetting.Gradient]);
        A.CallTo(() => colorConfig.BackgroundColor).Returns(Color.FromArgb(63, 31, 63));
        A.CallTo(() => colorConfig.MainColor).Returns(Color.FromArgb(255, 191, 127));
        A.CallTo(() => colorConfig.SecondaryColor).Returns(Color.FromArgb(127, 63, 255));

        app.Run(algorithmsConfig, wordsSelectionConfig, wordSizesGetterConfig,
            rectanglesPositioningConfig, colorConfig, fontConfig);

        using var expectedImage = GetExpectedImageOrFail();
        outputImage.Should().Be(expectedImage, bitmapComparer);
    }

    [Test]
    public void Run_CanSetsFontInConfig()
    {
        inputText = "abc";

        A.CallTo(() => fontConfig.FontName).Returns("Constantia");
        A.CallTo(() => fontConfig.FontStyle).Returns(FontStyle.Strikeout);

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

        A.CallTo(() => wordsSelectionConfig.ExcludedWords).Returns([word]);

        app.Run(algorithmsConfig, wordsSelectionConfig, wordSizesGetterConfig,
            rectanglesPositioningConfig, colorConfig, fontConfig);

        outputImage.Size.Should().Be(expectedSize);
    }

    [Test]
    public void Run_CanChooseIncludedPartsOfSpeechInConfig()
    {
        inputText = "сиреневая сирень";

        A.CallTo(() => wordsSelectionConfig.IncludedPartsOfSpeech).Returns([PartOfSpeech.S]);

        app.Run(algorithmsConfig, wordsSelectionConfig, wordSizesGetterConfig,
            rectanglesPositioningConfig, colorConfig, fontConfig);

        using var expectedImage = GetExpectedImageOrFail();
        outputImage.Should().Be(expectedImage, bitmapComparer);
    }

    [Test]
    public void Run_CanSetRadiusFormulaInConfig()
    {
        inputText = "abc def ghi jkl mno pqr stu vwx yza bcd efg hij klm nop qrs tuv wxy";

        A.CallTo(() => algorithmsConfig.RectanglesLayouter).Returns(RectanglesLayouter.Shaped);
        A.CallTo(() => rectanglesPositioningConfig.RadiusEquation).Returns(angle => Math.Sin(5 * angle + Math.PI));

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
