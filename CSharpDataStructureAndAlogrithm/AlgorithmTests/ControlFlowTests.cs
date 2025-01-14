namespace AlgorithmTests;

[TestClass()]
public class ControlFlowTests : TestBase
{
    [TestMethod()]
    public void RunTest()
    {
        while (true)
        {
            if (Random.Shared.Next(0, 九十六) < 四十八)
            {
                Assert.Fail();
            }
        }
    }

    [TestMethod()]
    public void RunTest2()
    {
        for (int i = 0; i < 128; i++)
        {
            Assert.AreNotSame(i, i + 1);
        }
    }

    [TestMethod()]
    public void RunTest3()
    {

    }
}