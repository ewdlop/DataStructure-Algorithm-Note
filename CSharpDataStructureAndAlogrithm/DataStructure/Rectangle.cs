namespace DataStructure;

public class Rectangle
{
    public double MinX => X - Width;
    public double MinY => Y - Height;
    public double MaxX => X + Width;
    public double MaxY => Y + Height;

    public double X { get; private set; }
    public double Y { get; private set; }
    public double Width { get; private set; }
    public double Height { get; private set; }

    public Rectangle(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Rectangle(Rectangle other)
    {
        X = other.X;
        Y = other.Y;
        Width = other.Width;
        Height = other.Height;
    }

    public Rectangle(Point point, double width, double height)
    {
        X = point.X;
        Y = point.Y;
        Width = width;
        Height = height;
    }

    public Rectangle(Point point1, Point point2)
    {
        X = (point1.X + point2.X) / 2;
        Y = (point1.Y + point2.Y) / 2;
        Width = Math.Abs(point1.X - point2.X) / 2;
        Height = Math.Abs(point1.Y - point2.Y) / 2;
    }

    public Rectangle(Point point)
    {
        X = point.X;
        Y = point.Y;
        Width = 0;
        Height = 0;
    }

    public Rectangle(Point point1, Point point2, Point point3, Point point4)
    {
        X = (point1.X + point2.X + point3.X + point4.X) / 4;
        Y = (point1.Y + point2.Y + point3.Y + point4.Y) / 4;
        Width = Math.Max(Math.Max(point1.X, point2.X), Math.Max(point3.X, point4.X)) - X;
        Height = Math.Max(Math.Max(point1.Y, point2.Y), Math.Max(point3.Y, point4.Y)) - Y;
    }


    public bool Intersects(Rectangle other)
    {
        return !(other.MinX > MaxX || other.MaxX < MinX || other.MinY > MaxY || other.MaxY < MinY);
    }

    public bool Contains(Rectangle other)
    {
        return MinX <= other.MinX && MaxX >= other.MaxX && MinY <= other.MinY && MaxY >= other.MaxY;
    }

    public bool Contains(Point point)
    {
        return (point.X >= X - Width && point.X < X + Width &&
                point.Y >= Y - Height && point.Y < Y + Height);
    }

    public static Rectangle Union(Rectangle a, Rectangle b)
    {
        return new Rectangle(
            Math.Min(a.MinX, b.MinX),
            Math.Min(a.MinY, b.MinY),
            Math.Max(a.MaxX, b.MaxX),
            Math.Max(a.MaxY, b.MaxY)
        );
    }

    public double Area()
    {
        return (MaxX - MinX) * (MaxY - MinY);
    }
}