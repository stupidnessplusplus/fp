using FluentResults;

namespace TagsCloudCreation.TagsDrawingDecorators;

public interface ITagsDrawingDecorator
{
    public Result<TagDrawing[]> Decorate(IList<TagDrawing> tags);
}
