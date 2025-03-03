using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Angular velocity.
/// </summary>
/// <remarks>
/// <a href="">wikipedia</a>
/// </remarks>
public static class AngularVelocity
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromLinearVelocity<N>(N velocity, N radius)
        where N : INumberBase<N>
        =>
        velocity / radius;
}
