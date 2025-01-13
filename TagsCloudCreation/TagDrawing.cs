using System.Drawing;

namespace TagsCloudCreation;

public record TagDrawing(string Word, Rectangle Rectangle, Color Color, string? FontName, FontStyle FontStyle)
    : Tag(Word, Rectangle)
{
    public TagDrawing(Tag tag)
        : this(tag.Word, tag.Rectangle, default, null, default)
    {
    }
}
