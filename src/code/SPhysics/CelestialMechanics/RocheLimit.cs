using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics.CelestialMechanics;

/// <summary>
/// Roche limit, the distance from a primary body within which its tides tear apart a satellite held together only by its own gravity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Roche_limit">wikipedia</a>
/// </remarks>
public static class RocheLimit
{
    /// <summary>
    /// Roche limit of a rigid spherical satellite, R * cbrt(2 * rhoM / rhom).
    /// </summary>
    /// <param name="primaryRadius"> Radius of the primary body. </param>
    /// <param name="primaryDensity"> Density of the primary body. </param>
    /// <param name="satelliteDensity"> Density of the satellite. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Rigid<N>(N primaryRadius, N primaryDensity, N satelliteDensity)
        where N : IRootFunctions<N>
        =>
        primaryRadius * N.Cbrt(N.CreateTruncating(2) * primaryDensity / satelliteDensity);

    /// <summary>
    /// Roche limit of a rigid spherical satellite from the masses, r * cbrt(2 * M / m).
    /// </summary>
    /// <param name="satelliteRadius"> Radius of the satellite. </param>
    /// <param name="primaryMass"> Mass of the primary body. </param>
    /// <param name="satelliteMass"> Mass of the satellite. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N RigidFromMasses<N>(N satelliteRadius, N primaryMass, N satelliteMass)
        where N : IRootFunctions<N>
        =>
        satelliteRadius * N.Cbrt(N.CreateTruncating(2) * primaryMass / satelliteMass);

    /// <summary>
    /// Roche limit of a fluid satellite that deforms under the tides, approximately 2.44 * R * cbrt(rhoM / rhom).
    /// </summary>
    /// <param name="primaryRadius"> Radius of the primary body. </param>
    /// <param name="primaryDensity"> Density of the primary body. </param>
    /// <param name="satelliteDensity"> Density of the satellite. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Fluid<N>(N primaryRadius, N primaryDensity, N satelliteDensity)
        where N : IRootFunctions<N>
        =>
        N.CreateTruncating(2.44) * primaryRadius * N.Cbrt(primaryDensity / satelliteDensity);
}
