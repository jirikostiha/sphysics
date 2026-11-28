namespace SPhysics;

/// <summary> Boltzmann constant, exact by the definition of the kelvin. </summary>
/// <remarks><a href="https://physics.nist.gov/cgi-bin/cuu/Value?k">CODATA 2018, NIST</a></remarks>
public static class BoltzmannConstant
{
    public const string Name = "Boltzmann constant";
    public const string DefaultSymbol = "k";
    public const string Dimension = Energy.Dimension + " " + Temperature.Dimension + "-1";

    /// <summary> Value in J K^-1. </summary>
    public const double Value = 1.380649e-23;
}
