using FluentAssertions;
using System.Drawing;
using TagsCloudCreation;
using TagsCloudCreation.TagsDrawingDecorators;

namespace TagsCloudCreation_Tests.TagsDrawingDecorators;

[TestFixture]
internal abstract class TagsDrawingDecoratorTests
{
    protected static readonly TagDrawing[] tags =
    [
        new TagDrawing("a", default, default!, default!, default),
        new TagDrawing("b", default, default!, "Century", FontStyle.Italic),
        new TagDrawing("c", default, Color.White, default!, default),
        new TagDrawing("d", default, Color.White, "Century", FontStyle.Italic),
    ];

    protected ITagsDrawingDecorator tagsDecorator = null!;

    [Test]
    public void Decorate_Fails_WhenTagsListIsNull()
    {
        var decoratedTags = tagsDecorator.Decorate(null!);
        decoratedTags.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void Decorate_CreatesNewListAndTagDrawings()
    {
        var decoratedTags = tagsDecorator.Decorate(tags);

        decoratedTags.IsSuccess.Should().BeTrue();
        decoratedTags.Value.Should().NotBeSameAs(tags);
        decoratedTags.Value.Should()
            .AllSatisfy(decoratedTag => tags.Should()
                .AllSatisfy(tag => decoratedTag.Should().NotBeSameAs(tag)));
    }

    [Test]
    public void Decorate_KeepsTagsOrder()
    {
        var decoratedTags = tagsDecorator.Decorate(tags);

        decoratedTags.IsSuccess.Should().BeTrue();
        decoratedTags.Value.Should().HaveCount(tags.Length);
        decoratedTags.Value.Zip(tags).Should()
            .AllSatisfy(tagsPair => tagsPair.First.Word.Should().Be(tagsPair.Second.Word));
    }
}
