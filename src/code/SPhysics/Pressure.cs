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

    /// <summary>Pressure at the center of a self-gravitating sphere of uniform density, P = 3 G M^2 / (8 pi R^4).</summary>
    /// <remarks>
    /// Follows from the hydrostatic equilibrium dP/dr = -G m(r) rho / r^2 integrated from the surface, where
    /// m(r) = 4/3 pi rho r^3, giving P = 2/3 pi G rho^2 R^2, the same value.
    /// Holds for a non-rotating, Newtonian sphere in hydrostatic equilibrium with constant density.
    /// Real bodies are denser towards the center, so their central pressure is higher: the Earth reaches about
    /// 364 GPa against 170 GPa of the uniform sphere, the Sun 26.5 PPa against 0.13 PPa. Treat the result as a lower bound.
    /// <a href="https://en.wikipedia.org/wiki/Hydrostatic_equilibrium">wikipedia</a>,
    /// <a href="https://home.strw.leidenuniv.nl/~keller/Teaching/Planets_2010/Planets2010_E04.pdf">derivation for a constant-density planet</a>,
    /// <a href="https://tohline.education/SelfGravitatingFluids/index.php/SSC/VirialEquilibrium/UniformDensity">uniform-density sphere</a>,
    /// <a href="https://en.wikipedia.org/wiki/Solar_core">central pressure of the Sun</a>,
    /// <a href="https://www.jamstec.go.jp/e/about/press_release/archive/2010/20100405.pdf">central pressure of the Earth</a>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N CentralOfUniformSphere<N>(N mass, N radius, N gravitationalConstant)
        where N : IFloatingPointConstants<N>
    {
        N squaredRadius = radius * radius;
        return N.CreateTruncating(3) * gravitationalConstant * mass * mass
            / (N.CreateTruncating(8) * N.Pi * squaredRadius * squaredRadius);
    }
}
