using System.Numerics;

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
    public static N Linear<N>(N mass, N velocity)
        where N : INumberBase<N>
        =>
        (N.One / N.CreateTruncating(2)) * mass * (velocity * velocity);

    /// <summary>
    /// Kinetic energy of spinning motion.
    /// </summary>
    public static N Spinning<N>(N momentOfInertia, N angularVelocity)
        where N : INumberBase<N>
        =>
        (N.One / N.CreateTruncating(2)) * momentOfInertia * (angularVelocity * angularVelocity);
}
