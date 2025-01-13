namespace WordsFiltration.Configs;

public interface IWordsSelectionConfig
{
    public string[]? ExcludedWords { get; }

    public PartOfSpeech[]? IncludedPartsOfSpeech { get; }
}
