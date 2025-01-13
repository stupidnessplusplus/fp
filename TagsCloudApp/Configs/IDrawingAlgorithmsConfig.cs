using RectanglesCloudPositioning;
using TagsCloudCreation.TagsDrawingDecorators;
using TagsCloudCreation.WordSizesGetters;

namespace TagsCloudApp.Configs;

public interface IDrawingAlgorithmsConfig
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

    private static readonly Dictionary<DrawingSetting, Type> drawingSettingsSetterTypes = new()
    {
        { DrawingSetting.Gradient, typeof(GradientTagsDecorator) },
    };

    public WordSizingMethod WordSizingMethod { get; }

    public RectanglesLayouter RectanglesLayouter { get; }

    public DrawingSetting[] DrawingSettings { get; }

    internal Type WordSizesGetterType
    {
        get
        {
            if (!sizingMethodTypes.TryGetValue(WordSizingMethod, out var type))
            {
                throw new ArgumentException($"Unknown word sizing method type: '{WordSizingMethod}'.");
            }

            return type;
        }
    }

    internal Type RectanglesLayouterType
    {
        get
        {
            if (!rectanglesLayoutersTypes.TryGetValue(RectanglesLayouter, out var type))
            {
                throw new ArgumentException($"Unknown rectangle layouter type: '{RectanglesLayouter}'.");
            }

            return type;
        }
    }

    internal Type[] AdditionalSettingsSetterTypes
    {
        get
        {
            var result = new Type[DrawingSettings.Length];

            for (var i = 0; i < DrawingSettings.Length; i++)
            {
                if (!drawingSettingsSetterTypes.TryGetValue(DrawingSettings[i], out var type))
                {
                    throw new ArgumentException($"Unknown setting: '{DrawingSettings[i]}'.");
                }

                result[i] = type;
            }

            return result;
        }
    }
}
