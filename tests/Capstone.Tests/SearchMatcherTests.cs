using SharpQuest.Contracts;

namespace Capstone.Tests;

public class SearchMatcherTests
{
    private readonly SearchMatcher _matcher = SearchMatcher.Create();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void R1_empty_or_whitespace_query_matches_any_non_null_text(string query)
    {
        Assert.True(_matcher.Matches("Healing Potion", query));
    }

    [Theory]
    [InlineData("HEALING")]
    [InlineData("healing")]
    [InlineData("HeAlInG")]
    public void R2_matching_ignores_case(string query)
    {
        Assert.True(_matcher.Matches("Healing Potion", query));
    }

    [Theory]
    [InlineData("Café au lait", "cafe")]
    [InlineData("Cafe au lait", "café")]
    [InlineData("Șoseaua Kiseleff", "sosea")]
    public void R3_matching_ignores_diacritics_in_both_directions(string text, string query)
    {
        Assert.True(_matcher.Matches(text, query));
    }

    [Fact]
    public void R4_every_query_word_must_occur()
    {
        Assert.True(_matcher.Matches("Fire Resistance Potion", "fire potion"));
        Assert.False(_matcher.Matches("Fire Resistance Potion", "fire armor"));
    }

    [Fact]
    public void R5_query_word_order_does_not_matter()
    {
        Assert.True(_matcher.Matches("Fire Lizard Scale", "lizard fire"));
    }

    [Fact]
    public void R6_a_query_word_can_match_inside_a_longer_word()
    {
        Assert.True(_matcher.Matches("Healing Potion", "tion"));
    }

    [Fact]
    public void R7_null_text_never_matches()
    {
        Assert.False(_matcher.Matches(null, "potion"));
        Assert.False(_matcher.Matches(null, ""));
    }

    [Fact]
    public void R8_null_query_throws_argument_null_exception()
    {
        Assert.Throws<ArgumentNullException>(() => _matcher.Matches("Healing Potion", null!));
    }
}