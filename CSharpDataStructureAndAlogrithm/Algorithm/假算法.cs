using System.Runtime.CompilerServices;

namespace Algorithm;

//public record Car(string Brand, string Model) : ICar<Car>
//{
//    public static string Brand { get; set; } = "Toyota";
//    public static string Model { get; set; } = "Corolla";
//    public static int Year { get; set; } = 2021;
//    public string LicensePlate { get; set; } = string.Empty;
//}

//public class CarFactoryM<T> : Factory<Car>
//{
//    public override Car Create()
//    {
//        return new Car();
//    }
//}

public class 假算法
{
    public static IEnumerable<string> RunTimer(CancellationToken token = default)
    {
        bool _isTicking = false;

        while (!token.IsCancellationRequested)
        {
            // Atomic toggle acting as a clock pulse
            if (!Interlocked.Exchange(ref _isTicking, !_isTicking))
            {
                yield return $"{DateTime.UtcNow:HH:mm:ss.fff}";
            }
        }
    }
}
