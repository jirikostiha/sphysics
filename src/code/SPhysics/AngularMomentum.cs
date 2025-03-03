using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Angular momentum
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Angular_momentum">wikipedia</a>
/// </remarks>
public static class AngularMomentum
{
    //scalar
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass, N velocity, N radius)
        where N : INumberBase<N>
        =>
        mass * velocity * radius;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromMomentum<N>(N momentum, N radius)
        where N : INumberBase<N>
        =>
        momentum * radius;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromMomentOfInertia<N>(N momentOfInertia, N angularVelocity)
        where N : INumberBase<N>
        =>
        momentOfInertia * angularVelocity;
}
