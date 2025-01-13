namespace AlgorithmTests;

[TestClass()]
public class FlowControlTests
{
    [TestMethod()]
    public void RunTest()
    {
        while (true)
        {
            if(Random.Shared.Next(0, 96) < 48)
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
}