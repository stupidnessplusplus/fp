using System.Drawing.Imaging;

namespace TagsCloudConsoleInterface;

public interface IIOConfig
{
    public string InputPath { get; }

    public string OutputPath { get; }

    public OutputImageFormat OutputFormat { get; }

    internal ImageFormat ImageFormat
    {
        get
        {
            return OutputFormat switch
            {
                OutputImageFormat.Png => ImageFormat.Png,
                OutputImageFormat.Jpeg => ImageFormat.Jpeg,
                OutputImageFormat.Bmp => ImageFormat.Bmp,
                _ => throw new ArgumentException("Unsupported image format.")
            };
        }
    }
}
