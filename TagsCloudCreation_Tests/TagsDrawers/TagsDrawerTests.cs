using FluentAssertions;
using FluentResults;
using System.Drawing;
using TagsCloudCreation.Configs;
using TagsCloudCreation.TagsDrawers;

namespace TagsCloudCreation_Tests.TagsDrawers;

[TestFixture]
internal class TagsDrawerTests
{
    private static readonly Color backgroundColor = Color.FromArgb(255, 255, 255);

    private TagsColorConfig tagsColorConfig;
    private ITagsDrawer tagsDrawer;

    private Result<Bitmap>? imageResult;

    [SetUp]
    public void SetUp()
    {
        tagsColorConfig = new TagsColorConfig(default, default, backgroundColor);
        tagsDrawer = new TagsDrawer(tagsColorConfig);
    }

    [TearDown]
    public void TearDown()
    {
        if (imageResult != null && imageResult.IsSuccess)
        {
            imageResult.Value?.Dispose();
        }
    }

    [Test]
    public void Constructor_ThrowsException_WhenTagsColorConfigIsNull()
    {
        var ctor = () => new TagsDrawer(null!);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Draw_Fails_WhenTagsListIsNull()
    {
        imageResult = tagsDrawer.Draw(null!);
        imageResult.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void Draw_Returns1x1Image_WhenTagsListIsEmpty()
    {
        var expectedSize = new Size(1, 1);

        imageResult = tagsDrawer.Draw([]);

        imageResult.IsSuccess.Should().BeTrue();
        imageResult.Value.Size.Should().Be(expectedSize);
    }

    [Test]
    public void Draw_SetsImageBackgroundColor()
    {
        imageResult = tagsDrawer.Draw([]);

        imageResult.IsSuccess.Should().BeTrue();
        imageResult.Value.GetPixel(0, 0).Should().Be(backgroundColor);
    }
}
