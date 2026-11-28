namespace SPhysics;

/// <summary> Planck constant, exact by the definition of the kilogram. </summary>
/// <remarks><a href="https://physics.nist.gov/cgi-bin/cuu/Value?h">CODATA 2018, NIST</a></remarks>
public static class PlanckConstant
{
    public const string Name = "Planck constant";
    public const string DefaultSymbol = "h";
    public const string Dimension = Mass.Dimension + " " + Length.Dimension + "+2 " + Time.Dimension + "-1";

    /// <summary> Value in J s. </summary>
    public const double Value = 6.62607015e-34;
}
