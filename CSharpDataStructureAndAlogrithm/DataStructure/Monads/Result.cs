// Result.cs - Result monad implementation
namespace DataStructure.Monads;

public class Result<T> : MonadBase<T>
{
    private readonly T _value;
    private readonly string _error;
    private readonly bool _isSuccess;

    private Result(T value)
    {
        _value = value;
        _isSuccess = true;
        _error = string.Empty;
    }

    private Result(string error)
    {
        _value = default!;
        _isSuccess = false;
        _error = error;
    }

    public static Result<T?> Success(T? value) => new(value);
    public static Result<T?> Failure(string error) => new(error);

    public virtual Result<TResult?> Bind<TResult>(Func<T, Result<TResult?>> func)
        => _isSuccess ? func(_value) : Result<TResult?>.Failure(_error);

    public override IMonad<TResult?> Bind<TResult>(Func<T, IMonad<TResult?>> func) where TResult : default
        => _isSuccess ? func(_value) : Result<TResult?>.Failure(_error);

    public override Result<TResult?> Map<TResult>(Func<T, TResult?> func) where TResult : default
        => _isSuccess ? Result<TResult?>.Success(func(_value)) : Result<TResult>.Failure(_error);

    public override T? GetValueOrDefault(T? defaultValue)
    {
        return _isSuccess? _value : defaultValue;
    }

    public override bool TryGetValue(out T? value)
    {
        if (_isSuccess)
        {
            value = _value;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public bool IsSuccess => _isSuccess;
    public string Error => _error;
    public T Value => _value;
}