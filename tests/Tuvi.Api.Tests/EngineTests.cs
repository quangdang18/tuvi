using Microsoft.Extensions.Caching.Memory;
using Tuvi.Api.Services;
using Xunit;

namespace Tuvi.Api.Tests;

public class ZodiacServiceTests
{
    private readonly ZodiacService _sut = new();

    [Theory]
    [InlineData(2001, 8, 1, "leo")]
    // Ranh giới các cung (ngày chuyển cung).
    [InlineData(2024, 3, 20, "pisces")]
    [InlineData(2024, 3, 21, "aries")]
    [InlineData(2024, 12, 21, "sagittarius")]
    [InlineData(2024, 12, 22, "capricorn")]
    [InlineData(2024, 1, 19, "capricorn")]
    [InlineData(2024, 1, 20, "aquarius")]
    [InlineData(2024, 2, 18, "aquarius")]
    [InlineData(2024, 2, 19, "pisces")]
    public void GetByDate_returns_correct_sign_at_boundaries(int y, int m, int d, string expected)
        => Assert.Equal(expected, _sut.GetByDate(new DateOnly(y, m, d)).Key);

    [Fact]
    public void GetByDate_covers_all_12_signs_across_the_year()
    {
        var keys = new HashSet<string>();
        var date = new DateOnly(2024, 1, 1);
        for (int i = 0; i < 366; i++, date = date.AddDays(1))
            keys.Add(_sut.GetByDate(date).Key);
        Assert.Equal(12, keys.Count);
    }

    [Fact]
    public void Get_is_case_insensitive_and_null_safe()
    {
        Assert.Equal("leo", _sut.Get("LEO")!.Key);
        Assert.Null(_sut.Get("khong-ton-tai"));
    }
}

public class NumerologyServiceTests
{
    private readonly NumerologyService _sut = new();

    [Fact]
    public void LifePath_reduces_to_single_digit()
        => Assert.Equal(3, _sut.LifePath(new DateOnly(2001, 8, 1)).Number); // 3+8+1 = 12 -> 3

    [Fact]
    public void LifePath_preserves_master_number_11()
        => Assert.Equal(11, _sut.LifePath(new DateOnly(2000, 8, 1)).Number); // 2+8+1 = 11

    [Fact]
    public void LifePath_has_meaning_text()
        => Assert.False(string.IsNullOrWhiteSpace(_sut.LifePath(new DateOnly(2001, 8, 1)).Title));
}

public class StableHashTests
{
    [Fact]
    public void Compute_is_deterministic()
        => Assert.Equal(StableHash.Compute("leo|20260703|love"), StableHash.Compute("leo|20260703|love"));

    [Fact]
    public void Compute_is_non_negative()
        => Assert.True(StableHash.Compute("bất kỳ chuỗi nào") >= 0);

    [Fact]
    public void Pick_stays_in_range()
    {
        for (int i = 0; i < 100; i++)
        {
            int idx = StableHash.Pick(6, "k", i.ToString());
            Assert.InRange(idx, 0, 5);
        }
    }
}

public class HoroscopeServiceTests
{
    private static HoroscopeService NewSut() =>
        new(new ZodiacService(), new MemoryCache(new MemoryCacheOptions()));

    [Fact]
    public void GetDaily_is_deterministic_for_same_sign_and_date()
    {
        var sut = NewSut();
        var date = new DateOnly(2026, 7, 3);
        var a = sut.GetDaily("leo", date)!;
        var b = sut.GetDaily("leo", date)!;
        Assert.Equal(a.Overall, b.Overall);
        Assert.Equal(a.Score, b.Score);
        Assert.Equal(a.LuckyNumber, b.LuckyNumber);
    }

    [Fact]
    public void GetDaily_score_in_valid_range_and_lucky_number_two_digits()
    {
        var sut = NewSut();
        var r = sut.GetDaily("scorpio", new DateOnly(2026, 7, 3))!;
        Assert.InRange(r.Score, 1, 5);
        Assert.InRange(r.LuckyNumber, 0, 99);
    }

    [Fact]
    public void GetDaily_returns_null_for_unknown_sign()
        => Assert.Null(NewSut().GetDaily("khong-ton-tai", new DateOnly(2026, 7, 3)));
}
