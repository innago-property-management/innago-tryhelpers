namespace UnitTests;

using FluentAssertions;

using Innago.Shared.TryHelpers;

using Xunit.OpenCategories;

[UnitTest(nameof(UnitExtensions))]
public class UnitExtensionsTests
{
    [Fact]
    public void AsFuncShouldReturnFunc()
    {
        var called = false;

        Action a = DoSomething;
        Func<Unit> f = a.AsFunc();

        called.Should().BeFalse();

        f();

        called.Should().BeTrue();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        void DoSomething()
        {
            called = true;
        }
    }

    [Fact]
    public async Task AsFuncTaskUnitShouldReturnFuncTaskUnit()
    {
        var called = false;

        Func<Task> func = DoSomethingAsync();
        Func<Task<Unit>> unitFunc = func.AsFuncTaskUnit();
        await unitFunc();

        called.Should().BeTrue();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        Func<Task> DoSomethingAsync()
        {
            return () =>
            {
                called = true;
                return Task.CompletedTask;
            };
        }
    }

    [Fact]
    public async Task AsFuncTaskUnitShouldReturnFuncTTaskUnit()
    {
        var called = false;

        Func<bool, Task> func = DoSomethingAsync<bool>();
        Func<bool, Task<Unit>>? unitFunc = func.AsFuncTaskUnit();
        await unitFunc(true);

        called.Should().BeTrue();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        Func<T, Task> DoSomethingAsync<T>()
        {
            return _ =>
            {
                called = true;
                return Task.CompletedTask;
            };
        }
    }

    [Fact]
    public void AsFuncTShouldReturnFunc()
    {
        var called = false;

        Action<object?> a = DoSomething;
        Func<object?, Unit> f = a.AsFunc();

        called.Should().BeFalse();

        f(null);

        called.Should().BeTrue();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        void DoSomething(object? _)
        {
            called = true;
        }
    }

    [Fact]
    public async Task AsUnitTaskShouldConvertTaskToTaskUnitAndRunTask()
    {
        var called = false;

        Task t = DoSomethingAsync();
        Task<Unit> tu = t.AsUnitTask();

        await tu;

        called.Should().BeTrue();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        async Task DoSomethingAsync()
        {
            await Task.Yield();
            called = true;
        }
    }
}