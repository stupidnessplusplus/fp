using MystemSharp;

namespace WordsFiltration.WordsSelectors;

public class WordsStemmer : IWordsSelector
{
    public IEnumerable<string> Select(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        return words.Select(word => new Analyses(word)[0].Text);
    }
}
