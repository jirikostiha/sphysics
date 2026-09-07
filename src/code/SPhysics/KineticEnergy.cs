using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Kinetic energy
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Kinetic_energy">wikipedia</a>
/// </remarks>
public static class KineticEnergy
{
    /// <summary>
    /// Kinetic energy of linear motion.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Linear<N>(N mass, N velocity)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(0.5) * mass * (velocity * velocity);

    /// <summary>
    /// Kinetic energy of spinning motion.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Spinning<N>(N momentOfInertia, N angularVelocity)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(0.5) * momentOfInertia * (angularVelocity * angularVelocity);

    /// <summary>
    /// Kinetic energy of linear motion from momentum.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromMomentum<N>(N momentum, N mass)
        where N : INumberBase<N>
        =>
        (momentum * momentum) / (N.CreateTruncating(2) * mass);
}
