using System.Drawing;
using TagsCloudCreation.Configs;

namespace TagsCloudCreation.TagsDrawingDecorators;

public class SingleSolidColorTagsDecorator : ITagsDrawingDecorator
{
    private readonly ITagsColorConfig colorConfig;

    public SingleSolidColorTagsDecorator(ITagsColorConfig colorConfig)
    {
        ArgumentNullException.ThrowIfNull(colorConfig);

        this.colorConfig = colorConfig;
    }

    public TagDrawing[] Decorate(IList<TagDrawing> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);
        
        return tags
            .Select(tag => tag with { Color = colorConfig.MainColor })
            .ToArray();
    }
}
