using System.Drawing;

namespace RectanglesCloudPositioning.Configs;

public record RectanglesPositioningConfig(Point Center, int RaysCount, Func<double, double> RadiusEquation);
