using FluentAssertions;
using MystemSharp;
using WordsFiltration.WordsSelectors;

namespace WordsFiltration_Tests.WordsSelectors;

internal abstract class WordsSelectorTests
{
    protected IWordsSelector wordSelector = null!;

    [Test]
    public void Select_Fails_WhenWordsEnumerableIsNull()
    {
        var actualWords = wordSelector.Select(null!);
        actualWords.IsSuccess.Should().BeFalse();
    }
}
