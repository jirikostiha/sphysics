using System.Numerics;
using System.Runtime.CompilerServices;

using SMath.Geometry2D;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary>Angular momentum</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Angular_momentum">wikipedia</a></remarks>
public static class AngularMomentum
{
    //scalar
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass, N velocity, N radius)
        where N : IMultiplyOperators<N, N, N>
        =>
        mass * velocity * radius;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromMomentum<N>(N momentum, N radius)
        where N : IMultiplyOperators<N, N, N>
        =>
        momentum * radius;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromMomentOfInertia<N>(N momentOfInertia, N angularVelocity)
        where N : IMultiplyOperators<N, N, N>
        =>
        momentOfInertia * angularVelocity;

    /// <summary> Angular momentum of a rigid body in space spinning at an angular velocity, <c>L = I * omega</c>. </summary>
    /// <param name="inertiaTensor"> Inertia tensor of the body, in the frame of the angular velocity. </param>
    /// <param name="angularVelocity"> Angular velocity of the body. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N X, N Y, N Z) FromInertiaTensor<N>((N XX, N YY, N ZZ, N XY, N XZ, N YZ) inertiaTensor, (N X, N Y, N Z) angularVelocity)
        where N : INumberBase<N>
        =>
        InertiaTensor.Apply(inertiaTensor, angularVelocity);

    /// <summary>
    /// Total angular momentum of bodies in a plane about <paramref name="origin"/>, <c>sum(m * (r - o) x v)</c>; the
    /// result is its component perpendicular to the plane, positive for a counterclockwise motion.
    /// </summary>
    /// <remarks>
    /// About the center of mass the result equals the one in the frame of the center of mass, as the moving frame adds
    /// <c>sum(m * (r - o)) x V</c>, which is zero there.
    /// </remarks>
    /// <param name="positions"> Positions of the bodies. </param>
    /// <param name="velocities"> Velocities of the bodies, as many as there are positions. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are positions. </param>
    /// <param name="origin"> Point the angular momentum is taken about. </param>
    public static N Total<N>(
        ReadOnlySpan<(N X, N Y)> positions,
        ReadOnlySpan<(N X, N Y)> velocities,
        ReadOnlySpan<N> masses,
        (N X, N Y) origin)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(positions.Length, velocities.Length, masses.Length);

        N total = N.Zero;
        for (int i = 0; i < positions.Length; i++)
        {
            var arm = GeometricVector2.Direction.FromCartesian(origin, positions[i]);
            total += masses[i] * GeometricVector2.CrossProduct.FromCartesian(arm, velocities[i]);
        }

        return total;
    }

    /// <summary> Total angular momentum of bodies in space about <paramref name="origin"/>, <c>sum(m * (r - o) x v)</c>. </summary>
    /// <remarks> About the center of mass the result equals the one in the frame of the center of mass. </remarks>
    /// <param name="positions"> Positions of the bodies. </param>
    /// <param name="velocities"> Velocities of the bodies, as many as there are positions. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are positions. </param>
    /// <param name="origin"> Point the angular momentum is taken about. </param>
    public static (N X, N Y, N Z) Total<N>(
        ReadOnlySpan<(N X, N Y, N Z)> positions,
        ReadOnlySpan<(N X, N Y, N Z)> velocities,
        ReadOnlySpan<N> masses,
        (N X, N Y, N Z) origin)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(positions.Length, velocities.Length, masses.Length);

        N x = N.Zero;
        N y = N.Zero;
        N z = N.Zero;
        for (int i = 0; i < positions.Length; i++)
        {
            var arm = GeometricVector3.Direction.FromCartesian(origin, positions[i]);
            var (lx, ly, lz) = GeometricVector3.CrossProduct.FromCartesian(arm, velocities[i]);
            x += masses[i] * lx;
            y += masses[i] * ly;
            z += masses[i] * lz;
        }

        return (x, y, z);
    }
}
