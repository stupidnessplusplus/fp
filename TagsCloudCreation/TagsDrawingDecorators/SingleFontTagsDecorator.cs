using TagsCloudCreation.Configs;

namespace TagsCloudCreation.TagsDrawingDecorators;

public class SingleFontTagsDecorator : ITagsDrawingDecorator
{
    private readonly ITagsFontConfig fontConfig;

    public SingleFontTagsDecorator(ITagsFontConfig fontConfig)
    {
        ArgumentNullException.ThrowIfNull(fontConfig);

        this.fontConfig = fontConfig;
    }

    public TagDrawing[] Decorate(IList<TagDrawing> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        return tags
            .Select(tag => tag with
            {
                FontName = fontConfig.FontName,
                FontStyle = fontConfig.FontStyle,
            })
            .ToArray();
    }
}
