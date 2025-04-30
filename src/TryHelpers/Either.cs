#nullable enable

namespace Innago.Shared.TryHelpers;

using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

/// <summary>
///     An Either is a monad that contains exactly one value, either of type TLeft or of type TRight.
///
///     It can be used to make writing code that is more functional easier.
///
///     You can use it as a return type in switch expressions, e.g.
///     <code lang="C#">
///     Either&lt;string, int> either = obj switch
///     {
///         string s => s,
///         int n => n,
///         _ => default,
///     };
///     </code>
///
///     You can also utilize it to apply different functions that produce the same return type, e.g.
///     <code lang="C#">
///     int eitherMap = either.Map(StringHash, IntHash);
///     </code> 
/// </summary>
/// <typeparam name="TLeft">The left type.</typeparam>
/// <typeparam name="TRight">The right type.</typeparam>
/// <remarks>
///     It is invalid to use the same type for both type parameters.
/// </remarks>
[PublicAPI]
public record struct Either<TLeft, TRight>
{
    /// <summary>
    ///     An Either.
    /// </summary>
    /// <remarks>
    ///     All uninitialized Either values are equal. There is no need to call new.
    ///     The default Either is NEITHER TLeft nor TRight.
    /// </remarks>
    public static readonly Either<TLeft, TRight> Default;

    private readonly Lazy<TLeft?>? leftValueLazy;
    private readonly Lazy<TRight?>? rightValueLazy;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Either{TLeft,TRight}" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor results in a monad that is NEITHER TLeft nor TRight.
    /// </remarks>
    public Either()
    {
        if (InvalidTypePair)
        {
            throw new InvalidOperationException("TLeft and TRight should not be the same type");
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Either{TLeft, TRight}" /> class.
    /// </summary>
    /// <param name="leftValue">
    ///     The value.
    /// </param>
    /// <remarks>
    ///     This constructor create a LEFT-valued monad.
    /// </remarks>
    public Either(TLeft? leftValue) : this(() => leftValue)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Either{TLeft, TRight}" /> class.
    /// </summary>
    /// <param name="leftValueFn">
    ///     The value function.
    /// </param>
    /// <remarks>
    ///     This constructor create a LEFT-valued monad.
    /// </remarks>
    public Either(Func<TLeft?> leftValueFn)
    {
        this.leftValueLazy = new Lazy<TLeft?>(leftValueFn);
        this.IsLeft = true;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Either{TLeft, TRight}" /> class.
    /// </summary>
    /// <param name="rightValue">
    ///     The value.
    /// </param>
    /// <remarks>
    ///     This constructor create a RIGHT-valued monad.
    /// </remarks>
    public Either(TRight rightValue) : this(() => rightValue)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Either{TLeft, TRight}" /> class.
    /// </summary>
    /// <param name="rightValueFn">
    ///     The value function.
    /// </param>
    /// <remarks>
    ///     This constructor create a RIGHT-valued monad.
    /// </remarks>
    public Either(Func<TRight?> rightValueFn)
    {
        this.IsRight = true;
        this.rightValueLazy = new Lazy<TRight?>(rightValueFn);
    }

    /// <inheritdoc />
    public readonly bool Equals(Either<TLeft, TRight> other)
    {
        return (this.IsLeft && ((TLeft?)this)?.Equals((TLeft?)other) == true) ||
               (this.IsRight && ((TRight?)this)?.Equals((TRight?)other) == true) ||
               !(this.IsLeft || this.IsRight);
    }

    /// <summary>
    ///     Gets the IsLeft. True if instance is constructed/bound using <typeparamref name="TLeft" />.
    /// </summary>
    public bool IsLeft { get; }

    /// <summary>
    ///     Gets a value indicating whether the Either is neither TLeft nor TRight.
    /// </summary>
    /// <value><c>true</c> if the Either is neither TLeft nor TRight; otherwise, <c>false</c>.</value>
    public bool IsNeither => this is { IsLeft: false, IsRight: false };

    /// <summary>
    ///     Gets the IsRight. True if instance is constructed/bound using <typeparamref name="TRight" />.
    /// </summary>
    public bool IsRight { get; }

    private static bool InvalidTypePair => typeof(TLeft) == typeof(TRight);
    private readonly TLeft? LeftValue => this.leftValueLazy != null ? this.leftValueLazy.Value : default;
    private readonly TRight? RightValue => this.rightValueLazy != null ? this.rightValueLazy.Value : default;

    /// <inheritdoc />
    public readonly override int GetHashCode()
    {
        return (this.LeftValue, this.IsLeft, this.RightValue, this.IsRight).GetHashCode();
    }

    /// <summary>
    ///     Executes the function if the value is TLeft.
    /// </summary>
    /// <param name="onLeft">The function.</param>
    public void IfLeft(Action<TLeft?> onLeft)
    {
        this.IfLeft(onLeft.AsFunc());
    }

    /// <summary>
    ///     Executes the function if the value is TLeft.
    /// </summary>
    /// <param name="onLeft">The function.</param>
    public Unit IfLeft(Func<TLeft?, Unit> onLeft)
    {
        return this.Map(onLeft, DefaultValue<TRight?, Unit>);
    }

    /// <summary>
    ///     Executes the function if the value is TLeft.
    /// </summary>
    /// <param name="onLeft">The function.</param>
    public Task IfLeftAsync(Func<TLeft?, Task> onLeft)
    {
        return this.IfLeftAsync(onLeft.AsFuncTaskUnit());
    }

    /// <summary>
    ///     Executes the function if the value is TLeft.
    /// </summary>
    /// <param name="onLeft">The function.</param>
    public Task<Unit> IfLeftAsync(Func<TLeft?, Task<Unit>> onLeft)
    {
        return this.Map(onLeft, DoNothing<TRight, Unit>)!;
    }

    /// <summary>
    ///     Executes the function if the value is TLeft.
    /// </summary>
    /// <param name="onLeft">The function.</param>
    public Task<T?> IfLeftAsync<T>(Func<TLeft?, Task<T?>> onLeft)
    {
        return this.Map(onLeft, DoNothing<TRight?, T?>)!;
    }

    /// <summary>
    ///     Executes the function if the value is TRight.
    /// </summary>
    /// <param name="onRight">The function.</param>
    public void IfRight(Action<TRight?> onRight)
    {
        this.IfRight(onRight.AsFunc());
    }

    /// <summary>
    ///     Executes the function if the value is TRight.
    /// </summary>
    /// <param name="onRight">The function.</param>
    public Unit IfRight(Func<TRight?, Unit> onRight)
    {
        return this.Map(DefaultValue<TLeft?, Unit>, onRight);
    }

    /// <summary>
    ///     Executes the function if the value is TRight.
    /// </summary>
    /// <param name="onRight">The function.</param>
    public T? IfRight<T>(Func<TRight?, T> onRight)
    {
        return this.Map(DefaultValue<TLeft?, T?>, onRight);
    }

    /// <summary>
    ///     Executes the function if the value is TRight.
    /// </summary>
    /// <param name="onRight">The function.</param>
    public Task IfRightAsync(Func<TRight?, Task> onRight)
    {
        return this.IfRightAsync(onRight.AsFuncTaskUnit());
    }

    /// <summary>
    ///     Executes the function if the value is TRight.
    /// </summary>
    /// <param name="onRight">The function.</param>
    public async Task<Unit> IfRightAsync(Func<TRight?, Task<Unit>> onRight)
    {
        return await this.Map(DoNothing<TLeft?, Unit>, onRight)!;
    }

    /// <summary>
    ///     Executes the function if the value is TRight.
    /// </summary>
    /// <param name="onRight">The function.</param>
    public Task<T?> IfRightAsync<T>(Func<TRight?, Task<T?>> onRight)
    {
        return this.Map(DoNothing<TLeft?, T?>, onRight)!;
    }

    /// <summary>
    ///     Applies the corresponding function and returns its value.
    /// </summary>
    /// <param name="onLeft">The function to execute if TLeft.</param>
    /// <param name="onRight">The function to execute if TRight.</param>
    /// <typeparam name="T">The return type.</typeparam>
    /// <returns>The result of evaluating the corresponding function.</returns>
    public T? Map<T>(Func<TLeft?, T?> onLeft, Func<TRight?, T?> onRight)
    {
        return this.IsRight ? onRight(this.RightValue) : onLeft(this.LeftValue);
    }

    /// <summary>
    ///     Applies the corresponding function.
    /// </summary>
    /// <param name="onLeft">The function to execute if TLeft.</param>
    /// <param name="onRight">The function to execute if TRight.</param>
    public void Match(Action<TLeft?> onLeft, Action<TRight?> onRight)
    {
        this.Match(onLeft.AsFunc(), onRight.AsFunc());
    }

    /// <summary>
    ///     Applies the corresponding function.
    /// </summary>
    /// <param name="onLeft">The function to execute if TLeft.</param>
    /// <param name="onRight">The function to execute if TRight.</param>
    public Unit Match(Func<TLeft?, Unit> onLeft, Func<TRight?, Unit> onRight)
    {
        return this.Map(onLeft, onRight);
    }

    /// <summary>
    ///     Binds a <typeparamref name="TLeft" /> value.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     An <see cref="Either{TLeft,TRight}" />.
    /// </returns>
    public static implicit operator Either<TLeft, TRight>(TLeft value)
    {
        return new Either<TLeft, TRight>(value);
    }

    /// <summary>
    ///     Binds a <typeparamref name="TLeft" /> value.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     An <see cref="Either{TLeft,TRight}" />.
    /// </returns>
    public static implicit operator Either<TLeft, TRight>(Func<TLeft> value)
    {
        return new Either<TLeft, TRight>(value);
    }

    /// <summary>
    ///     Binds a <typeparamref name="TRight" /> value.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     An <see cref="Either{TLeft,TRight}" />.
    /// </returns>
    public static implicit operator Either<TLeft, TRight>(TRight value)
    {
        return new Either<TLeft, TRight>(value);
    }

    /// <summary>
    ///     Binds a <typeparamref name="TRight" /> value.
    /// </summary>
    /// <param name="value">
    ///     The value fn.
    /// </param>
    /// <returns>
    ///     An <see cref="Either{TLeft,TRight}" />.
    /// </returns>
    public static implicit operator Either<TLeft, TRight>(Func<TRight?> value)
    {
        return new Either<TLeft, TRight>(value);
    }

    /// <summary>
    ///     Implicitly casts the instance to type <typeparamref name="TLeft" />.
    /// </summary>
    /// <param name="either">
    ///     The <see cref="Either{TLeft,TRight}" />.
    /// </param>
    /// <returns>
    ///     The Left value, which will be `default` if the Right value was bound.
    /// </returns>
    public static implicit operator TLeft?(Either<TLeft, TRight> either)
    {
        return either.LeftValue;
    }

    /// <summary>
    ///     Implicitly casts the instance to type <typeparamref name="TRight" />.
    /// </summary>
    /// <param name="either">
    ///     The <see cref="Either{TLeft,TRight}" />.
    /// </param>
    /// <returns>
    ///     The Right value, which will be `default` if the Left value was bound.
    /// </returns>
    public static implicit operator TRight?(Either<TLeft, TRight> either)
    {
        return either.RightValue;
    }

    private static TOut? DefaultValue<TIn, TOut>(TIn _)
    {
        return default;
    }

    private static Task<TOut?> DoNothing<TIn, TOut>(TIn? _)
    {
        return Task.FromResult(default(TOut));
    }
}