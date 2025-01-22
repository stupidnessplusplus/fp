using FluentResults;
using TagsCloudCreation.Configs;

namespace TagsCloudCreation.TagsDrawingDecorators;

public class SingleFontTagsDecorator : ITagsDrawingDecorator
{
    private readonly TagsFontConfig fontConfig;

    public SingleFontTagsDecorator(TagsFontConfig fontConfig)
    {
        ArgumentNullException.ThrowIfNull(fontConfig);

        this.fontConfig = fontConfig;
    }

    public Result<TagDrawing[]> Decorate(IList<TagDrawing> tags)
    {
        if (tags == null)
        {
            return Result.Fail("Tags collection is null.");
        }

        return tags
            .Select(tag => tag with
            {
                FontName = fontConfig.FontName,
                FontStyle = fontConfig.FontStyle,
            })
            .ToArray()
            .ToResult()
            .WithSuccess($"Set tags font to {fontConfig.FontName} {fontConfig.FontStyle}.");
    }
}
