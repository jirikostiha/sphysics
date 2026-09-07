using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Pressure quantity
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Pressure">wikipedia</a>
/// </remarks>
public static class Pressure
{
    public const string Name = "pressure";
    public const string DefaultSymbol = "p";
    public const string Dimension = Mass.Dimension + " " + Length.Dimension + "-1 " + Time.Dimension + "-2";

    /// <summary> Pressure as a force distributed over an area. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N force, N area)
        where N : IDivisionOperators<N, N, N>
        =>
        force / area;
}
