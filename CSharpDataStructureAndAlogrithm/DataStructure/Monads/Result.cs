// Result.cs - Result monad implementation
namespace DataStructure.Monads;

public class Result<T>
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

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);

    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> func)
        => _isSuccess ? func(_value) : Result<TResult>.Failure(_error);

    public Result<TResult> Map<TResult>(Func<T, TResult> func)
        => _isSuccess ? Result<TResult>.Success(func(_value)) : Result<TResult>.Failure(_error);

    public bool IsSuccess => _isSuccess;
    public string Error => _error;
    public T Value => _value;
}