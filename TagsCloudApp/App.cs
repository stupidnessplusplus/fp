using Autofac;
using FluentResults;
using RectanglesCloudPositioning;
using RectanglesCloudPositioning.Configs;
using System.Drawing;
using TagsCloudApp.Configs;
using TagsCloudCreation;
using TagsCloudCreation.Configs;
using TagsCloudCreation.TagsDrawers;
using TagsCloudCreation.TagsDrawingDecorators;
using TagsCloudCreation.WordSizesGetters;
using WordsFiltration;
using WordsFiltration.Configs;
using WordsFiltration.WordsSelectors;

namespace TagsCloudApp;

public class App
{
    private readonly Func<string> readText;
    private readonly Action<Bitmap> writeImage;

    public App(Func<string> readText, Action<Bitmap> writeImage)
    {
        this.readText = readText;
        this.writeImage = writeImage;
    }

    public Result Run(
        DrawingAlgorithmsConfig algorithmsConfig,
        WordsSelectionConfig wordsSelectionConfig,
        WordSizesGetterConfig wordSizesGetterConfig,
        RectanglesPositioningConfig rectanglesPositioningConfig,
        TagsColorConfig colorConfig,
        TagsFontConfig fontConfig)
    {
        var builder = new ContainerBuilder();

        builder.RegisterInstance(wordsSelectionConfig).As<WordsSelectionConfig>().SingleInstance();
        builder.RegisterInstance(wordSizesGetterConfig).As<WordSizesGetterConfig>().SingleInstance();
        builder.RegisterInstance(rectanglesPositioningConfig).As<RectanglesPositioningConfig>().SingleInstance();
        builder.RegisterInstance(colorConfig).As<TagsColorConfig>().SingleInstance();
        builder.RegisterInstance(fontConfig).As<TagsFontConfig>().SingleInstance();

        builder.RegisterType<WordsStemmer>().As<IWordsSelector>().SingleInstance();
        builder.RegisterType<PartsOfSpeechFilter>().As<IWordsSelector>().SingleInstance();
        builder.RegisterType<WordsFilter>().As<IWordsSelector>().SingleInstance();
        builder.RegisterType<TextSplitter>().AsSelf().SingleInstance();

        RegisterDrawingAlgorithms(builder, algorithmsConfig);
        builder.RegisterType<TagsDrawer>().As<ITagsDrawer>().SingleInstance();
        builder.RegisterType<TagsCloudCreator>().AsSelf().SingleInstance();

        var container = builder.Build();
        var tagsCloudCreator = container.Resolve<TagsCloudCreator>();
        var textSplitter = container.Resolve<TextSplitter>();

        return Run(textSplitter, tagsCloudCreator);
    }

    private Result Run(
        TextSplitter textSplitter,
        TagsCloudCreator tagsCloudCreator)
    {
        return Result
            .Try(readText)
            .Bind(textSplitter.SplitToWords)
            .Bind(tagsCloudCreator.DrawTagsCloud)
            .Bind(image => Result
                .Try(() => writeImage(image)));
    }

    private void RegisterDrawingAlgorithms(
        ContainerBuilder builder,
        DrawingAlgorithmsConfig config)
    {
        builder.RegisterType(config.RectanglesLayouterType).As<ICloudLayouter>().SingleInstance();
        builder.RegisterType(config.WordSizesGetterType).As<IWordSizesGetter>().SingleInstance();

        builder.RegisterType<SingleSolidColorTagsDecorator>().As<ITagsDrawingDecorator>().SingleInstance();
        builder.RegisterType<SingleFontTagsDecorator>().As<ITagsDrawingDecorator>().SingleInstance();

        foreach (var type in config.TagsDecoratorTypes)
        {
            builder.RegisterType(type).As<ITagsDrawingDecorator>().SingleInstance();
        }
    }
}
