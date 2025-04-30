#nullable enable
namespace Innago.Shared.TryHelpers;

using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

/// <summary>
///     Unit is an alternative to void.
/// </summary>
/// <remarks>
///     It allows you to only write Func&lt;T> methods,
///     without needing to write an overload for Action&lt;T>.
/// </remarks>
[SuppressMessage("ReSharper", "UnusedParameter.Global")]
public record struct Unit : IComparable<Unit>
{
    /// <summary>
    ///     A Unit.
    /// </summary>
    /// <remarks>
    ///     All Unit instances are equal. There is no need to call new().
    /// </remarks>
    public static readonly Unit Default = new();

    /// <inheritdoc />
    [Pure]
    public int CompareTo(Unit other)
    {
        return 0;
    }

    /// <inheritdoc />
    [Pure]
    public bool Equals(Unit other)
    {
        return true;
    }

    /// <inheritdoc />
    [Pure]
    public override int GetHashCode()
    {
        return 0;
    }

    /// <summary>
    ///     Greater than operator.
    /// </summary>
    /// <param name="l">Left.</param>
    /// <param name="r">Right.</param>
    public static bool operator >(Unit l, Unit r)
    {
        return false;
    }

    /// <summary>
    ///     Greater than or equal to operator.
    /// </summary>
    /// <param name="l">Left.</param>
    /// <param name="r">Right.</param>
    public static bool operator >=(Unit l, Unit r)
    {
        return true;
    }

    /// <summary>
    ///     Less than operator.
    /// </summary>
    /// <param name="l">Left.</param>
    /// <param name="r">Right.</param>
    public static bool operator <(Unit l, Unit r)
    {
        return false;
    }

    /// <summary>
    ///     Less than or equal to operator.
    /// </summary>
    /// <param name="l">Left.</param>
    /// <param name="r">Right.</param>
    public static bool operator <=(Unit l, Unit r)
    {
        return true;
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString()
    {
        return "()";
    }
}