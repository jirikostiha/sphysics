using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics.CelestialMechanics;

/// <summary> Apsides, the points of an orbit nearest to and farthest from the central body. </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Apsis">wikipedia</a></remarks>
public static class Apsis
{
    /// <summary> Distance of the pericenter, the nearest point, a * (1 - e). </summary>
    /// <remarks> Holds on a hyperbolic orbit too, whose semi-major axis is negative. </remarks>
    /// <param name="semiMajorAxis"> Semi-major axis of the orbit. </param>
    /// <param name="eccentricity"> Orbital eccentricity. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Pericenter<N>(N semiMajorAxis, N eccentricity)
        where N : INumberBase<N>
        =>
        semiMajorAxis * (N.One - eccentricity);

    /// <summary> Distance of the apocenter, the farthest point, a * (1 + e). </summary>
    /// <remarks> Only an elliptic orbit, 0 &lt;= e &lt; 1, has one. </remarks>
    /// <param name="semiMajorAxis"> Semi-major axis of the orbit. </param>
    /// <param name="eccentricity"> Orbital eccentricity. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Apocenter<N>(N semiMajorAxis, N eccentricity)
        where N : INumberBase<N>
        =>
        semiMajorAxis * (N.One + eccentricity);
}
