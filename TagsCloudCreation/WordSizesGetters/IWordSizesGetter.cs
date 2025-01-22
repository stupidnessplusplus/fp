using FluentResults;

namespace TagsCloudCreation.WordSizesGetters;

public interface IWordSizesGetter
{
    public Result<UnplacedTag[]> GetSizes(IList<string> words);
}
