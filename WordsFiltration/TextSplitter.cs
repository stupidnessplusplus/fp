using System.Text.RegularExpressions;
using WordsFiltration.WordsSelectors;

namespace WordsFiltration;

public class TextSplitter
{
    private static readonly Regex wordSplitRegex = new Regex(@"[\p{P}\s-[-]]+");

    private readonly IEnumerable<IWordsSelector> wordsSelectors;

    public TextSplitter(IEnumerable<IWordsSelector> wordsSelectors)
    {
        ArgumentNullException.ThrowIfNull(wordsSelectors);

        this.wordsSelectors = wordsSelectors;
    }

    public string[] SplitToWords(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        text = text.ToLower();

        var words = wordSplitRegex
            .Split(text)
            .Where(word => !string.IsNullOrEmpty(word) && !word.All(ch => ch == '-'));

        return wordsSelectors
            .Aggregate(words, (words, wordsSelector) => wordsSelector.Select(words))
            .ToArray();
    }
}
