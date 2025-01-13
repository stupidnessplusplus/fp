using WordsFiltration.Configs;

namespace WordsFiltration.WordsSelectors;

public class WordsFilter : IWordsSelector
{
    private readonly IWordsSelectionConfig wordsSelectionConfig;

    public WordsFilter(IWordsSelectionConfig wordsSelectionConfig)
    {
        ArgumentNullException.ThrowIfNull(wordsSelectionConfig);

        this.wordsSelectionConfig = wordsSelectionConfig;
    }

    public IEnumerable<string> Select(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        var excludedWords = wordsSelectionConfig.ExcludedWords?
            .Select(word => word.ToLower())
            .ToHashSet();

        if (excludedWords == null)
        {
            return words;
        }

        return words.Where(word => !excludedWords.Contains(word));
    }
}
