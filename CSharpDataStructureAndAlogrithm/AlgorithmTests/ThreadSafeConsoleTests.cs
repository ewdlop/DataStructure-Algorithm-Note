using System.Collections;

namespace AlgorithmTests;

[TestClass()]
public class ThreadSafeConsoleTests : TestBase
{
    public override IDictionary Properties => throw new NotImplementedException();

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

    [TestMethod()]
    public void WriteLineTest()
    {
        //Assert.Fail();
        Assert.Inconclusive();
    }
}
