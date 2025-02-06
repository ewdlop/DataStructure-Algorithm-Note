using System.Runtime.InteropServices;

namespace Algorithm;

// Result type for TLB operations
public readonly struct TLBResult<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private TLBResult(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static TLBResult<T> Success(T value) => new(true, value, null);
    public static TLBResult<T> Failure(string error) => new(false, default, error);
}

// Memory alignment information
public readonly struct AlignmentInfo
{
    public int RawSize { get; }
    public int AlignedSize { get; }
    public int PaddingSize { get; }

    public AlignmentInfo(int rawSize, int alignmentSize)
    {
        RawSize = rawSize;
        AlignedSize = (rawSize + alignmentSize - 1) / alignmentSize * alignmentSize;
        PaddingSize = AlignedSize - rawSize;
    }

    public override string ToString() =>
        $"Raw Size: {RawSize} bytes, Aligned Size: {AlignedSize} bytes, Padding: {PaddingSize} bytes";
}

// Cache entry with alignment information
public readonly record struct CacheEntry<TKey, TValue>(TKey Key, TValue Value, AlignmentInfo KeyAlignment, AlignmentInfo ValueAlignment) where TKey : struct
{
    public TKey Key { get; } = Key;
    public TValue Value { get; } = Value;
    public AlignmentInfo KeyAlignment { get; } = KeyAlignment;
    public AlignmentInfo ValueAlignment { get; } = ValueAlignment;
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public int TotalSize => KeyAlignment.AlignedSize + ValueAlignment.AlignedSize;

    public override string ToString() =>
        $"""
        === Cache Entry ===
        Key: {KeyAlignment}
        Value: {ValueAlignment}
        Total Size: {TotalSize} bytes
        Timestamp: {Timestamp:O}
        ==================
        """;
}

// TLB Configuration
public class TLBConfiguration
{
    public int Capacity { get; }
    public int AlignmentSize { get; }

    protected TLBConfiguration(int capacity, int alignmentSize)
    {
        Capacity = capacity;
        AlignmentSize = alignmentSize;
    }

    public static TLBResult<TLBConfiguration> Create(int capacity, int alignmentSize = 8)
    {
        if (capacity <= 0)
            return TLBResult<TLBConfiguration>.Failure("Capacity must be greater than 0");

        if (alignmentSize <= 0 || (alignmentSize & alignmentSize - 1) != 0)
            return TLBResult<TLBConfiguration>.Failure("Alignment size must be a positive power of 2");

        int alignedCapacity = IsPowerOfTwo(capacity) ? capacity : NextPowerOfTwo(capacity);
        return TLBResult<TLBConfiguration>.Success(new TLBConfiguration(alignedCapacity, alignmentSize));
    }

    protected static bool IsPowerOfTwo(int x) => (x & x - 1) == 0;

    protected static int NextPowerOfTwo(int x)
    {
        if (x < 1) return 1;
        x--;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        return x + 1;
    }
}

// Main TLB implementation
public class TranslationLookasideBuffer<TKey, TValue>(TLBConfiguration config)
        where TKey : struct
        where TValue : struct
{
    protected readonly TLBConfiguration _config = config;
    protected readonly Dictionary<TKey, CacheEntry<TKey, TValue>> _entries = new Dictionary<TKey, CacheEntry<TKey, TValue>>(config.Capacity);
    protected readonly LinkedList<TKey> _accessOrder = new LinkedList<TKey>();

    public TLBResult<TValue> TryGet(TKey key)
    {
        if (!_entries.TryGetValue(key, out var entry))
            return TLBResult<TValue>.Failure("Cache miss");

        UpdateAccessOrder(key);
        return TLBResult<TValue>.Success(entry.Value);
    }

    public virtual TLBResult<Unit> Add(TKey key, TValue value, int? keySize = null, int? valueSize = null)
    {
        keySize ??= GetDefaultSize<TKey>();
        if(keySize < GetDefaultSize<TKey>())
            return TLBResult<Unit>.Failure("Key size must be greater than or equal to the default size");
        TLBResult<int> keySizeResult = ValidateSize(keySize.Value);
        if (!keySizeResult.IsSuccess)
            return TLBResult<Unit>.Failure(keySizeResult.Error!);

        valueSize ??= GetDefaultSize<TValue>();
        if (valueSize < GetDefaultSize<TValue>())
            return TLBResult<Unit>.Failure("Value size must be greater than or equal to the default size");
        TLBResult<int> valueSizeResult = ValidateSize(valueSize.Value);
        if (!valueSizeResult.IsSuccess)
            return TLBResult<Unit>.Failure(valueSizeResult.Error!);

        CacheEntry<TKey, TValue> entry = new CacheEntry<TKey, TValue>(
            key,
            value,
            new AlignmentInfo(keySizeResult.Value, _config.AlignmentSize),
            new AlignmentInfo(valueSizeResult.Value, _config.AlignmentSize));

        if (!_entries.TryAdd(key, entry))
        {
            _entries[key] = entry;
            UpdateAccessOrder(key);
        }
        else
        {
            if (_entries.Count >= _config.Capacity)
                EvictLRU();
            _accessOrder.AddFirst(key);
        }

        Console.WriteLine($"Added entry:\n{entry}");
        return TLBResult<Unit>.Success(default);
    }

    protected virtual void UpdateAccessOrder(TKey key)
    {
        LinkedListNode<TKey>? node = _accessOrder.Find(key);
        if (node != null)
        {
            _accessOrder.Remove(node);
            _accessOrder.AddFirst(key);
        }
    }

    protected virtual void EvictLRU()
    {
        if (_accessOrder.Last != null)
        {
            _entries.Remove(_accessOrder.Last.Value);
            _accessOrder.RemoveLast();
        }
    }

    protected static TLBResult<int> ValidateSize(int size)
    {
        return size > 0
            ? TLBResult<int>.Success(size)
            : TLBResult<int>.Failure("Size must be greater than 0");
    }

    protected static int GetDefaultSize<T>()
    {
        if (typeof(T).IsValueType)
        {
            try
            {
                return Marshal.SizeOf<T>();
            }
            catch
            {
                return 16;
            }
        }
        return 16;
    }
}

// Unit type for operations that don't return a value
public readonly struct Unit
{
    public static Unit Value => default;
}

// Example usage
public class Program
{
    public static void Main()
    {
        TLBResult<TLBConfiguration> configResult = TLBConfiguration.Create(4);
        if (!configResult.IsSuccess)
        {
            Console.WriteLine($"Configuration error: {configResult.Error}");
            return;
        }

        if(configResult.Value is null) return;

        TranslationLookasideBuffer<int, int> tlb = new TranslationLookasideBuffer<int, int>(configResult.Value);

        // Add entries with explicit sizes
        tlb.Add(0x1000, 0xA000, 16, 24);
        tlb.Add(0x2000, 0xA200, 18, 28);

        // Try to get a value
        TLBResult<int> getResult = tlb.TryGet(0x1000);
        if (getResult.IsSuccess)
            Console.WriteLine($"Found: {getResult.Value}");
        else
            Console.WriteLine($"Error: {getResult.Error}");

        // Add with default sizes
        tlb.Add(0x3000, 0xA300);

        // Add another entry that might cause eviction
        tlb.Add(0x4000, 0xA400, 20, 26);

        // Try to get potentially evicted value
        getResult = tlb.TryGet(0x1000);
        if (getResult.IsSuccess)
            Console.WriteLine($"Found: {getResult.Value}");
        else
            Console.WriteLine($"Error: {getResult.Error}");
    }
}