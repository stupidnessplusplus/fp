using MystemSharp;
using WordsFiltration.Configs;

namespace WordsFiltration.WordsSelectors;

public class PartsOfSpeechFilter : IWordsSelector
{
    private readonly IWordsSelectionConfig wordsSelectionConfig;

    public PartsOfSpeechFilter(IWordsSelectionConfig wordsSelectionConfig)
    {
        ArgumentNullException.ThrowIfNull(wordsSelectionConfig);

        this.wordsSelectionConfig = wordsSelectionConfig;
    }

    public IEnumerable<string> Select(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        var includedPartsOfSpeech = wordsSelectionConfig.IncludedPartsOfSpeech?.ToHashSet();

        if (includedPartsOfSpeech == null)
        {
            return words;
        }

        return words.Where(word => includedPartsOfSpeech.Contains(GetPartOfSpeech(word)));
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
