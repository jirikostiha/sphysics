using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Solid angle quantity, a dimensionless ratio of an area to the square of a length measured in steradians.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Solid_angle">wikipedia</a>
/// </remarks>
public static class SolidAngle
{
    public const string Name = "solid angle";
    public const string DefaultSymbol = "Ω";
    public const string Dimension = "1";

    /// <summary> Solid angle subtended by an area of a sphere, Omega = A / r^2. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromArea<N>(N area, N radius)
        where N : IDivisionOperators<N, N, N>, IMultiplyOperators<N, N, N>
        =>
        area / (radius * radius);

    /// <summary> Solid angle of a cone with the given half angle in rads, Omega = 2 pi (1 - cos(theta)). </summary>
    /// <remarks> Evaluated as 4 pi sin^2(theta / 2), which keeps the precision of narrow cones. </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromConeHalfAngle<N>(N halfAngle)
        where N : ITrigonometricFunctions<N>
    {
        N two = N.One + N.One;
        N sine = N.Sin(halfAngle / two);
        return two * two * N.Pi * sine * sine;
    }
}
