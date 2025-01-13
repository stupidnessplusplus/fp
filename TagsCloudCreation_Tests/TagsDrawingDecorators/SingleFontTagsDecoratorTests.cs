using FakeItEasy;
using FluentAssertions;
using System.Drawing;
using TagsCloudCreation.Configs;
using TagsCloudCreation.TagsDrawingDecorators;

namespace TagsCloudCreation_Tests.TagsDrawingDecorators;

[TestFixture]
internal class SingleFontTagsDecoratorTests : TagsDrawingDecoratorTests
{
    private const string ConfigFontName = "Arial";
    private const FontStyle ConfigFontStyle = FontStyle.Regular;

    private ITagsFontConfig tagsFontConfig;

    [SetUp]
    public void SetUp()
    {
        tagsFontConfig = A.Fake<ITagsFontConfig>();
        A.CallTo(() => tagsFontConfig.FontName).Returns(ConfigFontName);
        A.CallTo(() => tagsFontConfig.FontStyle).Returns(ConfigFontStyle);

        tagsDecorator = new SingleFontTagsDecorator(tagsFontConfig);
    }

    [Test]
    public void Constructor_ThrowsException_WhenTagsFontConfigIsNull()
    {
        var ctor = () => new SingleFontTagsDecorator(null!);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Decorate_SetsTagsFontNameAndFontStyle()
    {
        var expectedTags = tags
            .Select(tag => tag with
            {
                FontName = ConfigFontName,
                FontStyle = ConfigFontStyle,
            });

        var decoratedTags = tagsDecorator.Decorate(tags);

        decoratedTags.Should().BeEquivalentTo(expectedTags);
    }
}
