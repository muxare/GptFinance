namespace GptFinance.Domain;

public sealed class Maybe<T>
{
    private readonly T _value;
    public bool HasValue { get; }

    private Maybe()
    {
        HasValue = false;
    }

    private Maybe(T value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        _value = value;
        HasValue = true;
    }

    public static Maybe<T> Some(T value) => new Maybe<T>(value);

    public static Maybe<T> None => new Maybe<T>();

    public T Value
    {
        get
        {
            if (!HasValue)
                throw new InvalidOperationException("Maybe does not have a value.");

            return _value;
        }
    }

    public Maybe<TResult> Select<TResult>(Func<T, TResult> selector)
    {
        if (selector == null)
            throw new ArgumentNullException(nameof(selector));

        return HasValue ? Maybe<TResult>.Some(selector(_value)) : Maybe<TResult>.None;
    }

    public TResult GetValueOrFallback<TResult>(TResult fallbackValue)
    {
        if (fallbackValue == null)
            throw new ArgumentNullException(nameof(fallbackValue));

        return HasValue ? (TResult)Convert.ChangeType(_value, typeof(TResult)) : fallbackValue;
    }
}

public static class MaybeExtensions
{
    public static Maybe<T> ToMaybe<T>(this T value) where T : class
    {
        return value != null ? Maybe<T>.Some(value) : Maybe<T>.None;
    }

    public static Maybe<T> ToMaybe<T>(this T? value) where T : struct
    {
        return value.HasValue ? Maybe<T>.Some(value.Value) : Maybe<T>.None;
    }
}
