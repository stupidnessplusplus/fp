using System.Drawing;

namespace TagsCloudCreation.TagsDrawers;

public interface ITagsDrawer
{
    public Bitmap Draw(IList<TagDrawing> tags);
}
