﻿using FluentAssertions;
using WordsFiltration;
using WordsFiltration.Configs;
using WordsFiltration.WordsSelectors;

namespace WordsFiltration_Tests.WordsSelectors;

internal class PartsOfSpeechFilterTests : WordsSelectorTests
{
    [SetUp]
    public void SetUp()
    {
        var wordsSelectionConfig = new WordsSelectionConfig(null, [PartOfSpeech.A, PartOfSpeech.S, PartOfSpeech.V]);
        wordSelector = new PartsOfSpeechFilter(wordsSelectionConfig);
    }

    [Test]
    public void Constructor_ThrowsException_WhenWordsSelectionConfigIsNull()
    {
        var ctor = () => new PartsOfSpeechFilter(null!);
        ctor.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Select_ReturnsWordsOfIncludedPartsOfSpeech()
    {
        var words = new[] { "ты", "лодка", "копать", "кто", "громко", "два", "песочный", "123", "абвгде", "abcde" };
        var expectedWords = new[] { "лодка", "копать", "песочный" };

        var actualWords = wordSelector.Select(words);

        actualWords.IsSuccess.Should().BeTrue();
        actualWords.Value.Should().BeEquivalentTo(expectedWords, options => options.WithStrictOrdering());
    }
}
