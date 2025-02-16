// AsyncResult.cs - Async Result monad implementation
namespace DataStructure.Monads
{
    public abstract class AsyncMonadBase<T> : IAsyncMonad<T>
    {
        public abstract Task<IMonad<TResult?>> BindAsync<TResult>(Func<T?, Task<IMonad<TResult?>>> func);
        public abstract Task<IMonad<TResult?>> MapAsync<TResult>(Func<T?, Task<TResult?>> func);
        public abstract IMonad<T?> ToSyncMonad();
        public abstract Task<IMonad<T?>> ToSyncMonadAsync(CancellationToken cancellationToken);
    }
}
