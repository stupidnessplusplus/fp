using FluentAssertions;
using WordsFiltration.WordsSelectors;

namespace WordsFiltration_Tests.WordsSelectors;

internal class WordsStemmerTests : WordsSelectorTests
{
    [SetUp]
    public void SetUp()
    {
        wordSelector = new WordsStemmer();
    }

    [TestCaseSource(nameof(GetWordsStemTestCases))]
    public void Select_ReturnsWordStems(string word, string expectedWord)
    {
        var actualWords = wordSelector.Select([word]);

        actualWords.Should().BeEquivalentTo(expectedWord);
    }

    private static IEnumerable<TestCaseData> GetWordsStemTestCases()
    {
        yield return new TestCaseData("цветок", "цветок");
        yield return new TestCaseData("цветы", "цветок");
        yield return new TestCaseData("цветка", "цветок");
        yield return new TestCaseData("цветку", "цветок");
        yield return new TestCaseData("цветов", "цветок");
        yield return new TestCaseData("цветков", "цветок");

        yield return new TestCaseData("желтый", "желтый");
        yield return new TestCaseData("желтая", "желтый");
        yield return new TestCaseData("желтые", "желтый");
        yield return new TestCaseData("желтого", "желтый");
        yield return new TestCaseData("желтому", "желтый");

        yield return new TestCaseData("сиять", "сиять");
        yield return new TestCaseData("сияет", "сиять");
        yield return new TestCaseData("сиял", "сиять");
        yield return new TestCaseData("сияющий", "сиять");
        yield return new TestCaseData("сиявший", "сиять");

        yield return new TestCaseData("я", "я");
        yield return new TestCaseData("меня", "я");
        yield return new TestCaseData("мне", "я");
    }
}
