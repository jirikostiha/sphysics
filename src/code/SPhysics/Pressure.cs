using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>Pressure quantity</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Pressure">wikipedia</a></remarks>
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

    /// <summary> Hydrostatic pressure of a fluid at rest at the given depth, <c>rho * g * h</c>. </summary>
    /// <remarks>
    /// <a href="https://en.wikipedia.org/wiki/Hydrostatics#Hydrostatic_pressure">wikipedia</a>.
    /// Gauge pressure of an incompressible fluid in a uniform gravity; add the pressure at the surface for the absolute one.
    /// </remarks>
    /// <param name="fluidDensity"> Density of the fluid. </param>
    /// <param name="gravity"> Gravitational acceleration. </param>
    /// <param name="depth"> Depth below the surface. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Hydrostatic<N>(N fluidDensity, N gravity, N depth)
        where N : IMultiplyOperators<N, N, N>
        =>
        fluidDensity * gravity * depth;
}
