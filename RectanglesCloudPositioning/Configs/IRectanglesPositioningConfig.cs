using System.Drawing;

namespace RectanglesCloudPositioning.Configs;

public interface IRectanglesPositioningConfig
{
    public Point Center { get; }

    public int RaysCount { get; }

    /// <summary>
    /// Уравнение фигуры в полярных координатах radius = f(angle).
    /// </summary>
    public Func<double, double> RadiusEquation { get; }
}
