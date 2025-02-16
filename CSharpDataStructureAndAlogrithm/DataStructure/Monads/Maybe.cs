namespace DataStructure.Monads;

public class Maybe<T> : MonadBase<T>
{
    public readonly T? _value;
    private readonly bool _hasValue;

    private Maybe(T? value, bool hasValue)
    {
        _value = value;
        _hasValue = hasValue;
    }

    public static Maybe<T?> Some(T? value) => new(value, true);

    public static Maybe<T?> None() => new(default!, false);

    public virtual Maybe<TResult?> Bind<TResult>(Func<T?, Maybe<TResult?>> func)
      => _hasValue ? func(_value) : Maybe<TResult?>.None();

    public override IMonad<TResult?> Bind<TResult>(Func<T?, IMonad<TResult?>> func) where TResult : default
        => _hasValue ? func(_value) : Maybe<TResult?>.None();

    public override Maybe<TResult?> Map<TResult>(Func<T?, TResult?> func) where TResult : default
        => _hasValue ? Maybe<TResult?>.Some(func(_value)) : Maybe<TResult?>.None();

    public override T? GetValueOrDefault(T? defaultValue)
        => _hasValue ? _value : defaultValue;

    public override bool TryGetValue(out T? value)
    {
        value = _value;
        return _hasValue;
    }
}