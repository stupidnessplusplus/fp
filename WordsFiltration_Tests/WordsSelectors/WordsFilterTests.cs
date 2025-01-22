using FluentAssertions;
using WordsFiltration.Configs;
using WordsFiltration.WordsSelectors;

namespace WordsFiltration_Tests.WordsSelectors;

internal class WordsFilterTests : WordsSelectorTests
{
    [SetUp]
    public void SetUp()
    {
        var wordsSelectionConfig = new WordsSelectionConfig(["a"], null);
        wordSelector = new WordsFilter(wordsSelectionConfig);
    }

    [Test]
    public void Constructor_ThrowsException_WhenWordsSelectionConfigIsNull()
    {
        var ctor = () => new WordsFilter(null!);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Select_ExcludesWords()
    {
        var words = new[] { "a", "b", "abc", "aaa" };
        var expectedWords = new[] { "b", "abc", "aaa" };

        var actualWords = wordSelector.Select(words);

        actualWords.IsSuccess.Should().BeTrue();
        actualWords.Value.Should().BeEquivalentTo(expectedWords, options => options.WithStrictOrdering());
    }
}
