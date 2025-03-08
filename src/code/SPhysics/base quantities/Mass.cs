using System.Numerics;

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
    public static N FromVolume<N>(N volume, N density)
        where N : IMultiplyOperators<N, N, N>
        =>
        volume * density;

    /// <summary>
    /// Mass
    /// </summary>
    /// <remarks>
    /// <a href="https://math-physics-calc.com/gravitational-force-calculator">calculator</a>
    /// </remarks>
    public static N FromGravitation<N>(N force, N distance, N mass2, N gravitationConst)
        where N : IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>
        =>
        force * (distance * distance) / (mass2 * gravitationConst);
}
