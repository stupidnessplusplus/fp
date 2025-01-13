using TagsCloudCreation.Configs;

namespace TagsCloudCreation.WordSizesGetters;

public class SmoothFrequencyProportionalWordSizesGetter : FrequencyProportionalWordSizesGetter
{
    public SmoothFrequencyProportionalWordSizesGetter(
        IWordSizesGetterConfig wordSizesGetterConfig,
        ITagsFontConfig tagsFontConfig)
        : base(wordSizesGetterConfig, tagsFontConfig)
    {
    }

    public override UnplacedTag[] GetSizes(IList<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        return words
            .GroupBy(word => word)
            .Select(group => (Word: group.Key, Frequency: group.Count()))
            .GroupBy(x => x.Frequency)
            .OrderBy(group => group.Key)
            .SelectMany((group, i) => group.Select(x => GetSize(x.Word, i + 1)))
            .Reverse()
            .ToArray();
    }
}
