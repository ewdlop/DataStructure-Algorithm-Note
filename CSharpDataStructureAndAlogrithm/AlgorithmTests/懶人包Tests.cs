namespace AlgorithmTests;

public class 懶人包Tests : TestBase
{
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
}