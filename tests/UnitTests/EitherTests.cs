namespace UnitTests;

using AutoFixture;

using FluentAssertions;

using Innago.Shared.TryHelpers;

using Moq;

using Xunit.OpenCategories;

[UnitTest(nameof(Either<object, object>))]
public class EitherTTests
{
    public static TheoryData<string?, int?> GetHashCodeTheoryData
    {
        get
        {
            var retVal = new TheoryData<string?, int?>
            {
                { null, null },
                { Guid.NewGuid().ToString(), null },
                { null, new Random().Next() },
            };

            return retVal;
        }
    }

    private Fixture Fixture { get; } = new();

    [Fact]
    public void DefaultEitherShouldBeNeitherLeftNorRight()
    {
        Either<string, int> either = default;
        either.IsLeft.Should().BeFalse();
        either.IsRight.Should().BeFalse();
    }

    [Fact]
    public void EitherFromLeftShouldBeLeft()
    {
        var value = this.Fixture.Create<bool>();
        Either<bool, int> either = value;
        either.IsLeft.Should().BeTrue();

        bool actual = either;
        actual.Should().Be(value);
    }

    [Fact]
    public void EitherFromRightShouldBeRight()
    {
        var value = this.Fixture.Create<int>();
        Either<bool, int> either = value;
        either.IsRight.Should().BeTrue();

        int actual = either;
        actual.Should().Be(value);
    }

    [Fact]
    public void EitherLeftFuncAndEitherLeftValShouldBeEqualIfSameLeftResult()
    {
        var value = this.Fixture.Create<string>();
        Either<string, int> either1 = value;
        var either2 = new Either<string, int>(() => value);

        either1.Equals(either2).Should().BeTrue();
        (either1 == either2).Should().BeTrue();
        (either1.GetHashCode() == either2.GetHashCode()).Should().BeTrue();
    }

    [Fact]
    public void EitherLeftInstancesWithTheSameLeftValueShouldBeEqual()
    {
        var value = this.Fixture.Create<string>();
        Either<string, int> either1 = value;
        Either<string, int> either2 = value;

        either1.Equals(either2).Should().BeTrue();
        (either1 == either2).Should().BeTrue();
        (either1.GetHashCode() == either2.GetHashCode()).Should().BeTrue();
    }

    [Fact]
    public void EitherLeftShouldBeAssignableFromFunc()
    {
        var s = this.Fixture.Create<string>();

        Either<string, int> either = (Func<string>)Fn;

        string? actual = either;

        actual.Should().Be(s);

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        string Fn()
        {
            return s;
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData("a")]
    public void EitherMapCanBeUsedInLieuOfSwitchExpression(object obj)
    {
        // this test exists as an example of usage
        int eitherSwitch = obj switch
        {
            string s => StringHash(s),
            int n => IntHash(n),
            _ => 0,
        };

        Either<string, int> either = obj switch
        {
            string s => s,
            int n => n,
            _ => default,
        };

        int eitherMap = either.Map(StringHash, IntHash);

        eitherMap.Should().Be(eitherSwitch);

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        static int StringHash(string? s)
        {
            return (s?.GetHashCode() ?? 0) + 1;
        }

        static int IntHash(int n)
        {
            return n.GetHashCode() - 1;
        }
    }

    [Fact]
    public void EitherRightFuncAndEitherRightValShouldBeEqualIfSameRightResult()
    {
        var value = this.Fixture.Create<int>();
        Either<string, int> either1 = value;
        var either2 = new Either<string, int>(() => value);

        either1.Equals(either2).Should().BeTrue();
        (either1 == either2).Should().BeTrue();
        (either1.GetHashCode() == either2.GetHashCode()).Should().BeTrue();
    }

    [Fact]
    public void EitherRightShouldBeAssignableFromFunc()
    {
        var i = this.Fixture.Create<int>();

        Either<string, int> either = (Func<int>)Fn;

        int? actual = either;

        actual.Should().Be(i);

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        int Fn()
        {
            return i;
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData("a")]
    public void EitherShouldBeUsableInSwitchExpression(object obj)
    {
        // this test exists as an example of usage
        Func<Either<string, int>> act = () => obj switch
        {
            string s => Identity(s),
            int n => Identity(n),
            _ => default,
        };

        act.Should().NotThrow();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        static T Identity<T>(T val)
        {
            return val;
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData("a")]
    public void EitherShouldBeUsableInSwitchExpression2(object obj)
    {
        // this test exists as an example of usage
        Func<Either<string, int>> act = () => obj switch
        {
            string s => ReverseUpper(s),
            int n => Negate(n),
            _ => default,
        };

        act.Should().NotThrow();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        static string ReverseUpper(string s)
        {
            return string.Join(null, s.ToUpperInvariant().Reverse());
        }

        static int Negate(int n)
        {
            return -n;
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData("a")]
    public void EitherShouldBeUsableToMapToEither(object obj)
    {
        // this test exists as an example of usage
        Either<string, int> either = obj switch
        {
            string s => s,
            int n => n,
            _ => default,
        };

        Func<string?, string> reverseUpper = s => string.Join(null, (s ?? string.Empty).ToUpperInvariant().Reverse());

        Func<int, int> negate = n => -n;

        Func<Either<string?, int>> act = () => either.Map(reverseUpper.ToEitherLeft<string?, string?, int>(), negate.ToEitherRight<int, string?, int>());

        act.Should().NotThrow();
    }

    [Theory]
    [MemberData(nameof(GetHashCodeTheoryData))]
    public void GetHashCodeShouldReturnCorrectValue(string? left, int? right)
    {
        Either<string?, int?> either = (left, right) switch
        {
            (null, null) => default,
            (not null, _) => left,
            (_, not null) => right,
        };

        int expected = (left, left is not null, right, right is not null).GetHashCode();

        int actual = either.GetHashCode();

        actual.Should().Be(expected);
    }

    [Fact]
    public async Task IfLeftAsyncTaskShouldCallFunctionIfLeft()
    {
        var called = false;

        var value = this.Fixture.Create<bool>();
        Either<bool, int> either = value;

        await either.IfLeftAsync(Fn);

        either.IsLeft.Should().BeTrue();
        called.Should().BeTrue();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        Task Fn(bool _)
        {
            called = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task IfLeftAsyncTaskShouldNotCallFunctionIfRight()
    {
        var called = false;
        var value = this.Fixture.Create<int>();
        Either<bool, int> either = value;

        await either.IfLeftAsync(Fn);

        either.IsLeft.Should().BeFalse();
        called.Should().BeFalse();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        Task Fn(bool _)
        {
            called = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task IfLeftAsyncTaskTShouldCallFunctionIfLeft()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<bool>();
        Either<bool, int> either = value;

        await either.IfLeftAsync(b => mock.Object.LeftFuncAsync<bool?, bool?>(b));

        either.IsLeft.Should().BeTrue();
        mock.Verify(stuff => stuff.LeftFuncAsync<bool?, bool?>(value));
    }

    [Fact]
    public async Task IfLeftAsyncTaskTShouldNotCallFunctionIfRight()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<int>();
        Either<bool, int> either = value;

        await either.IfLeftAsync(b => mock.Object.LeftFuncAsync<bool?, bool?>(b));

        either.IsLeft.Should().BeFalse();
        mock.Verify(stuff => stuff.LeftFuncAsync<bool?, bool?>(It.IsAny<bool?>()), Times.Never);
    }

    [Fact]
    public async Task IfLeftAsyncTaskUnitShouldCallFunctionIfLeft()
    {
        var called = false;

        var value = this.Fixture.Create<bool>();
        Either<bool, int> either = value;

        await either.IfLeftAsync(Fn);

        either.IsLeft.Should().BeTrue();
        called.Should().BeTrue();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        Task<Unit> Fn(bool _)
        {
            called = true;
            return Task.FromResult(Unit.Default);
        }
    }

    [Fact]
    public async Task IfLeftAsyncTaskUnitShouldNotCallFunctionIfRight()
    {
        var called = false;
        var value = this.Fixture.Create<int>();
        Either<bool, int> either = value;

        await either.IfLeftAsync(Fn);

        either.IsLeft.Should().BeFalse();
        called.Should().BeFalse();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        Task<Unit> Fn(bool _)
        {
            called = true;
            return Task.FromResult(Unit.Default);
        }
    }

    [Fact]
    public void IfLeftShouldCallActionIfLeft()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<bool>();
        Either<bool, int> either = value;

        either.IfLeft(b => mock.Object.LeftFunc<bool?, bool?>(b));

        either.IsLeft.Should().BeTrue();
        mock.Verify(stuff => stuff.LeftFunc<bool?, bool?>(value));
    }

    [Fact]
    public void IfLeftShouldNotCallActionIfRight()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<int>();
        Either<bool, int> either = value;

        either.IfLeft(b => mock.Object.LeftFunc<bool?, bool?>(b));

        either.IsLeft.Should().BeFalse();
        mock.Verify(stuff => stuff.LeftFunc<bool?, bool?>(It.IsAny<bool?>()), Times.Never);
    }

    [Fact]
    public void IfLeftUnitShouldCallActionIfLeft()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<bool>();
        Either<bool, int> either = value;

        either.IfLeft(b => mock.Object.LeftAction(b));

        either.IsLeft.Should().BeTrue();
        mock.Verify(stuff => stuff.LeftAction(value));
    }

    [Fact]
    public void IfLeftUnitShouldNotCallActionIfRight()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<int>();
        Either<bool, int> either = value;

        either.IfLeft(b => mock.Object.LeftAction(b));

        either.IsLeft.Should().BeFalse();
        mock.Verify(stuff => stuff.LeftAction(It.IsAny<bool?>()), Times.Never);
    }

    [Fact]
    public async Task IfRightAsyncTaskShouldCallFunctionIfRight()
    {
        var called = false;

        var value = this.Fixture.Create<int>();
        Either<bool, int> either = value;

        await either.IfRightAsync(Fn);

        either.IsRight.Should().BeTrue();
        called.Should().BeTrue();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        Task Fn(int _)
        {
            called = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public void IfRightAsyncTaskShouldNotCallFunctionIfRight()
    {
        var called = false;
        var value = this.Fixture.Create<bool>();
        Either<bool, int> either = value;

        either.IfRightAsync(Fn);

        either.IsRight.Should().BeFalse();
        called.Should().BeFalse();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        Task Fn(int _)
        {
            called = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task IfRightAsyncTaskTShouldCallFunctionIfRight()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<int>();
        Either<bool, int> either = value;

        await either.IfRightAsync(b => mock.Object.RightFuncAsync<int?, bool?>(b));

        either.IsRight.Should().BeTrue();
        mock.Verify(stuff => stuff.RightFuncAsync<int?, bool?>(value));
    }

    [Fact]
    public async Task IfRightAsyncTaskTShouldNotCallFunctionIfLeft()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<bool>();
        Either<bool, int> either = value;

        await either.IfRightAsync(b => mock.Object.RightFuncAsync<int?, bool?>(b));

        either.IsRight.Should().BeFalse();
        mock.Verify(stuff => stuff.LeftFuncAsync<bool?, bool?>(It.IsAny<bool?>()), Times.Never);
    }

    [Fact]
    public async Task IfRightAsyncTaskUnitShouldCallFunctionIfRight()
    {
        var called = false;

        var value = this.Fixture.Create<int>();
        Either<bool, int> either = value;

        await either.IfRightAsync(Fn);

        either.IsRight.Should().BeTrue();
        called.Should().BeTrue();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        Task<Unit> Fn(int _)
        {
            called = true;
            return Task.FromResult(Unit.Default);
        }
    }

    [Fact]
    public async Task IfRightAsyncTaskUnitShouldNotCallFunctionIfLeft()
    {
        var called = false;
        var value = this.Fixture.Create<bool>();
        Either<bool, int> either = value;

        await either.IfRightAsync(Fn);

        either.IsRight.Should().BeFalse();
        called.Should().BeFalse();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        Task<Unit> Fn(int _)
        {
            called = true;
            return Task.FromResult(Unit.Default);
        }
    }

    [Fact]
    public void IfRightShouldCallActionIfRight()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<int>();
        Either<bool, int> either = value;

        either.IfRight(b => mock.Object.RightFunc<int?, bool?>(b));

        either.IsRight.Should().BeTrue();
        mock.Verify(stuff => stuff.RightFunc<int?, bool?>(value));
    }

    [Fact]
    public void IfRightShouldNotCallActionIfLeft()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<bool>();
        Either<bool, int> either = value;

        either.IfRight(b => mock.Object.RightFunc<int?, bool?>(b));

        either.IsRight.Should().BeFalse();
        mock.Verify(stuff => stuff.RightFunc<bool?, bool?>(It.IsAny<bool?>()), Times.Never);
    }

    [Fact]
    public void IfRightUnitShouldCallActionIfRight()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<int>();
        Either<bool, int> either = value;

        either.IfRight(b => mock.Object.RightAction(b));

        either.IsRight.Should().BeTrue();
        mock.Verify(stuff => stuff.RightAction(value));
    }

    [Fact]
    public void IfRightUnitShouldNotCallActionIfLeft()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<bool>();
        Either<bool, int> either = value;

        either.IfRight(b => mock.Object.RightAction(b));

        either.IsRight.Should().BeFalse();
        mock.Verify(stuff => stuff.RightAction(It.IsAny<bool?>()), Times.Never);
    }

    [Fact]
    public void ItShouldBeAnErrorToHaveTheSameTypesForLeftAndRightCtor()
    {
        Func<Either<int, int>> fn = () => new Either<int, int>();

        fn.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void MapShouldCallOnLeftIfLeft()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<string>();
        Either<string, int> either = value;

        either.Map(
            s => mock.Object.LeftFunc<string?, string>(s),
            n => mock.Object.RightFunc<int?, string>(n));

        mock.Verify(stuff => stuff.LeftFunc<string?, string>(value));
        mock.Verify(stuff => stuff.RightFunc<int?, string>(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void MapShouldCallOnRightIfRight()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<int>();
        Either<string, int> either = value;

        either.Map(
            s => mock.Object.LeftFunc<string?, string>(s),
            n => mock.Object.RightFunc<int?, string>(n));

        mock.Verify(stuff => stuff.LeftFunc<string?, string>(It.IsAny<string?>()), Times.Never);
        mock.Verify(stuff => stuff.RightFunc<int?, string>(It.IsAny<int>()));
    }

    [Fact]
    public void MatchActionShouldCallOnLeftIfLeft()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<string>();
        Either<string, int> either = value;

        either.Match(
            s => mock.Object.LeftAction(s),
            n => mock.Object.RightAction(n));

        mock.Verify(stuff => stuff.LeftAction(value));
        mock.Verify(stuff => stuff.RightAction(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void MatchActionShouldCallOnRightIfRight()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<int>();
        Either<string, int> either = value;

        either.Match(
            s => mock.Object.LeftAction(s),
            n => mock.Object.RightAction(n));

        mock.Verify(stuff => stuff.LeftAction(It.IsAny<string?>()), Times.Never);
        mock.Verify(stuff => stuff.RightAction(It.IsAny<int>()));
    }

    [Fact]
    public void MatchFunctionShouldCallOnLeftIfLeft()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<string>();
        Either<string, int> either = value;

        either.Match(
            s => mock.Object.LeftFunc<string?, string>(s),
            n => mock.Object.RightFunc<int?, string>(n));

        mock.Verify(stuff => stuff.LeftFunc<string?, string>(value));
        mock.Verify(stuff => stuff.RightFunc<int?, string>(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void MatchFunctionShouldCallOnRightIfRight()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = this.Fixture.Create<int>();
        Either<string, int> either = value;

        either.Match(
            s => mock.Object.LeftFunc<string?, string>(s),
            n => mock.Object.RightFunc<int?, string>(n));

        mock.Verify(stuff => stuff.LeftFunc<string?, string>(It.IsAny<string?>()), Times.Never);
        mock.Verify(stuff => stuff.RightFunc<int?, string>(It.IsAny<int>()));
    }

    [Fact]
    public void NewEitherShouldBeNeitherLeftNorRight()
    {
        Either<string, int> either = new();
        either.IsLeft.Should().BeFalse();
        either.IsRight.Should().BeFalse();
        either.IsNeither.Should().BeTrue();
    }

    [Fact]
    public void UninitializedEitherInstancesShouldBeEqual()
    {
        Either<string, int> either1 = default;
        Either<string, int> either2 = default;

        (either1 == either2).Should().BeTrue();
        ((string?)either1 == (string?)either2).Should().BeTrue();
        ((int?)either1 == (int?)either2).Should().BeTrue();
        (either1.GetHashCode() == either2.GetHashCode()).Should().BeTrue();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public interface IDoStuff
    {
        public void LeftAction<TLeft>(TLeft val);

        public TReturn LeftFunc<TLeft, TReturn>(TLeft val);

        public Task<TReturn> LeftFuncAsync<TLeft, TReturn>(TLeft val);
        public void RightAction<TRight>(TRight val);
        public TReturn RightFunc<TRight, TReturn>(TRight val);
        public Task<TReturn> RightFuncAsync<TRight, TReturn>(TRight val);
    }
}