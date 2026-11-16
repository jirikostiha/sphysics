namespace SPhysics;

/// <summary> Newtonian constant of gravitation. </summary>
/// <remarks><a href="https://physics.nist.gov/cgi-bin/cuu/Value?bg">CODATA 2018, NIST</a></remarks>
public static class GravitationalConstant
{
    public const string Name = "Gravitational constant";
    public const string DefaultSymbol = "G";
    public const string Dimension = Length.Dimension + "+3 " + Mass.Dimension + "-1 " + Time.Dimension + "-2";

    /// <summary> Value in m^3 kg^-1 s^-2. </summary>
    public const double Value = 6.67430e-11;
}
