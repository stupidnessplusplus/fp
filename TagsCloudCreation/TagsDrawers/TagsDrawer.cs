using FluentResults;
using Microsoft.Extensions.Logging;
using System.Drawing;
using TagsCloudCreation.Configs;

namespace TagsCloudCreation.TagsDrawers;

public class TagsDrawer : ITagsDrawer
{
    private readonly TagsColorConfig colorConfig;

    public TagsDrawer(TagsColorConfig colorConfig)
    {
        ArgumentNullException.ThrowIfNull(colorConfig);

        this.colorConfig = colorConfig;
    }

    public Result<Bitmap> Draw(IList<TagDrawing> tagDrawings)
    {
        if (tagDrawings == null)
        {
            return Result.Fail("Tags collection is null.");
        }

        var imageSize = GetImageSizeToFitTags(tagDrawings);
        return Result
            .Try(
                () => new Bitmap(imageSize.Width, imageSize.Height),
                _ => new Error($"Unable to create image of size {imageSize}."))
            .Bind(image => Result
                .Try(() =>
                {
                    FillBackground(image, colorConfig.BackgroundColor);
                    DrawTags(image, tagDrawings);
                    return image;
                }))
            .LogIfSuccess(nameof(TagsDrawer), "Image is drawn.", LogLevel.Information)
            .LogIfFailed(nameof(TagsDrawer), null, LogLevel.Error);
    }

    private Size GetImageSizeToFitTags(IList<TagDrawing> tags)
    {
        if (tags.Count == 0)
        {
            return new Size(1, 1);
        }

        var width = 2 * tags.Max(tag => Math.Max(Math.Abs(tag.Rectangle.Left), tag.Rectangle.Right));
        var height = 2 * tags.Max(tag => Math.Max(Math.Abs(tag.Rectangle.Top), tag.Rectangle.Bottom));
        return new Size(width, height);
    }

    private void FillBackground(Image image, Color color)
    {
        using var graphics = Graphics.FromImage(image);
        using var brush = new SolidBrush(color);
        graphics.FillRectangle(brush, new Rectangle(Point.Empty, image.Size));
    }

    private void DrawTags(Image image, IList<TagDrawing> tags)
    {
        using var graphics = Graphics.FromImage(image);

        foreach (var tag in CenterTags(image.Size, tags))
        {
            Draw(graphics, tag);
        }
    }

    private IEnumerable<TagDrawing> CenterTags(Size imageSize, IList<TagDrawing> tags)
    {
        var delta = new Size(imageSize.Width / 2, imageSize.Height / 2);

        return tags
            .Select(tag => tag with
            {
                Rectangle = tag.Rectangle with
                {
                    Location = tag.Rectangle.Location + delta,
                },
            });
    }

    private void Draw(Graphics graphics, TagDrawing tag)
    {
        if (tag.FontName == null)
        {
            return;
        }

        using var brush = new SolidBrush(tag.Color);
        using var font = new Font(tag.FontName, tag.Rectangle.Height, tag.FontStyle, GraphicsUnit.Pixel);
        graphics.DrawString(tag.Word, font, brush, tag.Rectangle.Location);
    }
}
