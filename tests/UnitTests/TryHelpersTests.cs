namespace UnitTests;

using FluentAssertions;

using Innago.Shared.TryHelpers;

using Xunit.OpenCategories;

using static Innago.Shared.TryHelpers.TryHelpers;

[UnitTest(nameof(TryHelpers))]
public class TryHelpersTests
{
    [Fact]
    public void TryActionFailedShouldReturnResultWithException()
    {
        Exception ex = new();
        Exception? exUsed = null;

        Result result = Try(Act);
        result.HasFailed.Should().BeTrue();

        result.Match(() => { }, exception => { exUsed = exception; });
        exUsed.Should().Be(ex);

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        void Act()
        {
            throw ex;
        }
    }

    [Fact]
    public void TryActionSucceededShouldReturnResultSucceeded()
    {
        Result result = Try(Act);
        result.HasSucceeded.Should().BeTrue();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        static void Act()
        {
            /* empty */
        }
    }

    [Fact]
    public async Task TryAsyncShouldReturnResultWithException()
    {
        Exception ex = new();
        Exception? thrown = null;

        Result<bool> result = await TryAsync((Func<Task<bool>>)Func);

        result.Map(b => b,
            exception =>
            {
                thrown = exception;
                return false;
            });

        thrown.Should().Be(ex);

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        async Task<bool> Func()
        {
            await Task.Yield();
            throw ex;
        }
    }

    [Fact]
    public async Task TryAsyncShouldReturnResultWithValue()
    {
        const int value = 123456;
        int? used = null;

        Result<int> result = await TryAsync(Func);

        result.Match(i => used = i, _ => { });

        used.Should().Be(value);

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        static async Task<int> Func()
        {
            await Task.Yield();
            return value;
        }
    }

    [Fact]
    public async Task TryAsyncTaskShouldReturnException()
    {
        Exception ex = new();
        Exception? thrown = null;

        Result result = await TryAsync(Act);

        result.IfFailed(exception => thrown = exception);

        thrown.Should().Be(ex);

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        Task Act()
        {
            throw ex;
        }
    }

    [Fact]
    public async Task TryASyncTaskShouldReturnSucceeded()
    {
        Result result = await TryAsync(Act);

        result.HasSucceeded.Should().BeTrue();

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        static async Task Act()
        {
            await Task.Yield();
        }
    }

    [Fact]
    public void TryFuncShouldReturnResultWithException()
    {
        Exception ex = new();
        Exception? thrown = null;

        Result<bool> result = Try((Func<bool>)Func);
        result.Match(_ => { }, exception => { thrown = exception; });

        thrown.Should().Be(ex);

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        bool Func()
        {
            throw ex;
        }
    }

    [Fact]
    public void TryFuncShouldReturnResultWithValue()
    {
        const bool value = true;
        bool? used = null;

        Result<bool> result = Try(Func);
        result.Match(b => used = b, _ => { });

        used.Should().Be(value);

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        static bool Func()
        {
            return value;
        }
    }
}