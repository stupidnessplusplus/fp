using System.Drawing;
using TagsCloudCreation.Configs;

namespace TagsCloudCreation.WordSizesGetters;

public class FrequencyProportionalWordSizesGetter : IWordSizesGetter, IDisposable
{
    private readonly Image emptyImage = new Bitmap(1, 1);

    private readonly IWordSizesGetterConfig wordSizesGetterConfig;
    private readonly ITagsFontConfig tagsFontConfig;

    public FrequencyProportionalWordSizesGetter(IWordSizesGetterConfig wordSizesGetterConfig, ITagsFontConfig tagsFontConfig)
    {
        ArgumentNullException.ThrowIfNull(wordSizesGetterConfig);
        ArgumentNullException.ThrowIfNull(tagsFontConfig);

        this.wordSizesGetterConfig = wordSizesGetterConfig;
        this.tagsFontConfig = tagsFontConfig;
    }

    public void Dispose()
    {
        emptyImage.Dispose();
    }

    public virtual UnplacedTag[] GetSizes(IList<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        return words
            .GroupBy(word => word)
            .Select(group => (Word: group.Key, Frequency: group.Count()))
            .OrderByDescending(x => x.Frequency)
            .Select(x => GetSize(x.Word, x.Frequency))
            .ToArray();
    }

    protected UnplacedTag GetSize(string word, int wordFrequency)
    {
        var height = (int)(wordSizesGetterConfig.MinSize + wordSizesGetterConfig.Scale * (wordFrequency - 1));
        using var wordFont = new Font(tagsFontConfig.FontName, height, tagsFontConfig.FontStyle, GraphicsUnit.Pixel);
        using var graphics = Graphics.FromImage(emptyImage);

        var sizeF = graphics.MeasureString(word, wordFont);
        var size = new Size((int)Math.Ceiling(sizeF.Width), height);
        return new UnplacedTag(word, size);
    }
}
