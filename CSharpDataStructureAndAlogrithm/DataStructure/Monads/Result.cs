// Result.cs - Result monad implementation
namespace DataStructure.Monads;

public class Result<T> : MonadBase<T>
{
    private readonly T? _value;
    private readonly string _error;
    private readonly bool _isSuccess;

    protected Result(T? value)
    {
        _value = value;
        _isSuccess = true;
        _error = string.Empty;
    }

    protected Result(string error)
    {
        _value = default!;
        _isSuccess = false;
        _error = error;
    }

    protected Result(T? value, bool isSuccess)
    {
        _value = value;
        _isSuccess = isSuccess;
        _error = string.Empty;
    }

    protected Result(T? value, string error)
    {
        _value = value;
        _isSuccess = value is not null;
        _error = error;
    }

    protected Result(T? value, bool isSuccess, string error)
    {
        _value = value;
        _isSuccess = isSuccess;
        _error = error;
    }


    protected Result(bool isSuccess, string error)
    {
        _value = default;
        _isSuccess = isSuccess;
        _error = error;
    }

    public static Result<T?> Success(T? value) => new(value);
    public static Result<T?> Failure(string error) => new(error);

    public virtual Result<TResult?> Bind<TResult>(Func<T?, Result<TResult?>> func)
        => _isSuccess ? func(_value) : Result<TResult?>.Failure(_error);

    public override IMonad<TResult?> Bind<TResult>(Func<T?, IMonad<TResult?>> func) where TResult : default
        => _isSuccess ? func(_value) : Result<TResult?>.Failure(_error);

    public override Result<TResult?> Map<TResult>(Func<T?, TResult?> func) where TResult : default
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

public class Result : Result<Object>
{
    protected Result(object? value, bool isSuccess) : base(value, isSuccess){}
}