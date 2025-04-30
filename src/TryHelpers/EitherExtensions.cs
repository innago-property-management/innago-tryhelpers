#nullable enable
namespace Innago.Shared.TryHelpers;

using System;

/// <summary>
///     Provides extension methods for the Either class.
/// </summary>
public static class EitherExtensions
{
    /// <summary>
    ///     Converts a function to an Either with the left type assigned.
    /// </summary>
    /// <param name="func">The function to convert.</param>
    /// <typeparam name="TInput">The input type of the function.</typeparam>
    /// <typeparam name="TLeft">The nullable left type of the Either.</typeparam>
    /// <typeparam name="TRight">The right type of the Either.</typeparam>
    /// <returns>A function that returns an Either with the converted function.</returns>
    public static Func<TInput?, Either<TLeft?, TRight>> ToEitherLeft<TInput, TLeft, TRight>(this Func<TInput?, TLeft?> func)
    {
        // ReSharper disable once ConvertClosureToMethodGroup
        return input => Factory(input);

        Func<TLeft?> Factory(TInput? input)
        {
            return () => func(input);
        }
    }

    /// <summary>
    ///     Converts a function to an Either with the right type assigned.
    /// </summary>
    /// <param name="func">The function to convert.</param>
    /// <typeparam name="TInput">The input type of the function.</typeparam>
    /// <typeparam name="TLeft">The left type of the Either.</typeparam>
    /// <typeparam name="TRight">The nullable right type of the Either.</typeparam>
    /// <returns>A function that returns an Either with the converted function.</returns>
    public static Func<TInput?, Either<TLeft, TRight?>> ToEitherRight<TInput, TLeft, TRight>(this Func<TInput?, TRight?> func)
    {
        // ReSharper disable once ConvertClosureToMethodGroup
        return input => Factory(input);

        Func<TRight?> Factory(TInput? input)
        {
            return () => func(input);
        }
    }
}