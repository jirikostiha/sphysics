using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry2D;
using SMath.Geometry3D;

namespace SPhysics.CelestialMechanics;

/// <summary> Specific angular momentum, the orbital angular momentum of a small body per unit of its mass, h = r x v, constant along the orbit. </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Specific_angular_momentum">wikipedia</a></remarks>
public static class SpecificAngularMomentum
{
    /// <summary> Of a body in a plane, positive for a counterclockwise motion. </summary>
    /// <param name="position"> Position relative to the central body. </param>
    /// <param name="velocity"> Velocity relative to the central body. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>((N X, N Y) position, (N X, N Y) velocity)
        where N : INumberBase<N>
        =>
        GeometricVector2.CrossProduct.FromCartesian(position, velocity);

    /// <summary> Of a body in space, perpendicular to the plane of the orbit. </summary>
    /// <param name="position"> Position relative to the central body. </param>
    /// <param name="velocity"> Velocity relative to the central body. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N X, N Y, N Z) Eval<N>((N X, N Y, N Z) position, (N X, N Y, N Z) velocity)
        where N : INumberBase<N>
        =>
        GeometricVector3.CrossProduct.FromCartesian(position, velocity);
}
