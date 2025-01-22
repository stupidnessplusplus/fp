using FluentAssertions;
using System.Drawing;
using TagsCloudCreation.Configs;
using TagsCloudCreation.WordSizesGetters;

namespace TagsCloudCreation_Tests.WordSizesGetters;

[TestFixture]
internal abstract class WordSizesGetterTests
{
    protected const int ConfigMinSize = 8;
    protected const double ConfigScale = 1;
    protected const string ConfigFontName = "Arial";
    protected const FontStyle ConfigFontStyle = FontStyle.Regular;

    protected TagsFontConfig tagsFontConfig;
    protected WordSizesGetterConfig wordSizesGetterConfig;
    protected FrequencyProportionalWordSizesGetter wordSizesGetter = null!;

    [SetUp]
    public virtual void SetUp()
    {
        wordSizesGetterConfig = new WordSizesGetterConfig(ConfigMinSize, ConfigScale);
        tagsFontConfig = new TagsFontConfig(ConfigFontName, ConfigFontStyle);
    }

    [Test]
    public void GetSizes_Fails_WhenWordsListIsNull()
    {
        var unplacedTagsResult = wordSizesGetter.GetSizes(null!);
        unplacedTagsResult.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void GetSizes_ReturnsTagWithHeightEqualToConfigMinSize_IfWordOccursOnce()
    {
        var words = new[] { "a" };

        var unplacedTagsResult = wordSizesGetter.GetSizes(words);

        unplacedTagsResult.IsSuccess.Should().BeTrue();
        unplacedTagsResult.Value.Should().HaveCount(1);
        unplacedTagsResult.Value[0].Size.Height.Should().Be(ConfigMinSize);
    }
}
