using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics.CelestialMechanics;

/// <summary> Specific orbital energy, the orbital energy of a small body per unit of its mass, constant along the orbit. </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Specific_orbital_energy">wikipedia</a></remarks>
public static class SpecificOrbitalEnergy
{
    /// <summary> From the state of the body, v^2 / 2 - G * M / r. </summary>
    /// <remarks> Negative on an elliptic orbit, zero on a parabolic one and positive on a hyperbolic one. </remarks>
    /// <param name="mass"> Mass of the central body; the sum of both masses for a body that is not small. </param>
    /// <param name="distance"> Distance from the central body. </param>
    /// <param name="speed"> Speed relative to the central body. </param>
    /// <param name="gravitationalConstant"> Gravitational constant, in the real world <see cref="GravitationalConstant.Value"/>. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass, N distance, N speed, N gravitationalConstant)
        where N : INumberBase<N>
        =>
        KineticEnergy.Linear(N.One, speed) + GravitationalPotential.Eval(mass, distance, gravitationalConstant);

    /// <summary> From the state of a body in space, |v|^2 / 2 - G * M / r, without a square root. </summary>
    /// <remarks> Negative on an elliptic orbit, zero on a parabolic one and positive on a hyperbolic one. </remarks>
    /// <param name="mass"> Mass of the central body; the sum of both masses for a body that is not small. </param>
    /// <param name="distance"> Distance from the central body. </param>
    /// <param name="velocity"> Velocity relative to the central body. </param>
    /// <param name="gravitationalConstant"> Gravitational constant, in the real world <see cref="GravitationalConstant.Value"/>. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass, N distance, (N X, N Y, N Z) velocity, N gravitationalConstant)
        where N : INumberBase<N>
        =>
        KineticEnergy.Linear(N.One, velocity) + GravitationalPotential.Eval(mass, distance, gravitationalConstant);
}
