using System.Numerics;
using System.Runtime.CompilerServices;

using SMath.Geometry2D;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary> Perfectly inelastic collision: two bodies that stick together and move on as one. </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Inelastic_collision">wikipedia</a></remarks>
public static class InelasticCollision
{
    /// <summary> Velocity of the merged body in a plane, the velocity of the center of mass of the two. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N X, N Y) MergedVelocity<N>((N X, N Y) velocity1, N mass1, (N X, N Y) velocity2, N mass2)
        where N : INumberBase<N>
        =>
        CenterOfMass.Eval(velocity1, mass1, velocity2, mass2);

    /// <summary> Velocity of the merged body in space, the velocity of the center of mass of the two. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N X, N Y, N Z) MergedVelocity<N>((N X, N Y, N Z) velocity1, N mass1, (N X, N Y, N Z) velocity2, N mass2)
        where N : INumberBase<N>
        =>
        CenterOfMass.Eval(velocity1, mass1, velocity2, mass2);

    /// <summary>
    /// Kinetic energy the merging turns into heat in a plane, <c>mu * |v1 - v2|^2 / 2</c>: all of the energy of the
    /// motion of the two relative to their center of mass.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N EnergyLoss<N>((N X, N Y) velocity1, N mass1, (N X, N Y) velocity2, N mass2)
        where N : INumberBase<N>
        =>
        KineticEnergy.Linear(ReducedMass.Eval(mass1, mass2), GeometricVector2.Direction.FromCartesian(velocity2, velocity1));

    /// <summary>
    /// Kinetic energy the merging turns into heat in space, <c>mu * |v1 - v2|^2 / 2</c>: all of the energy of the
    /// motion of the two relative to their center of mass.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N EnergyLoss<N>((N X, N Y, N Z) velocity1, N mass1, (N X, N Y, N Z) velocity2, N mass2)
        where N : INumberBase<N>
        =>
        KineticEnergy.Linear(ReducedMass.Eval(mass1, mass2), GeometricVector3.Direction.FromCartesian(velocity2, velocity1));
}
