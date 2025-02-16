// Result.cs - Result monad implementation
namespace DataStructure.Monads;

//Option monad implementation
public class Option<T> : MonadBase<T>
{
    private readonly T? _value;
    private readonly bool _hasValue;
    
    protected Option(T? value, bool hasValue)
    {
        _value = value;
        _hasValue = hasValue;
    }

    public static Option<T?> Some(T? value) => new(value, true);
    public static Option<T?> None() => new(default!, false);
    public virtual Option<TResult?> Bind<TResult>(Func<T?, Option<TResult?>> func)
        => _hasValue ? func(_value) : Option<TResult?>.None();
    public override IMonad<TResult?> Bind<TResult>(Func<T?, IMonad<TResult?>> func) where TResult : default
        => _hasValue ? func(_value) : Option<TResult?>.None();
    public override Option<TResult?> Map<TResult>(Func<T?, TResult?> func) where TResult : default
        => _hasValue ? Option<TResult?>.Some(func(_value)) : Option<TResult?>.None();
    public override T? GetValueOrDefault(T? defaultValue)
    {
        return _hasValue ? _value : defaultValue;
    }
    public override bool TryGetValue(out T? value)
    {
        value = _value;
        return _hasValue;
    }
}

public class Option : Option<Object>
{
    public Option(object? value, bool hasValue) : base(value, hasValue){}
}