using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics.CelestialMechanics;

/// <summary>Semi-major axis of an orbit.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Semi-major_and_semi-minor_axes">wikipedia</a></remarks>
public static class SemiMajorAxis
{
    /// <summary>
    /// Semi-major axis of a small body orbiting a central body, from Kepler's third law.
    /// Inverse of <see cref="OrbitalPeriod"/>.
    /// </summary>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="mass"> Mass of the central body. </param>
    /// <param name="orbitalPeriod"> Orbital period. </param>
    /// <param name="gravitationalConstant"> Gravitational constant, in the real world <see cref="GravitationalConstant.Value"/>. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromOrbitalPeriod<N>(N mass, N orbitalPeriod, N gravitationalConstant)
        where N : ITrigonometricFunctions<N>, IPowerFunctions<N>
        =>
        N.Pow(gravitationalConstant * mass * (orbitalPeriod * orbitalPeriod)
            / (N.CreateTruncating(4) * N.Pi * N.Pi), N.CreateTruncating(1 / 3d));

    /// <summary> Semi-major axis of a small body orbiting a central body, from its specific orbital energy, -G * M / (2 * epsilon). </summary>
    /// <remarks> Negative on a hyperbolic orbit and infinite on a parabolic one. </remarks>
    /// <param name="mass"> Mass of the central body; the sum of both masses for a body that is not small. </param>
    /// <param name="specificOrbitalEnergy"> Specific orbital energy, see <see cref="SpecificOrbitalEnergy"/>. </param>
    /// <param name="gravitationalConstant"> Gravitational constant, in the real world <see cref="GravitationalConstant.Value"/>. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromSpecificOrbitalEnergy<N>(N mass, N specificOrbitalEnergy, N gravitationalConstant)
        where N : INumberBase<N>
        =>
        -gravitationalConstant * mass / (N.CreateTruncating(2) * specificOrbitalEnergy);
}
