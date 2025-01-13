using System.Drawing;

namespace TagsCloudCreation.Configs;

public interface ITagsFontConfig
{
    public string FontName { get; }

    public FontStyle FontStyle { get; }
}
