using FluentResults;
using Microsoft.Extensions.Logging;
using MystemSharp;
using WordsFiltration.Configs;

namespace WordsFiltration.WordsSelectors;

public class PartsOfSpeechFilter : IWordsSelector
{
    private readonly WordsSelectionConfig wordsSelectionConfig;

    public PartsOfSpeechFilter(WordsSelectionConfig wordsSelectionConfig)
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

        var includedPartsOfSpeech = wordsSelectionConfig.IncludedPartsOfSpeech?.ToHashSet();

        if (includedPartsOfSpeech == null)
        {
            return words
                .ToResult()
                .WithSuccess("Continuing without filtering words by parts of speech.");
        }

        return words
            .Select(word => (
                Word: word,
                POS: Result
                    .Try(() => GetPartOfSpeech(word))
                    .LogIfFailed(
                        nameof(PartsOfSpeechFilter),
                        $"Unable to determine the part of speech of the word '{word}'.",
                        LogLevel.Warning)))
            .Where(x => x.POS.IsSuccess && includedPartsOfSpeech.Contains(x.POS.Value))
            .Select(x => x.Word)
            .ToResult()
            .WithSuccess($"Words were filtered by parts of speech: [{string.Join(", ", includedPartsOfSpeech)}].");
    }

    private PartOfSpeech GetPartOfSpeech(string word)
    {
        var grammarList = new Analyses(word)[0].StemGram;

        if (grammarList.Contains(Grammar.Abbreviation))
        {
            return PartOfSpeech.Unknown;
        }

        return grammarList
            .Select(ToPartOfSpeech)
            .Where(pos => pos != PartOfSpeech.Unknown)
            .SingleOrDefault(PartOfSpeech.Unknown);
    }

    private PartOfSpeech ToPartOfSpeech(Grammar grammar)
    {
        return grammar switch
        {
            Grammar.Adjective => PartOfSpeech.A,
            Grammar.Adverb => PartOfSpeech.ADV,
            Grammar.AdvPronoun => PartOfSpeech.ADVPRO,
            Grammar.AdjNumeral => PartOfSpeech.ANUM,
            Grammar.AdjPronoun => PartOfSpeech.APRO,
            Grammar.Composite => PartOfSpeech.COM,
            Grammar.Conjunction => PartOfSpeech.CONJ,
            Grammar.Interjunction => PartOfSpeech.INTJ,
            Grammar.Numeral => PartOfSpeech.NUM,
            Grammar.Particle => PartOfSpeech.PART,
            Grammar.Preposition => PartOfSpeech.PR,
            Grammar.Substantive => PartOfSpeech.S,
            Grammar.SubstPronoun => PartOfSpeech.SPRO,
            Grammar.Verb => PartOfSpeech.V,
            _ => PartOfSpeech.Unknown,
        };
    }
}
