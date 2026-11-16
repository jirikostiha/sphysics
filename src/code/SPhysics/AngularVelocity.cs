using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>Angular velocity.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Angular_velocity">wikipedia</a></remarks>
public static class AngularVelocity
{
    /// <summary> Angular velocity of a point moving at <paramref name="velocity"/> on a circle of <paramref name="radius"/>, <c>v / r</c>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromLinearVelocity<N>(N velocity, N radius)
        where N : IDivisionOperators<N, N, N>
        =>
        velocity / radius;

    /// <summary> Angular velocity of a full turn completed in the given period. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromPeriod<N>(N period)
        where N : ITrigonometricFunctions<N>
        =>
        N.CreateTruncating(2) * N.Pi / period;

    /// <summary> Angular velocity from the rotation frequency. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromFrequency<N>(N frequency)
        where N : ITrigonometricFunctions<N>
        =>
        N.CreateTruncating(2) * N.Pi * frequency;
}
