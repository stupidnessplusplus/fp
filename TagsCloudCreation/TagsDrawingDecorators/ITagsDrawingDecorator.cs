namespace TagsCloudCreation.TagsDrawingDecorators;

public interface ITagsDrawingDecorator
{
    public TagDrawing[] Decorate(IList<TagDrawing> tags);
}
