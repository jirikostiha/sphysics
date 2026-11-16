using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>Kinetic energy</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Kinetic_energy">wikipedia</a></remarks>
public static class KineticEnergy
{
    /// <summary>Kinetic energy of linear motion.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Linear<N>(N mass, N velocity)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(0.5) * mass * (velocity * velocity);

    /// <summary>Kinetic energy of spinning motion.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Spinning<N>(N momentOfInertia, N angularVelocity)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(0.5) * momentOfInertia * (angularVelocity * angularVelocity);

    /// <summary>Kinetic energy of linear motion from momentum.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromMomentum<N>(N momentum, N mass)
        where N : INumberBase<N>
        =>
        (momentum * momentum) / (N.CreateTruncating(2) * mass);

    /// <summary> Total kinetic energy of linear motion of bodies in a plane, <c>sum(m * |v|^2 / 2)</c>. </summary>
    /// <param name="velocities"> Velocities of the bodies. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are velocities. </param>
    public static N Total<N>(ReadOnlySpan<(N X, N Y)> velocities, ReadOnlySpan<N> masses)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(velocities.Length, masses.Length);

        N doubled = N.Zero;
        for (int i = 0; i < velocities.Length; i++)
        {
            var (vx, vy) = velocities[i];
            doubled += masses[i] * ((vx * vx) + (vy * vy));
        }

        return N.CreateTruncating(0.5) * doubled;
    }

    /// <summary> Total kinetic energy of linear motion of bodies in space, <c>sum(m * |v|^2 / 2)</c>. </summary>
    /// <param name="velocities"> Velocities of the bodies. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are velocities. </param>
    public static N Total<N>(ReadOnlySpan<(N X, N Y, N Z)> velocities, ReadOnlySpan<N> masses)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(velocities.Length, masses.Length);

        N doubled = N.Zero;
        for (int i = 0; i < velocities.Length; i++)
        {
            var (vx, vy, vz) = velocities[i];
            doubled += masses[i] * ((vx * vx) + (vy * vy) + (vz * vz));
        }

        return N.CreateTruncating(0.5) * doubled;
    }
}
