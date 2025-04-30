#nullable enable
namespace Innago.Shared.TryHelpers;

using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

/// <summary>
///     A result.
/// </summary>
/// <typeparam name="T">
///     The type used for a successful result.
/// </typeparam>
/// <remarks>
///     This is inspired by https://github.com/louthy/language-ext/blob/main/LanguageExt.Core/Common/Result/Result.cs.
///     This is a record struct `default` => FAILED, `new` => SUCCEEDED.
///     This type is the same as <see cref="Either{T,Exception}"/> except that it defaults to failed.
/// </remarks>
[PublicAPI]
public record struct Result<T>
{
    private Either<T?, Exception> either;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Result{T}" /> class.
    /// </summary>
    public Result()
    {
        this.either = default(T);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Result{T}" /> class.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    public Result(T? value)
    {
        this.either = value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Result{T}" /> class.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public Result(Exception error)
    {
        this.either = error;
    }

    /// <summary>
    ///     True, if exception.
    /// </summary>
    public bool HasFailed => this.either.IsRight || this.either.IsNeither;

    /// <summary>
    ///     True, if no exception.
    /// </summary>
    public bool HasSucceeded => this.either.IsLeft;

    /// <summary>
    ///     Makes a result from an exception.
    /// </summary>
    /// <param name="exception">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     A result.
    /// </returns>
    public static Result<T> ExceptionAsResult(Exception exception)
    {
        return (Result<T>)exception;
    }

    /// <summary>
    ///     Makes a task result from an exception.
    /// </summary>
    /// <param name="exception">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     A task result.
    /// </returns>
    public static Task<Result<T>> ExceptionAsTaskResult(Exception exception)
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
        this.either.IfRight(onError);
    }

    /// <summary>
    ///     Executes the specified function if the Result has failed.
    /// </summary>
    /// <param name="onError">
    ///     The function to be executed if the Result has failed. It takes an Exception as a parameter and
    ///     returns Unit.
    /// </param>
    public Unit IfFailed(Func<Exception?, Unit> onError)
    {
        return this.either.IfRight(onError);
    }

    /// <summary>
    ///     Calls a function to return a <typeparamref name="T" /> if there is an error;
    ///     returns the success value if no error.
    /// </summary>
    /// <param name="onError">
    ///     The function to call if there was an error.
    /// </param>
    /// <returns>
    ///     Either a <typeparamref name="T" /> representing either the success value,
    ///     or the value created by <paramref name="onError" />.
    /// </returns>
    /// <remarks>
    ///     Sample use case: fallback value.
    /// </remarks>
    public T? IfFailed(Func<Exception?, T> onError)
    {
        return this.either.IfRight(onError);
    }

    /// <summary>
    ///     Calls a function if there is an error.
    /// </summary>
    /// <param name="onError">
    ///     The error callback function.
    /// </param>
    public Task IfFailedAsync(Func<Exception?, Task> onError)
    {
        return this.either.IfRightAsync(onError);
    }

    /// <summary>
    ///     Calls a function if there is an error.
    /// </summary>
    /// <param name="onError">
    ///     The error callback function.
    /// </param>
    public Task<Unit> IfFailedAsync(Func<Exception?, Task<Unit>> onError)
    {
        return this.either.IfRightAsync(onError);
    }

    /// <summary>
    ///     Calls a function to return a <typeparamref name="T" /> if there is an error;
    /// </summary>
    /// <param name="onError">
    ///     The function to call if there was an error.
    /// </param>
    /// <returns>
    ///     A <typeparamref name="T" />.
    /// </returns>
    public Task<T?> IfFailedAsync(Func<Exception?, Task<T?>> onError)
    {
        return this.either.IfRightAsync(onError);
    }

    /// <summary>
    ///     Calls the supplied function if the result succeeded.
    /// </summary>
    /// <param name="onSuccess">
    ///     The function to call on success.
    /// </param>
    public void IfSucceeded(Action<T?> onSuccess)
    {
        this.either.IfLeft(onSuccess);
    }

    /// <summary>
    ///     Executes the specified function if the result has succeeded.
    /// </summary>
    /// <param name="onSuccess">The function to execute if the result has succeeded.</param>
    /// <returns>A unit.</returns>
    public Unit IfSucceeded(Func<T?, Unit> onSuccess)
    {
        return this.either.IfLeft(onSuccess);
    }

    /// <summary>
    ///     Calls the supplied function if the result succeeded.
    /// </summary>
    /// <param name="onSuccess">
    ///     The function to call on success.
    /// </param>
    public Task IfSucceededAsync(Func<T?, Task> onSuccess)
    {
        return this.either.IfLeftAsync(onSuccess);
    }

    /// <summary>
    ///     Calls the supplied function if the result succeeded.
    /// </summary>
    /// <param name="onSuccess">
    ///     The function to call on success.
    /// </param>
    public Task<Unit> IfSucceededAsync(Func<T?, Task<Unit>> onSuccess)
    {
        return this.either.IfLeftAsync(onSuccess);
    }

    /// <summary>
    ///     Conditional value mapping function.
    /// </summary>
    /// <param name="onSuccess">
    ///     The function to call on success.
    /// </param>
    /// <param name="onError">
    ///     The function to call on error.
    /// </param>
    /// <typeparam name="TResult">
    ///     The result type.
    /// </typeparam>
    /// <returns>
    ///     The result.
    /// </returns>
    /// <remarks>
    ///     Calls the <paramref name="onError" /> function if there is an error;
    ///     otherwise, calls <paramref name="onSuccess" />.
    /// </remarks>
    public TResult? Map<TResult>(Func<T?, TResult?> onSuccess, Func<Exception?, TResult?> onError)
    {
        return this.either.Map(onSuccess, onError);
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
    public void Match(Action<T?> onSuccess, Action<Exception?> onError)
    {
        this.either.Match(onSuccess, onError);
    }

    /// <summary>
    ///     Matches the result and invokes the appropriate action based on whether the result is a success or failure.
    /// </summary>
    /// <param name="onSuccess">The function to invoke if the result is a success.</param>
    /// <param name="onFailure">The function to invoke if the result is a failure.</param>
    /// <returns>An instance of <see cref="Unit" />.</returns>
    public Unit Match(Func<T?, Unit> onSuccess, Func<Exception?, Unit> onFailure)
    {
        return this.either.Map(onSuccess, onFailure);
    }

    /// <summary>
    ///     Makes a <see cref="Result{T}" /> from a <typeparamref name="T" />.
    /// </summary>
    /// <param name="value">
    ///     The <typeparamref name="T" />.
    /// </param>
    /// <returns>
    ///     A <see cref="Result{T}" />.
    /// </returns>
    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    /// <summary>
    ///     Makes a <see cref="Result{T}" /> from a <typeparamref name="T" />.
    /// </summary>
    /// <param name="exception">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     A <see cref="Result{T}" />.
    /// </returns>
    public static implicit operator Result<T>(Exception exception)
    {
        return new Result<T>(exception);
    }

    /// <summary>
    ///     Implicitly converts a <see cref="Result{T}" /> to a <typeparamref name="T" />.
    /// </summary>
    /// <param name="result">
    ///     The result.
    /// </param>
    /// <returns>
    ///     A <typeparamref name="T" />.
    /// </returns>
    public static implicit operator T?(Result<T> result)
    {
        return result.either;
    }

    /// <summary>
    ///     Implicitly converts a <see cref="Result{T}" /> to an <see cref="Exception"/>.
    /// </summary>
    /// <param name="result">
    ///     The result.
    /// </param>
    /// <returns>
    ///     A <typeparamref name="T" />.
    /// </returns>
    public static implicit operator Exception?(Result<T> result)
    {
        return result.either;
    }
}