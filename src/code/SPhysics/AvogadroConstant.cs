namespace SPhysics;

/// <summary> Avogadro constant, exact by the definition of the mole. </summary>
/// <remarks><a href="https://physics.nist.gov/cgi-bin/cuu/Value?na">CODATA 2018, NIST</a></remarks>
public static class AvogadroConstant
{
    public const string Name = "Avogadro constant";
    public const string Dimension = AmountOfSubstance.Dimension + "-1";

    /// <summary> Value in mol^-1. </summary>
    public const double Value = 6.02214076e23;
}
