namespace WordsFiltration.WordsSelectors;

public interface IWordsSelector
{
    public IEnumerable<string> Select(IEnumerable<string> words);
}
