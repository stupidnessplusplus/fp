using System.Drawing;

namespace RectanglesCloudPositioning;

public interface ICloudLayouter
{
    public Rectangle PutNextRectangle(Size rectangleSize);
}
