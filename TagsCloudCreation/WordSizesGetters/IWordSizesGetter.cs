namespace TagsCloudCreation.WordSizesGetters;

public interface IWordSizesGetter
{
    public UnplacedTag[] GetSizes(IList<string> words);
}
