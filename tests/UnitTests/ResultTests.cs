namespace UnitTests;

using FluentAssertions;

using Innago.Shared.TryHelpers;

using Moq;

using Xunit.OpenCategories;

[UnitTest(nameof(Result))]
public class ResultTests
{
    [Fact]
    public void DefaultResultShouldBeFailed()
    {
        Result result = default;
        result.HasFailed.Should().BeTrue();
        result.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public void ErrorResultsShouldBeEqualForSameError()
    {
        var error = new Exception(Guid.NewGuid().ToString());

        Result result1 = error;
        Result result2 = error;

        int hash1 = result1.GetHashCode();
        int hash2 = result2.GetHashCode();

        result1.Should().Be(result2);
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void ErrorResultsShouldNotBeEqualForDifferentErrorEvenIfSameMessage()
    {
        var message = Guid.NewGuid().ToString();
        var error1 = new Exception(message);
        var error2 = new Exception(message);

        Result result1 = error1;
        Result result2 = error2;

        int hash1 = result1.GetHashCode();
        int hash2 = result2.GetHashCode();

        result1.Should().NotBe(result2);
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ExceptionAsResultShouldReturnCorrectValue()
    {
        Exception ex = new();

        var result = Result.ExceptionAsResult(ex);

        ((Exception)result)!.Should().Be(ex);
    }

    [Fact]
    public async Task ExceptionAsTaskResultShouldReturnCorrectValue()
    {
        Exception ex = new();

        var result = await Result.ExceptionAsTaskResult(ex);

        ((Exception)result)!.Should().Be(ex);
    }

    [Fact]
    public void IfFailedShouldEvaluateForFailedResult()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result result = ex;

        result.IfFailed(exception => mock.Object.FailureAction(exception));

        mock.Verify(stuff => stuff.FailureAction(ex));
    }

    [Fact]
    public void IfFailedShouldNotEvaluateForSuccessfulResult()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        Result result = Result.Success;

        result.IfFailed(exception => mock.Object.FailureAction(exception));

        mock.Verify(stuff => stuff.FailureAction(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public void IfFailedUnitShouldEvaluateForFailedResult()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result result = ex;

        Action<Exception?> onError = exception => mock.Object.FailureAction(exception);

        result.IfFailed(onError.AsFunc());

        mock.Verify(stuff => stuff.FailureAction(ex));
    }

    [Fact]
    public void IfFailedUnitShouldNotEvaluateForSuccessfulResult()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        Result result = Result.Success;

        Action<Exception?> onError = exception => mock.Object.FailureAction(exception);

        result.IfFailed(onError.AsFunc());

        mock.Verify(stuff => stuff.FailureAction(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public void IfSucceededShouldEvaluateForSuccessfulResult()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        Result result = Result.Success;

        result.IfSucceeded(() => mock.Object.SuccessAction());

        mock.Verify(stuff => stuff.SuccessAction());
    }

    [Fact]
    public void IfSucceededShouldNotEvaluateForFailedResult()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result result = ex;

        result.IfSucceeded(() => mock.Object.SuccessAction());

        mock.Verify(stuff => stuff.SuccessAction(), Times.Never);
    }

    [Fact]
    public void IfSucceededUnitShouldEvaluateForSuccessfulResult()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        Result result = Result.Success;

        Action onSuccess = () => mock.Object.SuccessAction();
        result.IfSucceeded(onSuccess.AsFunc());

        mock.Verify(stuff => stuff.SuccessAction());
    }

    [Fact]
    public void IfSucceededUnitShouldNotEvaluateForFailedResult()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result result = ex;

        Action onSuccess = () => mock.Object.SuccessAction();
        result.IfSucceeded(onSuccess.AsFunc());

        mock.Verify(stuff => stuff.SuccessAction(), Times.Never);
    }

    [Fact]
    public void InitializingWithAnExceptionShouldSetHasFailed()
    {
        Result result = new Exception();
        result.HasFailed.Should().BeTrue();
        result.HasSucceeded.Should().BeFalse();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ItShouldBeConvertibleToException(bool faulted)
    {
        var ex = new Exception("");
        Result result = (faulted ? ex : null)!;

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
    public void MatchShouldCallOnErrorIfFailed()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        var ex = new Exception();
        Result result = ex;

        result.Match(() => mock.Object.SuccessAction(), exception => mock.Object.FailureAction(exception));

        mock.Verify(stuff => stuff.SuccessAction(), Times.Never);
        mock.Verify(stuff => stuff.FailureAction(ex));
    }

    [Fact]
    public void MatchShouldCallOnSuccessIfSucceeded()
    {
        var mock = new Mock<IDoStuff>(MockBehavior.Loose);
        Result result = Result.Success;

        result.Match(() => mock.Object.SuccessAction(), exception => mock.Object.FailureAction(exception));

        mock.Verify(stuff => stuff.SuccessAction());
        mock.Verify(stuff => stuff.FailureAction(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public void NewResultShouldBeSuccessful()
    {
        Result result = new();
        result.HasSucceeded.Should().BeTrue();
        result.HasFailed.Should().BeFalse();
    }

    [Fact]
    public void SuccessResultsShouldBeEqual()
    {
        Result result1 = Result.Success;
        Result result2 = Result.Success;

        int hash1 = result1.GetHashCode();
        int hash2 = result2.GetHashCode();

        result1.Should().Be(result2);
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void SuccessShouldReturnSucceededResult()
    {
        Result result = Result.Success;
        result.HasSucceeded.Should().BeTrue();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public interface IDoStuff
    {
        public void FailureAction(Exception? e);
        public void SuccessAction();
    }
}