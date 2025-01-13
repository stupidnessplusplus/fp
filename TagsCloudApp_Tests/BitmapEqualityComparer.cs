using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace TagsCloudApp_Tests;

internal class BitmapEqualityComparer : IEqualityComparer<Bitmap>
{
    public bool Equals(Bitmap? bitmap1, Bitmap? bitmap2)
    {
        if (bitmap1 == bitmap2)
        {
            return true;
        }

        if (bitmap1 == null || bitmap2 == null || bitmap1.Size != bitmap2.Size || bitmap1.PixelFormat != bitmap2.PixelFormat)
        {
            return false;
        }

        for (var x = 0; x < bitmap1.Width; x++)
        {
            for (var y = 0; y < bitmap1.Height; y++)
            {
                if (bitmap1.GetPixel(x, y) != bitmap2.GetPixel(x, y))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public int GetHashCode([DisallowNull] Bitmap obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return obj.GetHashCode();
    }
}
