using FluentResults;
using System.Drawing;

namespace TagsCloudCreation.TagsDrawers;

public interface ITagsDrawer
{
    public Result<Bitmap> Draw(IList<TagDrawing> tags);
}
