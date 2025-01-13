using FakeItEasy;
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

    protected ITagsFontConfig tagsFontConfig;
    protected IWordSizesGetterConfig wordSizesGetterConfig;
    protected FrequencyProportionalWordSizesGetter wordSizesGetter = null!;

    [SetUp]
    public virtual void SetUp()
    {
        wordSizesGetterConfig = A.Fake<IWordSizesGetterConfig>();
        A.CallTo(() => wordSizesGetterConfig.MinSize).Returns(ConfigMinSize);
        A.CallTo(() => wordSizesGetterConfig.Scale).Returns(ConfigScale);

        tagsFontConfig = A.Fake<ITagsFontConfig>();
        A.CallTo(() => tagsFontConfig.FontName).Returns(ConfigFontName);
        A.CallTo(() => tagsFontConfig.FontStyle).Returns(ConfigFontStyle);
    }

    [Test]
    public void GetSizes_ThrowsException_WhenWordsListIsNull()
    {
        var getSizes = () => wordSizesGetter.GetSizes(null!);
        getSizes.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void GetSizes_ReturnsTagWithHeightEqualToConfigMinSize_IfWordOccursOnce()
    {
        var words = new[] { "a" };

        var unplacedTags = wordSizesGetter.GetSizes(words);

        unplacedTags.Should().HaveCount(1);
        unplacedTags[0].Size.Height.Should().Be(ConfigMinSize);
    }
}
