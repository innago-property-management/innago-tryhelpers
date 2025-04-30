#nullable enable
namespace Innago.Shared.TryHelpers;

using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

/// <summary>
///     Provides extension methods for working with boolean values in a functional way.
/// </summary>
[PublicAPI]
public static class BooleanFunctionalExtensions
{
    /// <summary>
    ///     Executes the specified action if the boolean value is false.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onFalse">The action to execute if the boolean value is false.</param>
    public static void IfFalse(this bool value, Action onFalse)
    {
        value.IfFalse(onFalse.AsFunc());
    }

    /// <summary>
    ///     Executes the specified action if the boolean value is false.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onFalse">The function to execute if the boolean value is false.</param>
    public static Unit IfFalse(this bool value, Func<Unit> onFalse)
    {
        return value.Match(DoNothing, onFalse);
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the boolean value is false.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onFalse">The asynchronous function to execute if the boolean value is false.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task IfFalseAsync(this bool value, Func<Task> onFalse)
    {
        return value.IfFalseAsync(onFalse.AsFuncTaskUnit());
    }
    
    /// <summary>
    ///     Executes the specified asynchronous function if the boolean value is false.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onFalse">The asynchronous function to execute if the boolean value is false.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<Unit> IfFalseAsync(this bool value, Func<Task<Unit>> onFalse)
    {
        return value.MatchAsync(DoNothingAsync, onFalse);
    }

    /// <summary>
    ///     Executes the specified action if the boolean value is true.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onTrue">The action to execute if the boolean value is true.</param>
    public static void IfTrue(this bool value, Action onTrue)
    {
        value.IfTrue(onTrue.AsFunc());
    }

    /// <summary>
    ///     Executes the specified action if the boolean value is true.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onTrue">The function to execute if the boolean value is true.</param>
    public static Unit IfTrue(this bool value, Func<Unit> onTrue)
    {
        return value.Match(onTrue, DoNothing);
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the boolean value is true.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onTrue">The asynchronous function to execute if the boolean value is true.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task IfTrueAsync(this bool value, Func<Task> onTrue)
    {
        return value.IfTrueAsync(onTrue.AsFuncTaskUnit());
    }
    
    /// <summary>
    ///     Executes the specified asynchronous function if the boolean value is true.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onTrue">The asynchronous function to execute if the boolean value is true.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<Unit> IfTrueAsync(this bool value, Func<Task<Unit>> onTrue)
    {
        return value.Map(onTrue, DoNothingAsync)!;
    }

    /// <summary>
    ///     Puts the specified value through a condition and returns the result based on the outcome of the condition.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="value">The value to be evaluated.</param>
    /// <param name="onTrue">The function to be executed if the condition is true.</param>
    /// <param name="onFalse">The function to be executed if the condition is false.</param>
    /// <returns>The result of the condition evaluation.</returns>
    public static T? Map<T>(this bool value, Func<T?> onTrue, Func<T?> onFalse)
    {
        return value ? onTrue() : onFalse();
    }

    /// <summary>
    ///     Executes the specified action based on the boolean value.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onTrue">The action to execute if the boolean value is true.</param>
    /// <param name="onFalse">The action to execute if the boolean value is false.</param>
    public static void Match(this bool value, Action onTrue, Action onFalse)
    {
        value.Match(onTrue.AsFunc(), onFalse.AsFunc());
    }

    /// <summary>
    ///     Executes the specified action based on the boolean value.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onTrue">The function to execute if the boolean value is true.</param>
    /// <param name="onFalse">The function to execute if the boolean value is false.</param>
    public static Unit Match(this bool value, Func<Unit> onTrue, Func<Unit> onFalse)
    {
        return value ? onTrue() : onFalse();
    }

    /// <summary>
    ///     Executes the specified action if the boolean value is false.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onTrue">The function to execute if the boolean value is true.</param>
    /// <param name="onFalse">The function to execute if the boolean value is false.</param>
    public static Task MatchAsync(this bool value, Func<Task> onTrue, Func<Task> onFalse)
    {
        return value.MatchAsync(onTrue.AsFuncTaskUnit(), onFalse.AsFuncTaskUnit());
    }
    
    /// <summary>
    ///     Executes the specified action if the boolean value is false.
    /// </summary>
    /// <param name="value">The boolean value to evaluate.</param>
    /// <param name="onTrue">The function to execute if the boolean value is true.</param>
    /// <param name="onFalse">The function to execute if the boolean value is false.</param>
    public static Task<Unit> MatchAsync(this bool value, Func<Task<Unit>> onTrue, Func<Task<Unit>> onFalse)
    {
        return value.Map(onTrue, onFalse)!;
    }

    private static Unit DoNothing()
    {
        return Unit.Default;
    }

    private static Task<Unit> DoNothingAsync()
    {
        return Task.FromResult(Unit.Default);
    }
}