using FakeItEasy;
using FluentAssertions;
using WordsFiltration.Configs;
using WordsFiltration.WordsSelectors;

namespace WordsFiltration_Tests.WordsSelectors;

internal class WordsFilterTests : WordsSelectorTests
{
    [SetUp]
    public void SetUp()
    {
        var wordsSelectionConfig = A.Fake<IWordsSelectionConfig>();

        A.CallTo(() => wordsSelectionConfig.ExcludedWords)
            .Returns(["a"]);

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

        actualWords.Should().BeEquivalentTo(expectedWords, options => options.WithStrictOrdering());
    }
}
