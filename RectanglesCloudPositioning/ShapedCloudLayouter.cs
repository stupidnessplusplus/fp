using RectanglesCloudPositioning.Configs;
using System.Drawing;

namespace RectanglesCloudPositioning;

public class ShapedCloudLayouter : ICloudLayouter
{
    private readonly List<Rectangle> rectangles = [];

    private readonly Func<double, double> radiusEquation;
    private readonly Point center;
    private readonly int raysCount;

    private IEnumerator<Point> pointsEnumerator;
    private int radius;

    public ShapedCloudLayouter(IRectanglesPositioningConfig config)
        : this(config.Center, config.RaysCount, config.RadiusEquation)
    {
    }

    public ShapedCloudLayouter(Point center, int raysCount, Func<double, double> radiusEquation)
    {
        ArgumentNullException.ThrowIfNull(radiusEquation);

        this.center = center;
        this.raysCount = raysCount;
        this.radiusEquation = radiusEquation;

        pointsEnumerator = new[] { center }.AsEnumerable().GetEnumerator();
    }

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rectangleSize.Width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rectangleSize.Height);

        var rectangle = GetRectangleToPut(rectangleSize);
        rectangles.Add(rectangle);
        return rectangle;
    }

    private Rectangle GetRectangleToPut(Size rectangleSize)
    {
        while (pointsEnumerator.MoveNext())
        {
            var point = pointsEnumerator.Current - rectangleSize / 2;
            var rectangle = new Rectangle(point, rectangleSize);

            if (CanPut(rectangle))
            {
                return rectangle;
            }
        }

        radius++;
        pointsEnumerator = GetPoints().GetEnumerator();
        return GetRectangleToPut(rectangleSize);
    }

    private bool CanPut(Rectangle rectangle)
    {
        return rectangles
            .All(otherRectangle => !otherRectangle.IntersectsWith(rectangle));
    }

    private IEnumerable<Point> GetPoints()
    {
        if (radius == 0)
        {
            yield return center;
            yield break;
        }

        var step = 2 * Math.PI / raysCount;

        for (var angle = .0; angle < 2 * Math.PI; angle += step)
        {
            var shapeRadius = radius * radiusEquation(angle);
            yield return GetPoint(shapeRadius, angle);
        }
    }

    private Point GetPoint(double radius, double angle)
    {
        var x = (int)(radius * Math.Cos(angle));
        var y = (int)(radius * Math.Sin(angle));
        return new Point(x + center.X, y + center.Y);
    }
}
