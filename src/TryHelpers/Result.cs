#nullable enable
namespace Innago.Shared.TryHelpers;

using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

/// <summary>
///     A result.
/// </summary>
/// <remarks>
///     This is inspired by https://github.com/louthy/language-ext/blob/main/LanguageExt.Core/Common/Result/Result.cs.
///     This is a record struct `default` => FAILED, `new` => SUCCEEDED.
/// </remarks>
[PublicAPI]
public record struct Result
{
    private Result<Unit> contained;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Result" /> class in a successful state.
    /// </summary>
    public Result()
    {
        this.contained = Unit.Default;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Result" /> class in a failed state.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public Result(Exception error)
    {
        this.contained = error;
    }

    /// <summary>
    ///     True, if exception.
    /// </summary>
    public bool HasFailed => this.contained.HasFailed;

    /// <summary>
    ///     True, if no exception.
    /// </summary>
    public bool HasSucceeded => this.contained.HasSucceeded;

    /// <summary>
    ///     A succeeded result.
    /// </summary>
    public static Result Success => new();

    /// <summary>
    ///     Returns an exception as a result.
    /// </summary>
    /// <param name="exception">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     The <see cref="Result" />.
    /// </returns>
    public static Result ExceptionAsResult(Exception exception)
    {
        return (Result)exception;
    }

    /// <summary>
    ///     Returns an exception as a task result.
    /// </summary>
    /// <param name="exception">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" />.
    /// </returns>
    public static Task<Result> ExceptionAsTaskResult(Exception exception)
    {
        return Task.FromResult(ExceptionAsResult(exception));
    }

    /// <summary>
    ///     Calls a function if there is an error.
    /// </summary>
    /// <param name="onError">
    ///     The error callback function.
    /// </param>
    /// <remarks>
    ///     Sample use case: logging.
    /// </remarks>
    public void IfFailed(Action<Exception?> onError)
    {
        this.contained.IfFailed(onError);
    }

    /// <summary>
    ///     Calls a function if there is an error.
    /// </summary>
    /// <param name="onError">
    ///     The error callback function.
    /// </param>
    /// <remarks>
    ///     Sample use case: logging.
    /// </remarks>
    public Unit IfFailed(Func<Exception?, Unit> onError)
    {
        return this.contained.IfFailed(onError);
    }

    /// <summary>
    ///     Calls the supplied function if the result succeeded.
    /// </summary>
    /// <param name="onSuccess">
    ///     The function to call on success.
    /// </param>
    public void IfSucceeded(Action onSuccess)
    {
        this.contained.IfSucceeded(_ => onSuccess());
    }

    /// <summary>
    ///     Calls the supplied function if the result succeeded.
    /// </summary>
    /// <param name="onSuccess">
    ///     The function to call on success.
    /// </param>
    public Unit IfSucceeded(Func<Unit> onSuccess)
    {
        return this.contained.IfSucceeded(_ => onSuccess());
    }

    /// <summary>
    ///     Conditional continuation action.
    /// </summary>
    /// <param name="onSuccess">
    ///     The function to call if success.
    /// </param>
    /// <param name="onError">
    ///     The function to call if error.
    /// </param>
    /// <remarks>
    ///     Calls <paramref name="onError" /> if failed; otherwise, calls <paramref name="onSuccess" />.
    /// </remarks>
    public void Match(Action onSuccess, Action<Exception?> onError)
    {
        this.contained.Match(_ => onSuccess(), onError);
    }

    /// <summary>
    ///     Conditional continuation action.
    /// </summary>
    /// <param name="onSuccess">
    ///     The function to call if success.
    /// </param>
    /// <param name="onError">
    ///     The function to call if error.
    /// </param>
    /// <remarks>
    ///     Calls <paramref name="onError" /> if failed; otherwise, calls <paramref name="onSuccess" />.
    /// </remarks>
    public Unit Match(Func<Unit> onSuccess, Func<Exception?, Unit> onError)
    {
        return this.contained.Map(_ => onSuccess(), onError);
    }

    /// <summary>
    ///     Makes a <see cref="Result" /> from an <see cref="Exception" />.
    /// </summary>
    /// <param name="exception">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     A <see cref="Result" />.
    /// </returns>
    public static implicit operator Result(Exception exception)
    {
        return new Result(exception);
    }

    /// <summary>
    ///     Makes an <see cref="Exception" /> from a <see cref="Result" />.
    /// </summary>
    /// <param name="result">
    ///     The result.
    /// </param>
    /// <returns>
    ///     An <see cref="Exception" />.
    /// </returns>
    public static implicit operator Exception?(Result result)
    {
        return result.contained;
    }
}