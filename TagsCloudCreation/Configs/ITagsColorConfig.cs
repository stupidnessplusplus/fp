using System.Drawing;

namespace TagsCloudCreation.Configs;

public interface ITagsColorConfig
{
    public Color MainColor { get; }
    
    public Color SecondaryColor { get; }

    public Color BackgroundColor { get; }
}
