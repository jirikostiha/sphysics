using System.Numerics;
using System.Runtime.CompilerServices;

using SMath.Geometry2D;
using SMath.Geometry3D;

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

    /// <summary> Kinetic energy of a body moving at a velocity in a plane, <c>m * |v|^2 / 2</c>, without a square root. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Linear<N>(N mass, (N X, N Y) velocity)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(0.5) * mass * GeometricVector2.DotProduct.FromCartesian(velocity, velocity);

    /// <summary> Kinetic energy of a body moving at a velocity in space, <c>m * |v|^2 / 2</c>, without a square root. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Linear<N>(N mass, (N X, N Y, N Z) velocity)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(0.5) * mass * GeometricVector3.DotProduct.FromCartesian(velocity, velocity);

    /// <summary>Kinetic energy of spinning motion.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Spinning<N>(N momentOfInertia, N angularVelocity)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(0.5) * momentOfInertia * (angularVelocity * angularVelocity);

    /// <summary> Kinetic energy of a rigid body in space spinning at an angular velocity, <c>omega^T * I * omega / 2</c>. </summary>
    /// <param name="inertiaTensor"> Inertia tensor of the body, in the frame of the angular velocity. </param>
    /// <param name="angularVelocity"> Angular velocity of the body. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Spinning<N>((N XX, N YY, N ZZ, N XY, N XZ, N YZ) inertiaTensor, (N X, N Y, N Z) angularVelocity)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(0.5) * GeometricVector3.DotProduct.FromCartesian(angularVelocity, InertiaTensor.Apply(inertiaTensor, angularVelocity));

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

        N total = N.Zero;
        for (int i = 0; i < velocities.Length; i++)
        {
            total += Linear(masses[i], velocities[i]);
        }

        return total;
    }

    /// <summary> Total kinetic energy of linear motion of bodies in space, <c>sum(m * |v|^2 / 2)</c>. </summary>
    /// <param name="velocities"> Velocities of the bodies. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are velocities. </param>
    public static N Total<N>(ReadOnlySpan<(N X, N Y, N Z)> velocities, ReadOnlySpan<N> masses)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(velocities.Length, masses.Length);

        N total = N.Zero;
        for (int i = 0; i < velocities.Length; i++)
        {
            total += Linear(masses[i], velocities[i]);
        }

        return total;
    }
}
