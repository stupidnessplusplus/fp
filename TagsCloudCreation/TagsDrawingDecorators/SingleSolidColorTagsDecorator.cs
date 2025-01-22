using FluentResults;
using System.Drawing;
using TagsCloudCreation.Configs;

namespace TagsCloudCreation.TagsDrawingDecorators;

public class SingleSolidColorTagsDecorator : ITagsDrawingDecorator
{
    private readonly TagsColorConfig colorConfig;

    public SingleSolidColorTagsDecorator(TagsColorConfig colorConfig)
    {
        ArgumentNullException.ThrowIfNull(colorConfig);

        this.colorConfig = colorConfig;
    }

    public Result<TagDrawing[]> Decorate(IList<TagDrawing> tags)
    {
        if (tags == null)
        {
            return Result.Fail("Tags collection is null.");
        }

        return tags
            .Select(tag => tag with { Color = colorConfig.MainColor })
            .ToArray()
            .ToResult()
            .WithSuccess($"Colored tags with {colorConfig.MainColor}.");
    }
}
