using FluentResults;
using RectanglesCloudPositioning;
using TagsCloudCreation.TagsDrawingDecorators;
using TagsCloudCreation.WordSizesGetters;

namespace TagsCloudApp.Configs;

public record DrawingAlgorithmsConfig(
    Type WordSizesGetterType,
    Type RectanglesLayouterType,
    Type[] TagsDecoratorTypes)
{
    private static readonly Dictionary<WordSizingMethod, Type> sizingMethodTypes = new()
    {
        { WordSizingMethod.Frequency, typeof(FrequencyProportionalWordSizesGetter) },
        { WordSizingMethod.SmoothFrequency, typeof(SmoothFrequencyProportionalWordSizesGetter) },
    };

    private static readonly Dictionary<RectanglesLayouter, Type> rectanglesLayoutersTypes = new()
    {
        { RectanglesLayouter.Circle, typeof(SpiralCircularCloudLayouter) },
        { RectanglesLayouter.Shaped, typeof(ShapedCloudLayouter) },
    };

    private static readonly Dictionary<DrawingSetting, Type> tagsDecoratorTypes = new()
    {
        { DrawingSetting.Gradient, typeof(GradientTagsDecorator) },
    };

    public static Result<DrawingAlgorithmsConfig> FromEnums(
        WordSizingMethod wordSizingMethod,
        RectanglesLayouter rectanglesLayouter,
        DrawingSetting[] drawingSettings)
    {
        var hasWordSizingMethod = sizingMethodTypes.ContainsKey(wordSizingMethod);
        var hasRectanglesLayouter = rectanglesLayoutersTypes.ContainsKey(rectanglesLayouter);
        var invalidSettings = drawingSettings.Where(setting => !tagsDecoratorTypes.ContainsKey(setting));
        var invalidSettingsString = string.Join(", ", invalidSettings);

        return Result
            .FailIf(!hasWordSizingMethod, new Error($"Unknown word sizing method type: '{wordSizingMethod}'."))
            .Bind(() => Result
                .FailIf(!hasRectanglesLayouter, new Error($"Unknown rectangle layouter type: '{rectanglesLayouter}'.")))
            .Bind(() => Result
                .FailIf(invalidSettings.Any(), new Error($"Unknown rectangle layouter type: '{invalidSettingsString}'.")))
            .Bind(() => Result
                .Ok(new DrawingAlgorithmsConfig(
                    sizingMethodTypes[wordSizingMethod],
                    rectanglesLayoutersTypes[rectanglesLayouter],
                    drawingSettings.Select(setting => tagsDecoratorTypes[setting]).ToArray())));
    }
}
