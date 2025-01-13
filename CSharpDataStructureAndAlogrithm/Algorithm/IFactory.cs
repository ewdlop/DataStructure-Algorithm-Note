namespace Algorithm;


public interface IFactory<T>
{

}

public abstract class Factory<T> : IFactory<T>
{
    public abstract T Create();
}


public interface ICar<T> where T : ICar<T>
{
    public static abstract string Brand { get; set; }
}

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