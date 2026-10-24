using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics.CelestialMechanics;

/// <summary>Hill sphere, the region where the gravity of a body dominates over the tidal pull of the body it orbits.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Hill_sphere">wikipedia</a></remarks>
public static class HillSphere
{
    /// <summary>Radius of the Hill sphere of a body on a circular orbit, a * cbrt(m / (3 * M)).</summary>
    /// <param name="mass"> Mass of the orbiting body. </param>
    /// <param name="centralMass"> Mass of the central body. </param>
    /// <param name="semiMajorAxis"> Semi-major axis of the orbit. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass, N centralMass, N semiMajorAxis)
        where N : IRootFunctions<N>
        =>
        semiMajorAxis * N.Cbrt(mass / (N.CreateTruncating(3) * centralMass));

    /// <summary>Radius of the Hill sphere of a body on an elliptic orbit at its pericenter, a * (1 - e) * cbrt(m / (3 * M)).</summary>
    /// <param name="mass"> Mass of the orbiting body. </param>
    /// <param name="centralMass"> Mass of the central body. </param>
    /// <param name="semiMajorAxis"> Semi-major axis of the orbit. </param>
    /// <param name="eccentricity"> Eccentricity of the orbit. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass, N centralMass, N semiMajorAxis, N eccentricity)
        where N : IRootFunctions<N>
        =>
        Eval(mass, centralMass, semiMajorAxis * (N.One - eccentricity));
}
