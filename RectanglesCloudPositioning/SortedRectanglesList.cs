using System.Drawing;

namespace RectanglesCloudPositioning;

/// <summary>
/// Вспомогательная структура для хранения прямоугольников, отсортированных по координатам сторон,
/// с возможностью получения прямоугольника по индексу в отсортированном списке.
/// </summary>
internal class SortedRectanglesList
{
    private readonly Dictionary<Direction, SortedList<int, Rectangle>> sortedRectangles;

    public SortedRectanglesList()
    {
        var noEqualityComparer = new NoEqualityComparer<int>();

        sortedRectangles = new Dictionary<Direction, SortedList<int, Rectangle>>(4)
        {
            { Direction.Left, new(noEqualityComparer) },
            { Direction.Right, new(noEqualityComparer) },
            { Direction.Up, new(noEqualityComparer) },
            { Direction.Down, new(noEqualityComparer) },
        };
    }

    public int Count { get; private set; }

    public void Add(Rectangle rectangle)
    {
        sortedRectangles[Direction.Left].Add(-rectangle.Right, rectangle);
        sortedRectangles[Direction.Right].Add(rectangle.Left, rectangle);
        sortedRectangles[Direction.Up].Add(-rectangle.Bottom, rectangle);
        sortedRectangles[Direction.Down].Add(rectangle.Top, rectangle);

        Count++;
    }

    public Rectangle Get(Direction sortingDirection, int index)
    {
        if (!sortedRectangles.TryGetValue(sortingDirection, out var rectangles))
        {
            throw new ArgumentException($"Unsupported sorting direction: {sortingDirection}.");
        }

        if (index < 0 || index >= Count)
        {
            throw new IndexOutOfRangeException($"Index was out of range: {index}.");
        }

        return rectangles.Values[index];
    }

    public bool HasIntersection(
        Rectangle rectangle,
        Direction sortingDirection,
        int startIndex,
        out int intersectedRectangleIndex)
    {
        if (!sortedRectangles.TryGetValue(sortingDirection, out var rectangles))
        {
            throw new ArgumentException($"Unsupported sorting direction: {sortingDirection}.");
        }

        if (startIndex < 0)
        {
            throw new IndexOutOfRangeException($"Index was out of range: {startIndex}.");
        }

        for (var i = startIndex; i < rectangles.Count; i++)
        {
            if (rectangle.IntersectsWith(rectangles.Values[i]))
            {
                intersectedRectangleIndex = i;
                return true;
            }
        }

        intersectedRectangleIndex = -1;
        return false;
    }
}
