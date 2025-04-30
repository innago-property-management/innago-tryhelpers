namespace UnitTests;

using Innago.Shared.TryHelpers;

using Moq;

using Xunit.OpenCategories;

[UnitTest(nameof(BooleanFunctionalExtensions))]
public class BooleanFunctionalExtensionsTests
{
    public static TheoryData<bool> TheoryData
    {
        get
        {
            var data = new TheoryData<bool>
            {
                false,
                true,
            };

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public async Task IfFalseAsyncShouldRuMethodIfFalse(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedTimes = value ? Times.Never() : Times.Once();

        await value.IfFalseAsync(actor.AsyncMethod);

        Mock.Get(actor).Verify(methods => methods.AsyncMethod(), expectedTimes);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public async Task IfFalseAsyncUnitShouldRuMethodIfFalse(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedTimes = value ? Times.Never() : Times.Once();

        Func<Task> asyncMethod = actor.AsyncMethod;
        Func<Task<Unit>> onFalse = asyncMethod.AsFuncTaskUnit();
        await value.IfFalseAsync(onFalse);

        Mock.Get(actor).Verify(methods => methods.AsyncMethod(), expectedTimes);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public void IfFalseShouldRuMethodIfFalse(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedTimes = value ? Times.Never() : Times.Once();

        value.IfFalse(actor.VoidMethod);

        Mock.Get(actor).Verify(methods => methods.VoidMethod(), expectedTimes);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public void IfFalseUnitShouldRuMethodIfFalse(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedTimes = value ? Times.Never() : Times.Once();

        Action voidMethod = actor.VoidMethod;
        Func<Unit> onFalse = voidMethod.AsFunc();
        value.IfFalse(onFalse);

        Mock.Get(actor).Verify(methods => methods.VoidMethod(), expectedTimes);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public async Task IfTrueAsyncShouldRuMethodIfTrue(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedTimes = value ? Times.Once() : Times.Never();

        await value.IfTrueAsync(actor.AsyncMethod);

        Mock.Get(actor).Verify(methods => methods.AsyncMethod(), expectedTimes);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public async Task IfTrueAsyncUnitShouldRuMethodIfTrue(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedTimes = value ? Times.Once() : Times.Never();

        Func<Task> asyncMethod = actor.AsyncMethod;
        Func<Task<Unit>> onTrue = asyncMethod.AsFuncTaskUnit();
        await value.IfTrueAsync(onTrue);

        Mock.Get(actor).Verify(methods => methods.AsyncMethod(), expectedTimes);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public void IfTrueShouldRuMethodIfTrue(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedTimes = value ? Times.Once() : Times.Never();

        value.IfTrue(actor.VoidMethod);

        Mock.Get(actor).Verify(methods => methods.VoidMethod(), expectedTimes);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public void IfTrueUnitShouldRuMethodIfTrue(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedTimes = value ? Times.Once() : Times.Never();

        Action voidMethod = actor.VoidMethod;
        Func<Unit> onTrue = voidMethod.AsFunc();
        value.IfTrue(onTrue);

        Mock.Get(actor).Verify(methods => methods.VoidMethod(), expectedTimes);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public async Task MapShouldRuMethodCorrectMethodAsync(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedOnTrue = value ? Times.Once() : Times.Never();
        Times expectedOnFalse = value ? Times.Never() : Times.Once();

        await value.Map(actor.ResultMethodAsync, actor.ResultMethodAsync2)!;

        Mock.Get(actor).Verify(methods => methods.ResultMethodAsync(), expectedOnTrue);
        Mock.Get(actor).Verify(methods => methods.ResultMethodAsync2(), expectedOnFalse);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public void MapShouldRunMethodCorrectMethod(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedOnTrue = value ? Times.Once() : Times.Never();
        Times expectedOnFalse = value ? Times.Never() : Times.Once();

        value.Map(actor.ResultMethod, actor.ResultMethod2);

        Mock.Get(actor).Verify(methods => methods.ResultMethod(), expectedOnTrue);
        Mock.Get(actor).Verify(methods => methods.ResultMethod2(), expectedOnFalse);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public async Task MatchAsyncShouldRuMethodCorrectMethod(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedOnTrue = value ? Times.Once() : Times.Never();
        Times expectedOnFalse = value ? Times.Never() : Times.Once();

        await value.MatchAsync(actor.AsyncMethod, actor.AsyncMethod2);

        Mock.Get(actor).Verify(methods => methods.AsyncMethod(), expectedOnTrue);
        Mock.Get(actor).Verify(methods => methods.AsyncMethod2(), expectedOnFalse);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public async Task MatchAsyncUnitShouldRuMethodCorrectMethod(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedOnTrue = value ? Times.Once() : Times.Never();
        Times expectedOnFalse = value ? Times.Never() : Times.Once();

        Func<Task> asyncMethod = actor.AsyncMethod;
        Func<Task<Unit>> onTrue = asyncMethod.AsFuncTaskUnit();

        Func<Task> asyncMethod2 = actor.AsyncMethod2;
        Func<Task<Unit>> onFalse = asyncMethod2.AsFuncTaskUnit();

        await value.MatchAsync(onTrue, onFalse);

        Mock.Get(actor).Verify(methods => methods.AsyncMethod(), expectedOnTrue);
        Mock.Get(actor).Verify(methods => methods.AsyncMethod2(), expectedOnFalse);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public void MatchShouldRuMethodCorrectMethod(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedOnTrue = value ? Times.Once() : Times.Never();
        Times expectedOnFalse = value ? Times.Never() : Times.Once();

        value.Match(actor.VoidMethod, actor.VoidMethod2);

        Mock.Get(actor).Verify(methods => methods.VoidMethod(), expectedOnTrue);
        Mock.Get(actor).Verify(methods => methods.VoidMethod2(), expectedOnFalse);
    }

    [Theory]
    [MemberData(nameof(TheoryData))]
    public void MatchUnitShouldRuMethodCorrectMethod(bool value)
    {
        var actor = Mock.Of<BooleanDummyMethods>(MockBehavior.Loose);
        Times expectedOnTrue = value ? Times.Once() : Times.Never();
        Times expectedOnFalse = value ? Times.Never() : Times.Once();

        Action voidMethod = actor.VoidMethod;
        Action voidMethod2 = actor.VoidMethod2;
        value.Match(voidMethod.AsFunc(), voidMethod2.AsFunc());

        Mock.Get(actor).Verify(methods => methods.VoidMethod(), expectedOnTrue);
        Mock.Get(actor).Verify(methods => methods.VoidMethod2(), expectedOnFalse);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    internal abstract class BooleanDummyMethods
    {
        public abstract Task AsyncMethod();
        public abstract Task AsyncMethod2();
        public abstract object? ResultMethod();
        public abstract object? ResultMethod2();
        public abstract Task<object?> ResultMethodAsync();
        public abstract Task<object?> ResultMethodAsync2();
        public abstract void VoidMethod();
        public abstract void VoidMethod2();
    }
}