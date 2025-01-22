namespace WordsFiltration.Configs;

public record WordsSelectionConfig(string[]? ExcludedWords, PartOfSpeech[]? IncludedPartsOfSpeech);
