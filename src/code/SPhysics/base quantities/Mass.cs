using System.Numerics;
using System.Runtime.CompilerServices;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromGravitation<N>(N force, N distance, N mass2, N gravitationConst)
        where N : IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>
        =>
        force * (distance * distance) / (mass2 * gravitationConst);

    /// <summary> Mass of a body accelerated by the given force, from Newton's second law. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromForce<N>(N force, N acceleration)
        where N : IDivisionOperators<N, N, N>
        =>
        force / acceleration;
}
