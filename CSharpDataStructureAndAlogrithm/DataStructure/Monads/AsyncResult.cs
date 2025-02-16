// AsyncResult.cs - Async Result monad implementation
using DataStructure.Monads;

namespace DataStructure.Monads
{

    public class AsyncResult<T> : AsyncMonadBase<T>
    {
        private readonly Task<Result<T?>> _task;

        private AsyncResult(Task<Result<T?>> task)
        {
            _task = task;
        }

        public static AsyncResult<T?> FromTask(Task<Result<T?>> task)
            => new(task);

        public static AsyncResult<T?> FromResult(Result<T?> result)
            => new(Task.FromResult(result));

        public virtual async Task<Result<TResult?>> BindAsync<TResult>(
            Func<T?, Task<Result<TResult?>>> func)
        {
            var result = await _task;
            return result.IsSuccess
                ? await func(result.Value)
                : Result<TResult?>.Failure(result.Error);
        }

        public override async Task<IMonad<TResult?>> BindAsync<TResult>(Func<T?, Task<IMonad<TResult?>>> func) where TResult : default
        {
            var result = await _task;
            return result.IsSuccess
                ? await func(result.Value)
                : Result<TResult?>.Failure(result.Error);
        }

        public override async Task<IMonad<TResult?>> MapAsync<TResult>(Func<T?, Task<TResult?>> func) where TResult : default
        {
            Result<T?> result = await _task;
            if (result.IsSuccess)
            {
                TResult? mappedValue = await func(result.Value);
                return Result<TResult?>.Success(mappedValue);
            }
            return Result<TResult?>.Failure(result.Error);
        }

        public override IMonad<T?> ToSyncMonad()
        {
            Result<T?> result = _task.Result;
            return result.IsSuccess
                ? Result<T>.Success(result.Value)
                : Result<T>.Failure(result.Error);
        }

        public override async Task<IMonad<T?>> ToSyncMonadAsync(CancellationToken cancellationToken = default)
        {
            Result<T?> result = await _task.WaitAsync(cancellationToken);
            return result.IsSuccess
                ? Result<T?>.Success(result.Value)
                : Result<T?>.Failure(result.Error);
        }
    }
}
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

    public async Task<Result<TResult?>> BindAsync<TResult>(
        Func<T, Task<Result<TResult?>>> func)
    {
        var result = await _task;
        return result.IsSuccess
            ? await func(result.Value)
            : Result<TResult?>.Failure(result.Error);
    }
}