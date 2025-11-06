using System.Numerics;

namespace Algorithm;

public abstract record QuarternionBase<T>(T? Real, T? X, T? Y, T? Z) where T : struct, INumber<T>
{
    /// <summary>
    /// Norm of any generic type?
    /// </summary>
    /// public T Norm

    public virtual QuarternionBase<T> ToReal => this with { X = T.Zero, Y = T.Zero, Z = T.Zero };

    public virtual QuarternionBase<T> ToImaginary => this with { Real = T.Zero };

    public virtual QuarternionBase<T> ToConjugate => this with { X = -X, Y = -Y, Z = -Z };

    public virtual QuarternionBase<T> ToInverse()
    {
        T? normSquared = Real * Real + X * X + Y * Y + Z * Z;
        return this with { Real = Real / normSquared, X = -X / normSquared, Y = -Y / normSquared, Z = -Z / normSquared };
    }

    public abstract T? Norm();
    public abstract T? Half();
    public abstract T? NegativeOne();


    public virtual QuarternionBase<T> ToPlus => this with { Real = Real + Z, X = X - Y, Y = T.Zero, Z = T.Zero } * this with {Real = Half(), X = T.Zero, Y = T.Zero, Z = Half()};

    public virtual QuarternionBase<T> ToMinus => this with {Real = Real - Z, X = X + Y, Y = T.Zero, Z = T.Zero } * this with { Real = Half(), X = T.Zero, Y = T.Zero, Z = NegativeOne() * Half() };


    public static QuarternionBase<T> operator +(QuarternionBase<T> p, QuarternionBase<T> q) => p with { Real = p.Real + q.Real, X = p.X + q.X, Y = p.Y + q.Y, Z = p.Z + q.Z };

    public static QuarternionBase<T> operator -(QuarternionBase<T> p, QuarternionBase<T> q) => p with { Real= p.Real - q.Real, X= p.X - q.X, Y= p.Y - q.Y, Z= p.Z - q.Z };

    public static QuarternionBase<T> operator *(QuarternionBase<T> p, QuarternionBase<T> q)
    {
        return p with
        {
            Real = p.Real * q.Real - p.X * q.X - p.Y * q.Y - p.Z * q.Z,
            X = p.Real * q.X + p.X * q.Real + p.Y * q.Z - p.Z * q.Y,
            Y = p.Real * q.Y - p.X * q.Z + p.Y * q.Real + p.Z * q.X,
            Z = p.Real * q.Z + p.X * q.Y - p.Y * q.X + p.Z * q.Real
        };
    }

    public static QuarternionBase<T> operator *(QuarternionBase<T> p, T scalar) => p with { Real = p.Real * scalar, X = p.X * scalar, Y = p.Y * scalar, Z = p.Z * scalar };

    public static QuarternionBase<T> operator *(T scalar, QuarternionBase<T> p) => p with { Real = p.Real * scalar, X = p.X * scalar, Y = p.Y * scalar, Z = p.Z * scalar };

    //public static QuarternionBase<T> operator *(QuarternionBase<T> p, System.Single scalar) => p with { Real = p.Real * scalar, X = p.X * scalar, Y = p.Y * scalar, Z = p.Z * scalar };

    //public static QuarternionBase<T> operator *(System.Single scalar, QuarternionBase<T> p) => p with { Real = p.Real * scalar, X = p.X * scalar, Y = p.Y * scalar, Z = p.Z * scalar };

    //public static QuarternionBase<T> operator *(System.Int64 scalar, QuarternionBase<T> p) => p with { Real = p.Real * scalar, X = p.X * scalar, Y = p.Y * scalar, Z = p.Z * scalar };

    //public static QuarternionBase<T> operator *(QuarternionBase<T> p, System.Int64 scalar) => p with { Real = p.Real * scalar, X = p.X * scalar, Y = p.Y * scalar, Z = p.Z * scalar };

    //public static QuarternionBase<T> operator *(QuarternionBase<T> p, System.Int32 scalar) => p with { Real = p.Real * scalar, X = p.X * scalar, Y = p.Y * scalar, Z = p.Z * scalar };

    //public static QuarternionBase<T> operator *(System.Int32 scalar, QuarternionBase<T> p) => p with { Real = p.Real * scalar, X = p.X * scalar, Y = p.Y * scalar, Z = p.Z * scalar };


    public virtual T? Distance(QuarternionBase<T> q) => (this - q).Norm();

    public virtual T? Dot(QuarternionBase<T> q) => Half() * (this * q.ToConjugate + q * ToConjugate).Real;

    /// <summary>
    /// <seealso cref="https://www.johndcook.com/blog/2012/02/15/dot-cross-and-quaternion-products/#:~:text=quaternion%20product%20%3D%20cross%20product%20%E2%88%92%20dot,what%20the%20equation%20above%20means.&text=i2%20%3D%20j2%20%3D%20k,i%2C%20j%2C%20and%20k."/>
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    public virtual QuarternionBase<T>? Cross(QuarternionBase<T> q) => this with
    {
        Real = NegativeOne() * (X * q.X + Y * q.Y + Z * q.Z),
        X = Y * q.Z - Z * q.Y,
        Y = Z * q.X - X * q.Z,
        Z = X * q.Y - Y * q.X
    };
}
