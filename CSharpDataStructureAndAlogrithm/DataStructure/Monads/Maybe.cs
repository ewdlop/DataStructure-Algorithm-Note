namespace DataStructure.Monads;

public class Maybe<T>
{
    private readonly T _value;
    private readonly bool _hasValue;

    private Maybe(T value, bool hasValue)
    {
        _value = value;
        _hasValue = hasValue;
    }

    public static Maybe<T> Some(T value) => new(value, true);
    public static Maybe<T> None() => new(default!, false);

    public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> func)
        => _hasValue ? func(_value) : Maybe<TResult>.None();

    public Maybe<TResult> Map<TResult>(Func<T, TResult> func)
        => _hasValue ? Maybe<TResult>.Some(func(_value)) : Maybe<TResult>.None();

    public T GetValueOrDefault(T defaultValue)
        => _hasValue ? _value : defaultValue;

    public bool TryGetValue(out T value)
    {
        value = _value;
        return _hasValue;
    }
}