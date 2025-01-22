using System.Drawing;
using TagsCloudCreation.WordSizesGetters;
using TagsCloudCreation.TagsDrawingDecorators;
using TagsCloudCreation.TagsDrawers;
using RectanglesCloudPositioning;
using FluentResults;
using Microsoft.Extensions.Logging;

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

    public Result<Bitmap> DrawTagsCloud(IList<string> words)
    {
        return Result
            .FailIf(words == null, new Error("Words collection is null."))
            .LogIfFailed(nameof(TagsCloudCreator), null, LogLevel.Error)
            .Bind(() => wordSizesGetter.GetSizes(words!))
            .Bind(unplacedTags => unplacedTags
                .Select(unplacedTag => cloudLayouter
                    .PutNextRectangle(unplacedTag.Size)
                    .LogIfFailed(LogLevel.Warning)
                    .Bind(rectangle => Result.Ok(new Tag(unplacedTag.Word, rectangle))))
                .Where(putResult => putResult.IsSuccess)
                .Select(putResult => putResult.Value)
                .ToArray()
                .ToResult()
                .Log(nameof(TagsCloudCreator), "Placed tags.", LogLevel.Information))
            .Bind(GetTagDrawings)
            .Bind(tagsDrawer.Draw);
    }

    private Result<TagDrawing[]> GetTagDrawings(IList<Tag> tags)
    {
        var tagDrawingsResult = tags
            .Select(tag => new TagDrawing(tag))
            .ToArray()
            .ToResult();

        foreach (var tagsSettingsSetter in tagsSettingsSetters)
        {
            var tagsSettingsSetterName = tagsSettingsSetter.GetType().Name;
            tagDrawingsResult = tagsSettingsSetter
                .Decorate(tagDrawingsResult.Value)
                .LogIfSuccess(tagsSettingsSetterName, null, LogLevel.Information)
                .LogIfFailed(tagsSettingsSetterName, null, LogLevel.Error);

            if (tagDrawingsResult.IsFailed)
            {
                return Result.Fail("Unable to apply settings to tags.");
            }
        }

        return Result.Ok(tagDrawingsResult.Value);
    }
}
