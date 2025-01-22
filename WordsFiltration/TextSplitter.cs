using System.Text.RegularExpressions;
using FluentResults;
using Microsoft.Extensions.Logging;
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

    public Result<string[]> SplitToWords(string text)
    {
        if (text == null)
        {
            return Result
                .Fail("Text is null.")
                .Log(nameof(TextSplitter), null, LogLevel.Error);
        }

        text = text.ToLower();

        var wordsResult = wordSplitRegex
            .Split(text)
            .Where(word => !string.IsNullOrEmpty(word) && !word.All(ch => ch == '-'))
            .ToResult()
            .Log(nameof(TextSplitter), "Text was split into words.", LogLevel.Information);

        foreach (var wordsSelector in wordsSelectors)
        {
            var wordsSelectorName = wordsSelector.GetType().Name;
            wordsResult = wordsSelector
                .Select(wordsResult.Value)
                .LogIfSuccess(wordsSelectorName, null, LogLevel.Information)
                .LogIfFailed(wordsSelectorName, null, LogLevel.Error);

            if (wordsResult.IsFailed)
            {
                return Result.Fail("Unable to split text to words.");
            }
        }

        return Result.Ok(wordsResult.Value.ToArray());
    }
}
