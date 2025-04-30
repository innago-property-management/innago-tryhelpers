namespace UnitTests;

using FluentAssertions;

using Innago.Shared.TryHelpers;

using Xunit.OpenCategories;

[UnitTest(nameof(Unit))]
public class UnitTests
{
    [Fact]
    public void AllUnitsShouldBeDefault()
    {
        Unit unit = new();

        unit.Equals(Unit.Default).Should().BeTrue();
    }

    [Fact]
    public void AllUnitsShouldBeEqual()
    {
        Unit unit0 = new();
        Unit unit1 = new();

        unit0.Equals(unit1).Should().BeTrue();

        (unit0 == unit1).Should().BeTrue();
    }

    [Fact]
    public void CompareToShouldReturn0ForAllUnits()
    {
        Unit unit0 = new();
        Unit unit1 = new();

        unit0.CompareTo(unit1).Should().Be(0);
    }

    [Fact]
    public void GetHashCodeShouldReturnZero()
    {
        Unit unit = new();
        unit.GetHashCode().Should().Be(0);
    }

    [Fact]
    public void GreaterThanOrEqualToShouldBeTrue()
    {
        Unit unit0 = new();
        Unit unit1 = new();
        (unit0 >= unit1).Should().BeTrue();
    }

    [Fact]
    public void GreaterThanShouldReturnFalse()
    {
        Unit unit0 = new();
        Unit unit1 = new();
        (unit0 > unit1).Should().BeFalse();
    }

    [Fact]
    public void LessThanOrEqualToShouldBeTrue()
    {
        Unit unit0 = new();
        Unit unit1 = new();
        (unit0 <= unit1).Should().BeTrue();
    }

    [Fact]
    public void LessThanShouldBeFalse()
    {
        Unit unit0 = new();
        Unit unit1 = new();
        (unit0 < unit1).Should().BeFalse();
    }

    [Fact]
    public void ToStringShouldReturnEmptyParentheses()
    {
        Unit unit = new();
        unit.ToString().Should().Be("()");
    }
}