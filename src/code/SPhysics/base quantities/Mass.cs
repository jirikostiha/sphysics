using SMath.Functions1;

namespace SPhysics;

/// <summary>
/// Mass quantity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Mass">wikipedia</a>
/// </remarks>
public static class Mass
{
    public const string Name = "mass";
    public const string DefaultSymbol = "m";
    public const string Dimension = "M";

    //from average density
    public static double FromVolume(double volume, double density) => volume * density;

    /// <summary>
    /// Mass
    /// </summary>
    /// <remarks>
    /// <a href="https://math-physics-calc.com/gravitational-force-calculator">calculator</a>
    /// </remarks>
    public static double FromGravitation(double force, double distance, double mass2, double gravitationConst)
        => force * Power2.Eval(distance) / (mass2 * gravitationConst);
}
