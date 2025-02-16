namespace DataStructure.Monads;

public interface IMonad<T>
{
    public abstract IMonad<TResult?> Bind<TResult>(Func<T?, IMonad<TResult?>> func);
    public abstract IMonad<TResult?> Map<TResult>(Func<T?, TResult?> func);
    public abstract T? GetValueOrDefault(T? defaultValue);
    public abstract bool TryGetValue(out T? value);
}
