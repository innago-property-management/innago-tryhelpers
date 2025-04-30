#nullable enable
namespace Innago.Shared.TryHelpers;

using System;
using System.Threading.Tasks;

/// <summary>
/// Provides extension methods for units.
/// </summary>
public static class UnitExtensions
{
    /// <summary>
    /// Converts an <see cref="Action{T}"/> to a <see cref="Func{T, Unit}"/>.
    /// </summary>
    /// <param name="action">The action to convert.</param>
    /// <typeparam name="T">The type of the parameter for the action.</typeparam>
    /// <returns>A function that, when called, executes the action and returns <see cref="Unit.Default"/>.</returns>
    public static Func<T?, Unit> AsFunc<T>(this Action<T?> action)
    {
        return t =>
        {
            action(t);
            return Unit.Default;
        };
    }

    /// <summary>
    /// Converts an <see cref="Action"/> to a <see cref="Func{Unit}"/>.
    /// </summary>
    /// <param name="action">The action to convert.</param>
    /// <returns>A function that, when called, executes the action and returns <see cref="Unit.Default"/>.</returns>
    public static Func<Unit> AsFunc(this Action action)
    {
        return () =>
        {
            action();
            return Unit.Default;
        };
    }

    /// <summary>
    /// Converts a <see cref="Task"/> to a <see cref="Task{Unit}"/> and awaits the task.
    /// </summary>
    /// <param name="task">The task to convert and await.</param>
    /// <returns>A <see cref="Task{Unit}"/>.</returns>
    public static async Task<Unit> AsUnitTask(this Task task)
    {
        await task.ConfigureAwait(false);
        return Unit.Default;
    }

    /// <summary>
    /// Converts a `Func&lt;T>` to a `Func&lt;Task&lt;Unit>`.
    /// </summary>
    /// <param name="func">The source <see cref="Func{Task}"/> delegate.</param>
    /// <returns>A `Func&lt;Task&lt;Unit>`.</returns>
    public static Func<Task<Unit>> AsFuncTaskUnit(this Func<Task> func)
    {
        return () =>
        {
            Task task = func();
            return task.AsUnitTask();
        };
    }

    /// <summary>
    /// Converts a `Func&lt;T,Task>` to a `Func&lt;T,Task&lt;Unit>`.
    /// </summary>
    /// <param name="func">The source <see cref="Func{Task}"/> delegate.</param>
    /// <returns>A `Func&lt;T,Task&lt;Unit>`.</returns>
    public static Func<T?, Task<Unit>> AsFuncTaskUnit<T>(this Func<T?, Task> func)
    {
        return t =>
        {
            Task task = func(t);
            return task.AsUnitTask();
        };
    }
}