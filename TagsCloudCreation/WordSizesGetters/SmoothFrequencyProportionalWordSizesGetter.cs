using FluentResults;
using TagsCloudCreation.Configs;

namespace TagsCloudCreation.WordSizesGetters;

public class SmoothFrequencyProportionalWordSizesGetter : FrequencyProportionalWordSizesGetter
{
    public SmoothFrequencyProportionalWordSizesGetter(
        WordSizesGetterConfig wordSizesGetterConfig,
        TagsFontConfig tagsFontConfig)
        : base(wordSizesGetterConfig, tagsFontConfig)
    {
    }

    public override Result<UnplacedTag[]> GetSizes(IList<string> words)
    {
        if (words == null)
        {
            return Result.Fail("Words collection is null.");
        }

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
