using Autofac;
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

    public void Run(
        IDrawingAlgorithmsConfig algorithmsConfig,
        IWordsSelectionConfig wordsSelectionConfig,
        IWordSizesGetterConfig wordSizesGetterConfig,
        IRectanglesPositioningConfig rectanglesPositioningConfig,
        ITagsColorConfig colorConfig,
        ITagsFontConfig fontConfig)
    {
        var builder = new ContainerBuilder();

        builder.RegisterInstance(wordsSelectionConfig).As<IWordsSelectionConfig>().SingleInstance();
        builder.RegisterInstance(wordSizesGetterConfig).As<IWordSizesGetterConfig>().SingleInstance();
        builder.RegisterInstance(rectanglesPositioningConfig).As<IRectanglesPositioningConfig>().SingleInstance();
        builder.RegisterInstance(colorConfig).As<ITagsColorConfig>().SingleInstance();
        builder.RegisterInstance(fontConfig).As<ITagsFontConfig>().SingleInstance();

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

        Run(textSplitter, tagsCloudCreator);
    }

    private void Run(
        TextSplitter textSplitter,
        TagsCloudCreator tagsCloudCreator)
    {
        var text = readText();
        var words = textSplitter.SplitToWords(text);
        var image = tagsCloudCreator.DrawTagsCloud(words);
        writeImage(image);
    }

    private void RegisterDrawingAlgorithms(
        ContainerBuilder builder,
        IDrawingAlgorithmsConfig config)
    {
        builder.RegisterType(config.RectanglesLayouterType).As<ICloudLayouter>().SingleInstance();
        builder.RegisterType(config.WordSizesGetterType).As<IWordSizesGetter>().SingleInstance();

        builder.RegisterType<SingleSolidColorTagsDecorator>().As<ITagsDrawingDecorator>().SingleInstance();
        builder.RegisterType<SingleFontTagsDecorator>().As<ITagsDrawingDecorator>().SingleInstance();

        foreach (var type in config.AdditionalSettingsSetterTypes)
        {
            builder.RegisterType(type).As<ITagsDrawingDecorator>().SingleInstance();
        }
    }
}
