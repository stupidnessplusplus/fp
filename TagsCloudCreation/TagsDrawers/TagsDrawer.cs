using System.Drawing;
using TagsCloudCreation.Configs;

namespace TagsCloudCreation.TagsDrawers;

public class TagsDrawer : ITagsDrawer
{
    private readonly ITagsColorConfig colorConfig;

    public TagsDrawer(ITagsColorConfig colorConfig)
    {
        ArgumentNullException.ThrowIfNull(colorConfig);

        this.colorConfig = colorConfig;
    }

    public Bitmap Draw(IList<TagDrawing> tagsWithSettings)
    {
        ArgumentNullException.ThrowIfNull(tagsWithSettings);

        var imageSize = GetImageSizeToFitTags(tagsWithSettings);
        Bitmap image;

        try
        {
            image = new Bitmap(imageSize.Width, imageSize.Height);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Unable to create image of size {imageSize}.", ex);
        }

        FillBackground(image, colorConfig.BackgroundColor);
        DrawTags(image, tagsWithSettings);

        return image;
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
