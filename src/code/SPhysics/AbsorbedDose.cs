namespace SPhysics;

/// <summary>
/// Absorbed dose of ionizing radiation.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Absorbed_dose">wikipedia</a>
/// </remarks>
public static class AbsorbedDose
{
    public const string Name = "absorbed dose of ionizing radiation";
    public const string DefaultSymbol = "D";
    public const string Dimension = Length.Dimension+"+2 "+Time.Dimension+"-2"; //J * kg-1
}
