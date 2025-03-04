using System.Collections;

namespace AlgorithmTests;

[TestClass()]
public class ControlFlowTests : TestBase, IDisposable
{
    protected bool _dispose;

    public override IDictionary Properties => throw new NotImplementedException();

    public ControlFlowTests()
    {
    }

    [TestMethod()]
    public void RunTest()
    {
        //Mental model: Random.Shared.NextDouble() is [0, 1)
        //double.Epsilon is the smallest positive double value that is significant?

        //Math.Abs(1.0 - (1.0 + double.Epsilon) * Random.Shared.NextDouble()???

        //use double-floating point error to break the loop?
        while (Random.Shared.NextDouble() is >= 0.0 and < 1.0)
        {
            if(CancellationTokenSource.Token.IsCancellationRequested)
            {
                break;
            }
#if false
            if (Random.Shared.Next(0, 九十六) < 四十八)
            {
                Assert.Fail();
            }
#endif
            Assert.IsFalse(Random.Shared.Next(0, 九十六) < 四十八);
            Assert.IsTrue(Random.Shared.Next(0, 九十六) is >= 0 and < 九十六);
            Assert.IsTrue(Random.Shared.NextDouble() is >= double.Epsilon and < 1.0);
            Assert.IsFalse(Random.Shared.Next(0, 九十六) is < 0 or >= 九十六);
        }
    }

    
    public static IEnumerable<bool> RunTest2(CancellationToken? cancellationToken = default)
    {
        while (true)
        {
            if (cancellationToken is not null && cancellationToken.Value.IsCancellationRequested)
            {
                yield break;
            }
            yield return Random.Shared.Next(0, 九十六) is >= 0 and < 九十六;
        }
    }


    [TestMethod()]
    public void RunTest2()
    {
        foreach (bool item in RunTest2(CancellationTokenSource.Token))
        {
            Assert.IsTrue(item);
        }
        Assert.IsTrue(CancellationTokenSource.Token.IsCancellationRequested || true);
    }

    [TestMethod()]
    public void RunTest3()
    {
        Assert.Inconclusive();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_dispose)
        {
            if (disposing)
            {
                CancellationTokenSource.Cancel();
                CancellationTokenSource.Dispose();
            }

            Interlocked.Exchange(ref _dispose, true);
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public override void AddResultFile(string fileName)
    {

    }

    public override void Write(string? message)
    {

    }

    public override void Write(string format, params object?[] args)
    {
        
    }

    public override void WriteLine(string? message)
    {
 
    }

    public override void WriteLine(string format, params object?[] args)
    {

    }
}