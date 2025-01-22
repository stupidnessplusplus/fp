using System.Drawing.Imaging;
using FluentResults;

namespace TagsCloudConsoleInterface.Configs;

public record IOConfig(string InputPath, string OutputPath, ImageFormat ImageFormat);
