﻿using FakeItEasy;
using FluentAssertions;
using System.Drawing;
using TagsCloudCreation.Configs;
using TagsCloudCreation.TagsDrawingDecorators;

namespace TagsCloudCreation_Tests.TagsDrawingDecorators;

[TestFixture]
internal class GradientTagsDecoratorTests : TagsDrawingDecoratorTests
{
    private static readonly Color configMainColor = Color.FromArgb(0, 3, 6);
    private static readonly Color configSecondaryColor = Color.FromArgb(3, 3, 3);

    private TagsColorConfig tagsColorConfig;

    [SetUp]
    public void SetUp()
    {
        tagsColorConfig = new TagsColorConfig(configMainColor, configSecondaryColor, default);
        tagsDecorator = new GradientTagsDecorator(tagsColorConfig);
    }

    [Test]
    public void Constructor_ThrowsException_WhenTagsColorConfigIsNull()
    {
        var ctor = () => new GradientTagsDecorator(null!);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Decorate_SetsTagBrushesColorsInGradientByIndex()
    {
        var expectedColors = new[]
        {
            Color.FromArgb(0, 3, 6),
            Color.FromArgb(1, 3, 5),
            Color.FromArgb(2, 3, 4),
            Color.FromArgb(3, 3, 3),
        };

        var decoratedTagsResult = tagsDecorator.Decorate(tags);

        decoratedTagsResult.IsSuccess.Should().BeTrue();
        var decoratedTagsColors = decoratedTagsResult.Value.Select(tag => tag.Color);
        decoratedTagsColors.Should().BeEquivalentTo(expectedColors, options => options.WithStrictOrdering());
    }
}
