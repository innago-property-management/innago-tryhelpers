#nullable enable
namespace Innago.Shared.TryHelpers;

using System;
using System.Threading.Tasks;

/// <summary>
///     Helper functions to make `try...catch` more functional.
/// </summary>
public static class TryHelpers
{
    /// <summary>
    ///     Tries to invoke a function.
    /// </summary>
    /// <param name="func">
    ///     The function to invoke.
    /// </param>
    /// <typeparam name="T">
    ///     The return type of the function to invoke.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="Result{T}" />.
    /// </returns>
    public static Result<T?> Try<T>(Func<T?> func)
    {
        Result<T?> result;

        try
        {
            result = func();
        }
        catch (Exception e)
        {
            result = e;
        }

        return result;
    }

    /// <summary>
    ///     Tries to invoke an action.
    /// </summary>
    /// <param name="action">
    ///     The action to invoke.
    /// </param>
    /// <returns>
    ///     A <see cref="Result" />.
    /// </returns>
    public static Result Try(Action action)
    {
        Result result;

        try
        {
            action();
            result = Result.Success;
        }
        catch (Exception e)
        {
            result = e;
        }

        return result;
    }

    /// <summary>
    ///     Tries to invoke an asynchronous function.
    /// </summary>
    /// <param name="func">
    ///     The function to invoke.
    /// </param>
    /// <typeparam name="T">
    ///     The return type of the function to invoke.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="Result{T}" />.
    /// </returns>
    public static async Task<Result<T?>> TryAsync<T>(Func<Task<T?>> func)
    {
        Result<T?> result;

        try
        {
            result = await func().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            result = e;
        }

        return result;
    }

    /// <summary>
    ///     Tries to invoke an asynchronous function.
    /// </summary>
    /// <param name="func">
    ///     The function to invoke.
    /// </param>
    /// <returns>
    ///     A <see cref="Result" />.
    /// </returns>
    public static async Task<Result> TryAsync(Func<Task> func)
    {
        Result result;

        try
        {
            await func().ConfigureAwait(false);
            result = Result.Success;
        }
        catch (Exception e)
        {
            result = e;
        }

        return result;
    }
}