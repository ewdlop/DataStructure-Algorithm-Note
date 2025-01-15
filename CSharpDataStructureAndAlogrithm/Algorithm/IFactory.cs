namespace Algorithm;

//public enum Brand
//{
//    None = 0,
//    Toyota = 1,
//    Honda = 2,
//    Ford = 3,
//}

public interface IFactory<T>
{

}

public abstract class Factory<T> : IFactory<T> //where T : ICar<T>
{
    public abstract T Create();
}

public interface ICarFactory<T> : IFactory<T>
{
    //public abstract ICar<T> Create();
    //public abstract T Create();
}

public abstract class CarFactoryBase<T> : ICarFactory<T>
{
    //public abstract ICar<T> Create();
    //public abstract T Create();
}

public interface ICar<T>// where T : ICar<T>
{
    public static abstract string Brand { get; set; }
    public static abstract string Model { get; set; }
    public static abstract int Year { get; set; }
    //public static abstract string BrandName { get; init; }
    //public static abstract string ModelName { get; init; }
    //public static abstract T Value { get; set; }
}

//public record Car<T>(/*LicensePlate LicensePlateNumber*/) : ICar<Car<T>>
//{
//    public static string Brand { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//    public static string Model { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//    public static int Year { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//}

//public record ToyotaCar<T>() : Car<T>()
//{
//    public static string Brand { get; set; } = "Toyota";
//    public static string Model { get; set; }
//}

//public record Car(string Brand, string Model) : ICar<Car>
//{
//    public static string? Brand { get; set; } = Brand ?? "Toyota";
//    public static string? Model { get; set; } = Model ?? "Corolla";
//    public static int Year { get; set; } = 2021;
//    public string LicensePlate { get; set; } = string.Empty;
//}

//public record LicensePlate(string LicensePlateNumber)
//{
//    public string LicensePlateNumber { get; init; } = LicensePlateNumber;
//}

//public class CarFactory<ICar<T>> : Factory<ICar<T>> where T: Car
//{
//    public override Car Create()
//    {
//        return new Car();
//    }
//}

//public class CarFactoryM<T> : Factory<Car>
//{
//    public override Car Create()
//    {
//        return new Car();
//    }
//}

//public class Carol
//{

//}