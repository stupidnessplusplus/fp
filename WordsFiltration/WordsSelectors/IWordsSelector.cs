using FluentResults;

namespace WordsFiltration.WordsSelectors;

public interface IWordsSelector
{
    public Result<IEnumerable<string>> Select(IEnumerable<string> words);
}
