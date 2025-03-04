using System.Collections;

namespace AlgorithmTests;

[TestClass()]
public class 懶人包Tests : TestBase
{
    public override IDictionary Properties => throw new NotImplementedException();

    public override void AddResultFile(string fileName)
    {

    }

    [TestMethod()]
    [DataRow(1)]
    public void RunTest(int 預判值)
    {
        const int 必值 = 2;
        Assert.AreNotEqual(必值, 預判值);
    }

    [TestMethod()]
    [DataRow(1,"1")]
    public void RunTest2(int 輸入值, string 預判字串)
    {
        int 閉值 = 輸入值;
        Lazy<string> 惰性求String = new(() => 閉值.ToString());
        string 惰性得String值 = 惰性求String.Value;
        Assert.AreEqual(惰性得String值, 預判字串);
    }

    public override void Write(string? message)
    {
        throw new NotImplementedException();
    }

    public override void Write(string format, params object?[] args)
    {
        throw new NotImplementedException();
    }

    public override void WriteLine(string? message)
    {
        throw new NotImplementedException();
    }

    public override void WriteLine(string format, params object?[] args)
    {
        throw new NotImplementedException();
    }
}