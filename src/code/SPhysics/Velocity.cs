using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Velocity quantity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Velocity">wikipedia</a>
/// </remarks>
public static class Velocity
{
    public const string Name = "velocity";
    public const string DefaultSymbol = "v";
    public const string Dimension = Length.Dimension + " " + Time.Dimension + "-1";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromAcceleration<N>(N acceleration, N time)
        where N : IMultiplyOperators<N, N, N>
        =>
        acceleration * time;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromAngularVelocity<N>(N angularVelocity, N radius)
        where N : IMultiplyOperators<N, N, N>
        =>
        angularVelocity * radius;
}
