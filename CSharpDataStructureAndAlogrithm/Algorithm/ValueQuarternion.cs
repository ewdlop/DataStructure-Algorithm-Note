using System.Numerics;
using System.Runtime.InteropServices;

namespace Algorithm;

/// <summary>
/// <seealso href="https://github.com/ewdlop/System-Calls-Stuffs-Notes/blob/main/C%2B%2B/VeryBasicMath/Quaternion.cpp"/>
/// </summary>
/// <typeparam name="T"></typeparam>
//[StructLayout(LayoutKind.Explicit, Size = 16)] // 4 * 4 = 16 bytes
//[StructLayout(LayoutKind.Explicit, Size = 32)] // 8 * 4 = 32 bytes
public readonly record struct ValueQuarternion<T> where T : struct, INumber<T>
{
    public ValueQuarternion()
    {
        Real = T.Zero;
        X = T.Zero;
        Y = T.Zero;
        Z = T.Zero;

    }

    //[FieldOffset(sizeof(T))]
    public T Real { get; init; } = T.Zero;
    public T X { get; init; } = T.Zero;
    public T Y { get; init; } = T.Zero;
    public T Z { get; init; } = T.Zero;

    /// <summary>
    /// Norm of any generic type?
    /// </summary>
    /// public T Norm

    public ValueQuarternion<T> ToReal => this with { X = T.Zero, Y = T.Zero, Z = T.Zero };

    public ValueQuarternion<T> ToImaginary => this with { Real = T.Zero};

    public static ValueQuarternion<T> operator +(ValueQuarternion<T> p, ValueQuarternion<T> q) =>
        new()
        {
            Real = p.Real + q.Real,
            X = p.X + q.X,
            Y = p.Y + q.Y,
            Z = p.Z + q.Z
        };

    public static ValueQuarternion<T> operator -(ValueQuarternion<T> p, ValueQuarternion<T> q) =>
        new()
        {
            Real = p.Real - q.Real,
            X = p.X - q.X,
            Y = p.Y - q.Y,
            Z = p.Z - q.Z
        };

    public static ValueQuarternion<T> operator *(ValueQuarternion<T> p, ValueQuarternion<T> q) =>
        new()
        {
            Real = p.Real * q.Real - p.X * q.X - p.Y * q.Y - p.Z * q.Z,
            X = p.Real * q.X + p.X * q.Real + p.Y * q.Z - p.Z * q.Y,
            Y = p.Real * q.Y - p.X * q.Z + p.Y * q.Real + p.Z * q.X,
            Z = p.Real * q.Z + p.X * q.Y - p.Y * q.X + p.Z * q.Real
        };

    public static ValueQuarternion<T> operator *(ValueQuarternion<T> p, T scalar) =>
        new()
        {
            Real = p.Real * scalar,
            X = p.X * scalar,
            Y = p.Y * scalar,
            Z = p.Z * scalar
        };

    public static ValueQuarternion<T> operator *(T scalar, ValueQuarternion<T> p) =>
        new()
        {
            Real = p.Real * scalar,
            X = p.X * scalar,
            Y = p.Y * scalar,
            Z = p.Z * scalar
        };
}
