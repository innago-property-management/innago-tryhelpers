namespace UnitTests;

using AutoFixture;

using FluentAssertions;

using Innago.Shared.TryHelpers;

using Xunit.OpenCategories;

[UnitTest(nameof(EitherExtensions))]
public class EitherExtensionsTests
{
    private Fixture Fixture { get; } = new();

    [Fact]
    public void ToEitherLeftShouldChangeTheReturnType()
    {
        var value = this.Fixture.Create<int>();

        Func<int, int> identity = n => n;
        Func<int, Either<int, string>> wrapped = identity.ToEitherLeft<int, int, string>();

        Either<int, string> actual = wrapped(value);

        actual.IsLeft.Should().BeTrue();
        ((int)actual).Should().Be(value);
    }

    [Fact]
    public void ToEitherRightShouldChangeTheReturnType()
    {
        var value = this.Fixture.Create<string>();

        Func<string?, string?> identity = s => s;
        Func<string?, Either<int, string?>> wrapped = identity.ToEitherRight<string, int, string>();

        Either<int, string?> actual = wrapped(value);

        actual.IsRight.Should().BeTrue();
        ((string?)actual).Should().Be(value);
    }
}