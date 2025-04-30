namespace UnitTests;

using FluentAssertions;

using Innago.Shared.TryHelpers;

using Moq;

using Xunit.OpenCategories;

[UnitTest(nameof(Result<object>))]
public class ResultTTests
{
    [Fact]
    public void DefaultResultShouldBeFailed()
    {
        Result<object> result = default;
        result.HasFailed.Should().BeTrue();
        result.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public void ExceptionAsResultShouldReturnCorrectValue()
    {
        Exception ex = new();

        Result<int> result = Result<int>.ExceptionAsResult(ex);

        ((Exception)result)!.Should().Be(ex);
    }

    [Fact]
    public async Task ExceptionAsTaskResultShouldReturnCorrectValue()
    {
        Exception ex = new();

        Result<int> result = await Result<int>.ExceptionAsTaskResult(ex);

        ((Exception)result)!.Should().Be(ex);
    }

    [Fact]
    public async Task IfFailedAsyncShouldCallFuncIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<bool> result = ex;

        await result.IfFailedAsync(exception => mock.Object.FailureActionAsync(exception));

        mock.Verify(stuff => stuff.FailureActionAsync(It.IsAny<Exception>()));
    }

    [Fact]
    public async Task IfFailedAsyncShouldNotCallFuncIfSuccess()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);

        Result<bool> result = true;

        await result.IfFailedAsync(exception => mock.Object.FailureActionAsync(exception));

        mock.Verify(stuff => stuff.FailureActionAsync(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public async Task IfFailedAsyncTShouldCallFuncIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<bool> result = ex;

        await result.IfFailedAsync(exception => mock.Object.FailureFuncAsync<bool>(exception));

        mock.Verify(stuff => stuff.FailureFuncAsync<bool>(It.IsAny<Exception>()));
    }

    [Fact]
    public async Task IfFailedAsyncTShouldNotCallFuncIfSuccess()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);

        Result<bool> result = true;

        await result.IfFailedAsync(exception => mock.Object.FailureFuncAsync<bool>(exception));

        mock.Verify(stuff => stuff.FailureFuncAsync<bool>(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public async Task IfFailedAsyncUnitShouldCallFuncIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<bool> result = ex;

        Func<Exception?, Task> func = exception => mock.Object.FailureActionAsync(exception);
        Func<Exception?, Task<Unit>> onError = func.AsFuncTaskUnit();
        await result.IfFailedAsync(onError);

        mock.Verify(stuff => stuff.FailureActionAsync(It.IsAny<Exception>()));
    }

    [Fact]
    public async Task IfFailedAsyncUnitShouldNotCallFuncIfSuccess()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);

        Result<bool> result = true;

        Func<Exception?, Task> func = exception => mock.Object.FailureActionAsync(exception);
        Func<Exception?, Task<Unit>> onError = func.AsFuncTaskUnit();

        await result.IfFailedAsync(onError);

        mock.Verify(stuff => stuff.FailureActionAsync(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public void IfFailedShouldCallActionIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<bool> result = ex;

        result.IfFailed(exception => mock.Object.FailureAction(exception));

        mock.Verify(stuff => stuff.FailureAction(ex));
    }

    [Fact]
    public void IfFailedShouldCallFuncIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<bool> result = ex;

        result.IfFailed(exception => mock.Object.FailureFunc<bool>(exception));

        mock.Verify(stuff => stuff.FailureFunc<bool>(ex));
    }

    [Fact]
    public void IfFailedShouldNotCallActionIfSucceeded()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        Result<bool> result = false;

        result.IfFailed(exception => mock.Object.FailureAction(exception));

        mock.Verify(stuff => stuff.FailureAction(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public void IfFailedShouldNotCallFuncIfSucceeded()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        Result<bool> result = false;

        result.IfFailed(exception => mock.Object.FailureFunc<bool>(exception));

        mock.Verify(stuff => stuff.FailureFunc<bool>(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public void IfFailedUnitShouldCallActionIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<bool> result = ex;

        Action<Exception?> onError = exception => mock.Object.FailureAction(exception);
        result.IfFailed(onError.AsFunc());

        mock.Verify(stuff => stuff.FailureAction(ex));
    }

    [Fact]
    public void IfFailedUnitShouldNotCallActionIfSucceeded()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        Result<bool> result = false;

        Action<Exception?> onError = exception => mock.Object.FailureAction(exception);
        result.IfFailed(onError.AsFunc());

        mock.Verify(stuff => stuff.FailureAction(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public async Task IfSucceededAsyncTShouldCallFuncIfSuccess()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);

        Result<bool> result = true;

        await result.IfSucceededAsync(b => mock.Object.SuccessFuncAsync<bool, bool>(b));

        mock.Verify(stuff => stuff.SuccessFuncAsync<bool, bool>(It.IsAny<bool>()));
    }

    [Fact]
    public async Task IfSucceededAsyncTShouldNotCallFuncIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<bool> result = ex;

        await result.IfSucceededAsync(value => mock.Object.SuccessFuncAsync<bool, bool>(value));

        mock.Verify(stuff => stuff.SuccessFuncAsync<bool, bool>(It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task IfSucceededAsyncTUnitShouldCallFuncIfSuccess()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);

        Result<bool> result = true;

        Func<bool, Task> func = value => mock.Object.SuccessFuncAsync<bool, bool>(value);
        Func<bool, Task<Unit>> onSuccess = func.AsFuncTaskUnit();

        await result.IfSucceededAsync(onSuccess);

        mock.Verify(stuff => stuff.SuccessFuncAsync<bool, bool>(It.IsAny<bool>()));
    }

    [Fact]
    public async Task IfSucceededAsyncTUnitShouldNotCallFuncIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<bool> result = ex;

        Func<bool, Task> func = value => mock.Object.SuccessFuncAsync<bool, bool>(value);
        Func<bool, Task<Unit>> onSuccess = func.AsFuncTaskUnit();
        await result.IfSucceededAsync(onSuccess);

        mock.Verify(stuff => stuff.SuccessFuncAsync<bool, bool>(It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public void IfSucceededShouldCallActionIfSuccess()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        const int val = 42;
        Result<int> result = val;

        result.IfSucceeded(i => mock.Object.SuccessAction(i));

        mock.Verify(stuff => stuff.SuccessAction(val));
    }

    [Fact]
    public void IfSucceededShouldNotCallActionIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);

        Result<int> result = new InvalidOperationException();

        result.IfSucceeded(i => mock.Object.SuccessAction(i));

        mock.Verify(stuff => stuff.SuccessAction(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void IfSucceededUnitShouldCallActionIfSuccess()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        const int val = 42;
        Result<int> result = val;

        Action<int> onSuccess = i => mock.Object.SuccessAction(i);
        result.IfSucceeded(onSuccess.AsFunc());

        mock.Verify(stuff => stuff.SuccessAction(val));
    }

    [Fact]
    public void IfSucceededUnitShouldNotCallActionIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);

        Result<int> result = new InvalidOperationException();

        Action<int> onSuccess = i => mock.Object.SuccessAction(i);
        result.IfSucceeded(onSuccess.AsFunc());

        mock.Verify(stuff => stuff.SuccessAction(It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ItShouldBeConvertibleToException(bool faulted)
    {
        var ex = new Exception("");
        Result<int> result = faulted ? ex : 1;

        Exception? exception = result;

        if (faulted)
        {
            exception.Should().Be(ex);
        }
        else
        {
            exception.Should().BeNull();
        }
    }

    [Fact]
    public void MapShouldCallOnErrorIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<string> result = ex;

        result.Map(
            s => mock.Object.SuccessFunc<string, bool>(s),
            exception => mock.Object.FailureFunc<bool>(exception));

        mock.Verify(stuff => stuff.SuccessFunc<string, bool>(It.IsAny<string>()), Times.Never);
        mock.Verify(stuff => stuff.FailureFunc<bool>(ex));
    }

    [Fact]
    public void MapShouldCallOnSuccessIfSuccess()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = Guid.NewGuid().ToString();
        Result<string> result = value;

        result.Map(
            s => mock.Object.SuccessFunc<string, bool>(s),
            exception => mock.Object.FailureFunc<bool>(exception));

        mock.Verify(stuff => stuff.SuccessFunc<string, bool>(value));
        mock.Verify(stuff => stuff.FailureFunc<bool>(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public void MatchFuncShouldCallOnErrorIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<string> result = ex;

        Action<string?> onSuccess = s => mock.Object.SuccessAction(s);

        Action<Exception?> onError = exception => mock.Object.FailureAction(exception);

        result.Match(onSuccess.AsFunc(), onError.AsFunc());

        mock.Verify(stuff => stuff.SuccessAction(It.IsAny<string>()), Times.Never);
        mock.Verify(stuff => stuff.FailureAction(ex));
    }

    [Fact]
    public void MatchFuncShouldCallOnSuccessIfSuccess()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = Guid.NewGuid().ToString();
        Result<string> result = value;

        Action<string?> onSuccess = s => mock.Object.SuccessAction(s);

        Action<Exception?> onError = exception => mock.Object.FailureAction(exception);

        result.Match(onSuccess.AsFunc(), onError.AsFunc());

        mock.Verify(stuff => stuff.SuccessAction(value));
        mock.Verify(stuff => stuff.FailureAction(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public void MatchShouldCallOnErrorIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<string> result = ex;

        result.Match(
            s => mock.Object.SuccessFunc<string, bool>(s),
            exception => mock.Object.FailureFunc<bool>(exception));

        mock.Verify(stuff => stuff.SuccessFunc<string, bool>(It.IsAny<string>()), Times.Never);
        mock.Verify(stuff => stuff.FailureFunc<bool>(ex));
    }

    [Fact]
    public void MatchShouldCallOnSuccessIfSuccess()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = Guid.NewGuid().ToString();
        Result<string> result = value;

        result.Match(
            s => mock.Object.SuccessFunc<string, bool>(s),
            exception => mock.Object.FailureFunc<bool>(exception));

        mock.Verify(stuff => stuff.SuccessFunc<string, bool>(value));
        mock.Verify(stuff => stuff.FailureFunc<bool>(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public void MatchUnitShouldCallOnErrorIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result<string> result = ex;

        Action<string?> onSuccess = s => mock.Object.SuccessFunc<string, bool>(s);

        Action<Exception?> onFailure = exception => mock.Object.FailureFunc<bool>(exception);

        result.Match(
            onSuccess.AsFunc(),
            onFailure.AsFunc());

        mock.Verify(stuff => stuff.SuccessFunc<string, bool>(It.IsAny<string>()), Times.Never);
        mock.Verify(stuff => stuff.FailureFunc<bool>(ex));
    }

    [Fact]
    public void MatchUnitShouldCallOnSuccessIfSuccess()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var value = Guid.NewGuid().ToString();
        Result<string> result = value;

        Action<string?> onSuccess = s => mock.Object.SuccessFunc<string, bool>(s);
        Action<Exception?> onError = exception => mock.Object.FailureFunc<bool>(exception);

        result.Match(onSuccess.AsFunc(), onError.AsFunc());

        mock.Verify(stuff => stuff.SuccessFunc<string, bool>(value));
        mock.Verify(stuff => stuff.FailureFunc<bool>(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public void NewResultShouldBeSucceeded()
    {
        Result<object> result = new();
        result.HasSucceeded.Should().BeTrue();
        result.HasFailed.Should().BeFalse();
    }

    [Fact]
    public void ResultFromExceptionShouldBeFailed()
    {
        Exception ex = new();
        Result<object> result = ex;
        result.HasFailed.Should().BeTrue();
    }

    [Fact]
    public void ResultFromValueShouldBeSucceeded()
    {
        Result<int> result = 1;
        result.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public void ResultsWithDifferentErrorShouldNotBeEqual()
    {
        var message = Guid.NewGuid().ToString();
        var error1 = new Exception(message);
        var error2 = new Exception(message);

        Result<string> result1 = error1;
        Result<string> result2 = error2;

        result1.Should().NotBe(result2);
        result1.GetHashCode().Should().NotBe(result2.GetHashCode());
    }

    [Fact]
    public void ResultsWithDifferentValuesShouldNotBeEqual()
    {
        Result<string> result1 = Guid.NewGuid().ToString();
        Result<string> result2 = Guid.NewGuid().ToString();

        result1.Should().NotBe(result2);
        result1.GetHashCode().Should().NotBe(result2.GetHashCode());
    }

    [Fact]
    public void ResultsWithSameErrorShouldBeEqual()
    {
        var message = Guid.NewGuid().ToString();
        var error = new Exception(message);

        Result<string> result1 = error;
        Result<string> result2 = error;

        result1.Should().Be(result2);
        result1.GetHashCode().Should().Be(result2.GetHashCode());
    }

    [Fact]
    public void ResultsWithSameValueShouldBeEqual()
    {
        var value = Guid.NewGuid().ToString();

        Result<string> result1 = value;
        Result<string> result2 = value;

        result1.Should().Be(result2);
        result1.GetHashCode().Should().Be(result2.GetHashCode());
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public interface IDoStuff
    {
        public void FailureAction(Exception? e);
        public Task FailureActionAsync(Exception? e);

        public TResult? FailureFunc<TResult>(Exception? e);
        public Task<TResult?> FailureFuncAsync<TResult>(Exception? e);
        public void SuccessAction<T>(T? val);
        public TResult? SuccessFunc<T, TResult>(T? val);
        public Task<TResult?> SuccessFuncAsync<T, TResult>(T? val);
    }
}