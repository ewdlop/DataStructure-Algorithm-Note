using System.Numerics;

namespace Algorithm;

public static class IntegerExtension
{
    public static int GreatestCommonDivisor(this int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static int LeastCommonMultiple(this int a, int b)
    {
        return a * b / a.GreatestCommonDivisor(b);
    }


    public static BigInteger Factorial(this int n)
    {
        if (n == 0 || n == 1) return 1;
        BigInteger result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }

    public static BigInteger GrowingFactorial(this int n)
    {
        int result = 1;
        for (int i = 0; i < n; i++)
        {
            result *= (n + i);
        }
        return result;
    }
}
