namespace WordsFiltration;

public enum PartOfSpeech
{
    /// <summary>
    /// Часть речи не определена.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Adjective, прилагательное.
    /// </summary>
    A = 1,

    /// <summary>
    /// Adverb, наречие.
    /// </summary>
    ADV = 2,

    /// <summary>
    /// Рronominal adverb, местоименное наречие.
    /// </summary>
    ADVPRO = 3,

    /// <summary>
    /// Numeral-adjective, числительное-прилагательное.
    /// </summary>
    ANUM = 4,

    /// <summary>
    /// Pronoun-adjective, местоимение-прилагательное.
    /// </summary>
    APRO = 5,

    /// <summary>
    /// Composite part, часть композита - сложного слова.
    /// </summary>
    COM = 6,

    /// <summary>
    /// Conjunction, союз.
    /// </summary>
    CONJ = 7,

    /// <summary>
    /// Interjection, междометие.
    /// </summary>
    INTJ = 8,

    /// <summary>
    /// Numeral, числительное.
    /// </summary>
    NUM = 9,

    /// <summary>
    /// Particle, частица.
    /// </summary>
    PART = 10,

    /// <summary>
    /// Pretext, предлог.
    /// </summary>
    PR = 11,

    /// <summary>
    /// Noun (substantive), существительное.
    /// </summary>
    S = 12,

    /// <summary>
    /// Pronoun-noun, местоимение-существительное.
    /// </summary>
    SPRO = 13,

    /// <summary>
    /// Verb, глагол.
    /// </summary>
    V = 14,
}
