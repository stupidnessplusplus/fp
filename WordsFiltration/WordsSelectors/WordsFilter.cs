using FluentResults;
using WordsFiltration.Configs;

namespace WordsFiltration.WordsSelectors;

public class WordsFilter : IWordsSelector
{
    private readonly WordsSelectionConfig wordsSelectionConfig;

    public WordsFilter(WordsSelectionConfig wordsSelectionConfig)
    {
        ArgumentNullException.ThrowIfNull(wordsSelectionConfig);

        this.wordsSelectionConfig = wordsSelectionConfig;
    }

    public Result<IEnumerable<string>> Select(IEnumerable<string> words)
    {
        if (words == null)
        {
            return Result
                .Fail("Words collection is null.");
        }

        var excludedWords = wordsSelectionConfig.ExcludedWords
            ?.Select(word => word.ToLower())
            .ToHashSet();

        if (excludedWords == null)
        {
            return words
                .ToResult()
                .WithSuccess("Continuing without filtering excluded words.");
        }

        return words
            .Where(word => !excludedWords.Contains(word))
            .ToResult()
            .WithSuccess($"Words were excluded.");
    }
}
