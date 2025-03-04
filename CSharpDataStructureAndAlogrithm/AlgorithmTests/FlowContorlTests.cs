using System.Collections;

namespace AlgorithmTests;

[TestClass()]
public class FlowControlTests : TestBase
{
    public override IDictionary Properties => throw new NotImplementedException();

    public override void AddResultFile(string fileName)
    {
    }

    [TestMethod()]
    public virtual void RunTest()
    {
#if false
        while (true)
        {
            if(Random.Shared.Next(0, 九十六) < 四十八)
            {
                Assert.Fail();
            }
        }
#endif
        Assert.Inconclusive();
    }

    [TestMethod()]
    public virtual void RunTest2()
    {
        for (int i = 0; i < 128; i++)
        {
            Assert.AreNotSame(i, i + 1);
        }
    }

    [TestMethod()]
    public virtual void RunTest3()
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
