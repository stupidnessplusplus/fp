using FluentAssertions;
using TagsCloudCreation.WordSizesGetters;

namespace TagsCloudCreation_Tests.WordSizesGetters;

internal class SmoothFrequencyProportionalWordSizesGetterTests : WordSizesGetterTests
{
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        wordSizesGetter = new SmoothFrequencyProportionalWordSizesGetter(wordSizesGetterConfig, tagsFontConfig);
    }

    [Test]
    public void Constructor_ThrowsException_WhenWordSizesGetterConfigIsNull()
    {
        var ctor = () => new SmoothFrequencyProportionalWordSizesGetter(null!, tagsFontConfig);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_ThrowsException_WhenTagsFontConfigIsNull()
    {
        var ctor = () => new SmoothFrequencyProportionalWordSizesGetter(wordSizesGetterConfig, null!);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(1.5)]
    public void GetSizes_ReturnsTags_WithHeightsIncreasedByWordFrequencyPlacement(double scale)
    {
        var words = new[] { "a", "b", "b", "c", "c", "d", "d", "d", "d" };
        wordSizesGetterConfig = wordSizesGetterConfig with { Scale = scale };
        wordSizesGetter = new SmoothFrequencyProportionalWordSizesGetter(wordSizesGetterConfig, tagsFontConfig);

        var unplacedTagsResult = wordSizesGetter.GetSizes(words);
        unplacedTagsResult.IsSuccess.Should().BeTrue();

        var aTag = unplacedTagsResult.Value.Single(tag => tag.Word == "a");
        var bTag = unplacedTagsResult.Value.Single(tag => tag.Word == "b");
        var cTag = unplacedTagsResult.Value.Single(tag => tag.Word == "c");
        var dTag = unplacedTagsResult.Value.Single(tag => tag.Word == "d");
        aTag.Size.Height.Should().Be(ConfigMinSize + (int)(0 * scale));
        bTag.Size.Height.Should().Be(ConfigMinSize + (int)(1 * scale));
        cTag.Size.Height.Should().Be(ConfigMinSize + (int)(1 * scale));
        dTag.Size.Height.Should().Be(ConfigMinSize + (int)(2 * scale));
    }

    [Test]
    public void GetSizes_ReturnsTags_SortedByHeightInDescendingOrder()
    {
        var words = new[] { "d", "a", "c", "c", "b", "d", "b", "d", "d" };

        var unplacedTagsResult = wordSizesGetter.GetSizes(words);
        unplacedTagsResult.IsSuccess.Should().BeTrue();

        var actualHeights = unplacedTagsResult.Value.Select(tag => tag.Size.Height);
        actualHeights.Should().BeInDescendingOrder();
    }
}
