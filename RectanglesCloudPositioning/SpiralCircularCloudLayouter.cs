using RectanglesCloudPositioning.Configs;
using System.Drawing;

namespace RectanglesCloudPositioning;

public class SpiralCircularCloudLayouter : ICloudLayouter
{
    private readonly SortedRectanglesList rectangles = new();
    private readonly Dictionary<Direction, int> directionIndices = [];

    private readonly Point center;
    private readonly int raysCount;

    private IEnumerator<(Point Point, Direction Direction)> pointsEnumerator;
    private int radius;

    public SpiralCircularCloudLayouter(IRectanglesPositioningConfig config)
        : this(config.Center, config.RaysCount)
    {
    }

    public SpiralCircularCloudLayouter(Point center, int raysCount)
    {
        this.center = center;
        this.raysCount = raysCount;
        pointsEnumerator = Enumerable
            .Empty<(Point, Direction)>()
            .GetEnumerator();
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
            var direction = pointsEnumerator.Current.Direction;
            var point = pointsEnumerator.Current.Point - rectangleSize / 2;
            var rectangle = new Rectangle(point, rectangleSize);

            if (CanPut(rectangle, direction))
            {
                ResetDirectionIndices();
                return rectangle;
            }
        }

        radius++;
        ResetDirectionIndices();
        pointsEnumerator = GetPoints().GetEnumerator();
        return GetRectangleToPut(rectangleSize);
    }

    private bool CanPut(Rectangle rectangle, Direction direction)
    {
        var wideRectangle = direction is Direction.Left or Direction.Right
            ? new Rectangle(new Point(rectangle.X, int.MinValue / 2), new Size(rectangle.Width, int.MaxValue))
            : new Rectangle(new Point(int.MinValue / 2, rectangle.Y), new Size(int.MaxValue, rectangle.Height));
        
        if (rectangles.HasIntersection(wideRectangle, direction, directionIndices[direction], out var intersectionIndex))
        {
            directionIndices[direction] = intersectionIndex;

            return !rectangles.HasIntersection(rectangle, direction, intersectionIndex, out _);
        }
        
        return true;
    }

    private IEnumerable<(Point, Direction)> GetPoints()
    {
        if (radius == 0)
        {
            yield return (center, Direction.None);
            yield break;
        }

        var step = 2 * Math.PI / raysCount;

        for (var angle = Math.PI / 4; angle < 3 * Math.PI / 4; angle += step)
        {
            var x = (int)(radius * Math.Cos(angle));
            var y = (int)(radius * Math.Sin(angle));

            yield return (new Point(x, y), Direction.Left);
            yield return (new Point(-y, x), Direction.Up);
            yield return (new Point(-x, -y), Direction.Right);
            yield return (new Point(y, -x), Direction.Down);
        }
    }

    private void ResetDirectionIndices()
    {
        directionIndices[Direction.Left] = 0;
        directionIndices[Direction.Right] = 0;
        directionIndices[Direction.Up] = 0;
        directionIndices[Direction.Down] = 0;
    }
}
