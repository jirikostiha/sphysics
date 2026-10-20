using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics.CelestialMechanics;

/// <summary>
/// Orbital speed of a small body around a central body.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Orbital_speed">wikipedia</a>
/// </remarks>
public static class OrbitalVelocity
{
    /// <summary>
    /// Speed on a circular orbit, sqrt(G * M / r).
    /// </summary>
    /// <param name="mass"> Mass of the central body. </param>
    /// <param name="distance"> Radius of the orbit. </param>
    /// <param name="gravitationalConstant"> constant in real world G = 6.67430e-11; // m^3 kg^-1 s^-2 </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Circular<N>(N mass, N distance, N gravitationalConstant)
        where N : IRootFunctions<N>
        =>
        N.Sqrt(gravitationalConstant * mass / distance);

    /// <summary>
    /// Speed at a distance on an elliptic orbit, the vis-viva equation sqrt(G * M * (2 / r - 1 / a)).
    /// </summary>
    /// <remarks>
    /// <a href="https://en.wikipedia.org/wiki/Vis-viva_equation">wikipedia</a>
    /// </remarks>
    /// <param name="mass"> Mass of the central body. </param>
    /// <param name="distance"> Current distance from the central body. </param>
    /// <param name="semiMajorAxis"> Semi-major axis of the orbit. </param>
    /// <param name="gravitationalConstant"> constant in real world G = 6.67430e-11; // m^3 kg^-1 s^-2 </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N VisViva<N>(N mass, N distance, N semiMajorAxis, N gravitationalConstant)
        where N : IRootFunctions<N>
        =>
        N.Sqrt(gravitationalConstant * mass * (N.CreateTruncating(2) / distance - N.One / semiMajorAxis));
}
