using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics.CelestialMechanics;

/// <summary>
/// Semi-major axis of an orbit.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Semi-major_and_semi-minor_axes">wikipedia</a>
/// </remarks>
public static class SemiMajorAxis
{
    /// <summary>
    /// Semi-major axis of a small body orbiting a central body, from Kepler's third law.
    /// Inverse of <see cref="OrbitalPeriod"/>.
    /// </summary>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="mass"> Mass of the central body. </param>
    /// <param name="orbitalPeriod"> Orbital period. </param>
    /// <param name="gravitationalConstant"> constant in real world G = 6.67430e-11; // m^3 kg^-1 s^-2 </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromOrbitalPeriod<N>(N mass, N orbitalPeriod, N gravitationalConstant)
        where N : ITrigonometricFunctions<N>, IPowerFunctions<N>
        =>
        N.Pow(gravitationalConstant * mass * (orbitalPeriod * orbitalPeriod)
            / (N.CreateTruncating(4) * N.Pi * N.Pi), N.CreateTruncating(1 / 3d));
}
