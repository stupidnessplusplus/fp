using System.Drawing;
using TagsCloudCreation.WordSizesGetters;
using TagsCloudCreation.TagsDrawingDecorators;
using TagsCloudCreation.TagsDrawers;
using RectanglesCloudPositioning;

namespace TagsCloudCreation;

public class TagsCloudCreator
{
    private readonly IWordSizesGetter wordSizesGetter;
    private readonly ICloudLayouter cloudLayouter;
    private readonly IEnumerable<ITagsDrawingDecorator> tagsSettingsSetters;
    private readonly ITagsDrawer tagsDrawer;

    public TagsCloudCreator(
        IWordSizesGetter wordSizesGetter,
        ICloudLayouter cloudLayouter,
        IEnumerable<ITagsDrawingDecorator> tagsSettingsSetters,
        ITagsDrawer tagsDrawer)
    {
        ArgumentNullException.ThrowIfNull(wordSizesGetter);
        ArgumentNullException.ThrowIfNull(cloudLayouter);
        ArgumentNullException.ThrowIfNull(tagsSettingsSetters);
        ArgumentNullException.ThrowIfNull(tagsDrawer);

        this.wordSizesGetter = wordSizesGetter;
        this.cloudLayouter = cloudLayouter;
        this.tagsSettingsSetters = tagsSettingsSetters;
        this.tagsDrawer = tagsDrawer;
    }

    public Bitmap DrawTagsCloud(IList<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        var tags = wordSizesGetter
            .GetSizes(words)
            .Select(unplacedTag => new Tag(unplacedTag.Word, cloudLayouter.PutNextRectangle(unplacedTag.Size)))
            .ToArray();

        var tagDrawings = GetTagDrawings(tags);
        return tagsDrawer.Draw(tagDrawings);
    }

    private TagDrawing[] GetTagDrawings(IList<Tag> tags)
    {
        var tagDrawings = tags
            .Select(tag => new TagDrawing(tag))
            .ToArray();

        return tagsSettingsSetters
            .Aggregate(tagDrawings, (tags, setter) => setter.Decorate(tags));
    }
}
