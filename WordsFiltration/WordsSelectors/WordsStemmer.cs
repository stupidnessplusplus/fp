using FluentResults;
using Microsoft.Extensions.Logging;
using MystemSharp;

namespace WordsFiltration.WordsSelectors;

public class WordsStemmer : IWordsSelector
{
    public Result<IEnumerable<string>> Select(IEnumerable<string> words)
    {
        if (words == null)
        {
            return Result
                .Fail("Words collection is null.");
        }

        return words
            .Select(word => Result
                .Try(
                    () => new Analyses(word)[0].Text,
                    ex => new Error($"Unable to stem word '{word}'. {ex.Message}"))
                .LogIfFailed(LogLevel.Warning))
            .Where(stemResult => stemResult.IsSuccess)
            .Select(stemResult => stemResult.Value)
            .ToResult()
            .WithSuccess($"Words were stemmed.");
    }
}
