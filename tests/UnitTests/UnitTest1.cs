namespace UnitTests;

using FluentAssertions;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        const string expected = "the dev was NOT too lazy to write tests";
        const string actual = "the dev was too lazy to write tests";

        actual.Should().NotBe(expected);
    }
}