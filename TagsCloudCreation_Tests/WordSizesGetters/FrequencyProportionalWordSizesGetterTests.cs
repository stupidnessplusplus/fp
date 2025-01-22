using FluentAssertions;
using TagsCloudCreation.Configs;
using TagsCloudCreation.WordSizesGetters;

namespace TagsCloudCreation_Tests.WordSizesGetters;

[TestFixture]
internal class FrequencyProportionalWordSizesGetterTests : WordSizesGetterTests
{
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        wordSizesGetter = new FrequencyProportionalWordSizesGetter(wordSizesGetterConfig, tagsFontConfig);
    }

    [Test]
    public void Constructor_ThrowsException_WhenWordSizesGetterConfigIsNull()
    {
        var ctor = () => new FrequencyProportionalWordSizesGetter(null!, tagsFontConfig);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_ThrowsException_WhenTagsFontConfigIsNull()
    {
        var ctor = () => new FrequencyProportionalWordSizesGetter(wordSizesGetterConfig, null!);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(1.5)]
    public void GetSizes_ReturnsTags_WithHeightsIncreasedByWordOccurencesNumber(double scale)
    {
        var words = new[] { "a", "b", "b", "c", "c", "d", "d", "d", "d" };
        wordSizesGetterConfig = wordSizesGetterConfig with { Scale = scale };
        wordSizesGetter = new FrequencyProportionalWordSizesGetter(wordSizesGetterConfig, tagsFontConfig);

        var unplacedTagsResult = wordSizesGetter.GetSizes(words);
        unplacedTagsResult.IsSuccess.Should().BeTrue();

        var aTag = unplacedTagsResult.Value.Single(tag => tag.Word == "a");
        var bTag = unplacedTagsResult.Value.Single(tag => tag.Word == "b");
        var cTag = unplacedTagsResult.Value.Single(tag => tag.Word == "c");
        var dTag = unplacedTagsResult.Value.Single(tag => tag.Word == "d");
        aTag.Size.Height.Should().Be(ConfigMinSize + (int)((words.Count(word => word == "a") - 1) * scale));
        bTag.Size.Height.Should().Be(ConfigMinSize + (int)((words.Count(word => word == "b") - 1) * scale));
        cTag.Size.Height.Should().Be(ConfigMinSize + (int)((words.Count(word => word == "c") - 1) * scale));
        dTag.Size.Height.Should().Be(ConfigMinSize + (int)((words.Count(word => word == "d") - 1) * scale));
    }

    [Test]
    public void GetSizes_ReturnsTags_SortedByHeightInDescendingOrder()
    {
        var words = new[] { "d", "a", "c", "c", "b", "d", "b", "d" };

        var unplacedTagsResult = wordSizesGetter.GetSizes(words);
        unplacedTagsResult.IsSuccess.Should().BeTrue();

        var actualHeights = unplacedTagsResult.Value.Select(tag => tag.Size.Height);
        actualHeights.Should().BeInDescendingOrder();
    }
}
