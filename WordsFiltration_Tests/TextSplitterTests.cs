using FakeItEasy;
using FluentAssertions;
using FluentResults;
using WordsFiltration;
using WordsFiltration.WordsSelectors;

namespace WordsFiltration_Tests;

internal class TextSplitterTests
{
    private TextSplitter textSplitter;

    [SetUp]
    public void Setup()
    {
        textSplitter = new TextSplitter([]);
    }

    [Test]
    public void Constructor_ThrowsException_WhenWordsSelctorsEnumerableIsNull()
    {
        var ctor = () => new TextSplitter(null!);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void SplitToWords_Fails_WhenTextIsNull()
    {
        var actualWords = textSplitter.SplitToWords(null!);
        actualWords.IsSuccess.Should().BeFalse();
    }

    [TestCaseSource(nameof(GetTextWithWhiteSpaceAndPunctuationTestCases))]
    public void SplitToWords_SplitsTextByWhiteSpaceAndPunctuation(string text, string[] expectedWords)
    {
        var actualWords = textSplitter.SplitToWords(text);

        actualWords.IsSuccess.Should().BeTrue();
        actualWords.Value.Should().BeEquivalentTo(expectedWords);
    }

    [Test]
    public void SplitToWords_DoesNotSplitByDash_WhenDashIsPartOfWord()
    {
        var text = "- a-b";
        var expectedWords = new[] { "a-b" };

        var actualWords = textSplitter.SplitToWords(text);

        actualWords.IsSuccess.Should().BeTrue();
        actualWords.Value.Should().BeEquivalentTo(expectedWords);
    }

    [Test]
    public void SplitToWords_AppliesWordsSelectors()
    {
        var text = "a b c";
        var expectedWords = new[] { "a12", "b12", "c12" };

        var wordSelector1 = A.Fake<IWordsSelector>();
        var wordSelector2 = A.Fake<IWordsSelector>();
        var textSplitter = new TextSplitter([wordSelector1, wordSelector2]);

        A.CallTo(() => wordSelector1.Select(null!))
            .WithAnyArguments()
            .ReturnsLazily(obj => ((IEnumerable<string>)obj.Arguments[0]!).Select(word => word + "1").ToResult());
        A.CallTo(() => wordSelector2.Select(null!))
            .WithAnyArguments()
            .ReturnsLazily(obj => ((IEnumerable<string>)obj.Arguments[0]!).Select(word => word + "2").ToResult());

        var actualWords = textSplitter.SplitToWords(text);

        actualWords.IsSuccess.Should().BeTrue();
        actualWords.Value.Should().BeEquivalentTo(expectedWords, options => options.WithStrictOrdering());
        A.CallTo(() => wordSelector1.Select(null!))
            .WithAnyArguments()
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => wordSelector2.Select(null!))
            .WithAnyArguments()
            .MustHaveHappenedOnceExactly();
    }

    private static IEnumerable<TestCaseData> GetTextWithWhiteSpaceAndPunctuationTestCases()
    {
        yield return new TestCaseData("", new string[0]);
        yield return new TestCaseData("  ", new string[0]);
        yield return new TestCaseData("!\"#%&'()*,-./:;?@[\\]_{}§«·»", new string[0]);
        yield return new TestCaseData("--", new string[0]);
        yield return new TestCaseData("a", new[] { "a" });
        yield return new TestCaseData("a ", new[] { "a" });
        yield return new TestCaseData(" a", new[] { "a" });
        yield return new TestCaseData(",a", new[] { "a" });
        yield return new TestCaseData("a,", new[] { "a" });
        yield return new TestCaseData("a b", new[] { "a", "b" });
        yield return new TestCaseData("a,b", new[] { "a", "b" });
    }
}