using FakeItEasy;
using FluentAssertions;
using System.Drawing;
using TagsCloudCreation.Configs;
using TagsCloudCreation.TagsDrawers;

namespace TagsCloudCreation_Tests.TagsDrawers;

[TestFixture]
internal class TagsDrawerTests
{
    private static readonly Color backgroundColor = Color.FromArgb(255, 255, 255);

    private ITagsColorConfig tagsColorConfig;
    private ITagsDrawer tagsDrawer;

    [SetUp]
    public void SetUp()
    {
        tagsColorConfig = A.Fake<ITagsColorConfig>();
        A.CallTo(() => tagsColorConfig.BackgroundColor).Returns(backgroundColor);

        tagsDrawer = new TagsDrawer(tagsColorConfig);
    }

    [Test]
    public void Constructor_ThrowsException_WhenTagsColorConfigIsNull()
    {
        var ctor = () => new TagsDrawer(null!);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Draw_ThrowsException_WhenTagsListIsNull()
    {
        var decorate = () => tagsDrawer.Draw(null!);
        decorate.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Draw_Returns1x1Image_WhenTagsListIsEmpty()
    {
        var expectedSize = new Size(1, 1);

        using var image = tagsDrawer.Draw([]);

        image.Size.Should().Be(expectedSize);
    }

    [Test]
    public void Draw_SetsImageBackgroundColor()
    {
        using var image = tagsDrawer.Draw([]);

        image.GetPixel(0, 0).Should().Be(backgroundColor);
    }
}
