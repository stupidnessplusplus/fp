using FluentResults;
using System.Drawing;
using TagsCloudCreation.Configs;

namespace TagsCloudCreation.TagsDrawingDecorators;

public class GradientTagsDecorator : ITagsDrawingDecorator
{
    private readonly TagsColorConfig colorConfig;

    public GradientTagsDecorator(TagsColorConfig colorConfig)
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

        var dr = colorConfig.SecondaryColor.R - colorConfig.MainColor.R;
        var dg = colorConfig.SecondaryColor.G - colorConfig.MainColor.G;
        var db = colorConfig.SecondaryColor.B - colorConfig.MainColor.B;
        return tags
            .Select((tag, i) => tag with
            {
                Color = GetColor(i, tags.Count - 1, colorConfig.MainColor, dr, dg, db),
            })
            .ToArray()
            .ToResult()
            .WithSuccess($"Colored tags with a gradient from {colorConfig.MainColor} to {colorConfig.SecondaryColor}.");
    }

    private Color GetColor(int value, int valueRange, Color mainColor, int dr, int dg, int db)
    {
        var ratio = (double)value / valueRange;
        var r = (byte)(mainColor.R + ratio * dr);
        var g = (byte)(mainColor.G + ratio * dg);
        var b = (byte)(mainColor.B + ratio * db);
        return Color.FromArgb(r, g, b);
    }
}
