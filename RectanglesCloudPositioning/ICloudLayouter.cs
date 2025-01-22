using System.Drawing;
using FluentResults;

namespace RectanglesCloudPositioning;

public interface ICloudLayouter
{
    public Result<Rectangle> PutNextRectangle(Size rectangleSize)
    {
        return Result
            .FailIf(
                rectangleSize.Width <= 0 || rectangleSize.Height <= 0,
                new Error("Width and height must be greater than zero."))
            .Bind(() => Result
                .Try(
                    () => PutNextRectangleOrThrow(rectangleSize),
                    ex => new Error($"Cannot put a rectangle of size {rectangleSize}. {ex.Message}")));
    }

    protected Rectangle PutNextRectangleOrThrow(Size rectangleSize);
}
