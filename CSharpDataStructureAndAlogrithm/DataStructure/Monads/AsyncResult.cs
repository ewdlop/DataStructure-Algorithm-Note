// AsyncResult.cs - Async Result monad implementation
using DataStructure.Monads;

public class AsyncResult<T>
{
    private readonly Task<Result<T>> _task;

    private AsyncResult(Task<Result<T>> task)
    {
        _task = task;
    }

    public static AsyncResult<T> FromTask(Task<Result<T>> task)
        => new(task);

    public static AsyncResult<T> FromResult(Result<T> result)
        => new(Task.FromResult(result));

    public async Task<Result<TResult>> BindAsync<TResult>(
        Func<T, Task<Result<TResult>>> func)
    {
        var result = await _task;
        return result.IsSuccess
            ? await func(result.Value)
            : Result<TResult>.Failure(result.Error);
    }
}
