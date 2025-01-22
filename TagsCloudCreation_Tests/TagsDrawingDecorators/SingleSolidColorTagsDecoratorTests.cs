using FakeItEasy;
using FluentAssertions;
using System.Drawing;
using TagsCloudCreation.Configs;
using TagsCloudCreation.TagsDrawingDecorators;

namespace TagsCloudCreation_Tests.TagsDrawingDecorators;

[TestFixture]
internal class SingleSolidColorTagsDecoratorTests : TagsDrawingDecoratorTests
{
    private static readonly Color configMainColor = Color.FromArgb(0, 0, 0);

    private TagsColorConfig tagsColorConfig;

    [SetUp]
    public void SetUp()
    {
        tagsColorConfig = new TagsColorConfig(configMainColor, default, default);
        tagsDecorator = new SingleSolidColorTagsDecorator(tagsColorConfig);
    }

    [Test]
    public void Constructor_ThrowsException_WhenTagsColorConfigIsNull()
    {
        var ctor = () => new SingleSolidColorTagsDecorator(null!);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Decorate_SetsTagsBrush()
    {
        var expectedTags = tags
            .Select(tag => tag with { Color = configMainColor });

        var decoratedTags = tagsDecorator.Decorate(tags);

        decoratedTags.IsSuccess.Should().BeTrue();
        decoratedTags.Value.Should().BeEquivalentTo(expectedTags);
    }
}
