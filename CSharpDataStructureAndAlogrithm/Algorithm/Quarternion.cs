namespace Algorithm;

public record Quarternion(System.Double? Real, System.Double? X, System.Double? Y, System.Double? Z) : QuarternionBase<System.Double>(Real, X, Y, Z)
{
    public override double? Half() => 0.5;

    public override double? NegativeOne() => -1.0;

    public override System.Double? Norm()
    {
        if(Real is not null && X is not null && Y is not null && Z is not null)
        {
            return System.Math.Sqrt(Real.Value* Real.Value + X.Value * X.Value + Y.Value * Y.Value+ Z.Value * Z.Value);
        }
        return null;
    }


}
