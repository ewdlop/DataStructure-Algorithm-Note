namespace DataStructure.Monads;


public struct ValueMaybe<T>;

#if false
{
    public struct ValueMaybe<T> : IMonad<T>
    {
#if false
        {
            public IMonad<TResult?> Bind<TResult>(Func<T?, IMonad<TResult?>> func)
            {
                throw new NotImplementedException();
            }
            public T? GetValueOrDefault(T? defaultValue)
            {
                throw new NotImplementedException();
            }
            public IMonad<TResult?> Map<TResult>(Func<T?, TResult?> func)
            {
                throw new NotImplementedException();
            }
            public bool TryGetValue(out T? value)
            {
                throw new NotImplementedException();
            }
        }
#endif
    }
}
#endif