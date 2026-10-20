using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Jerk quantity, the rate of change of acceleration.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Jerk_(physics)">wikipedia</a>
/// </remarks>
public static class Jerk
{
    public const string Name = "jerk";
    public const string DefaultSymbol = "j";
    public const string Dimension = Length.Dimension + " " + Time.Dimension + "-3";

    /// <summary> Jerk as a change of acceleration over a time span. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N accelerationChange, N time)
        where N : IDivisionOperators<N, N, N>
        =>
        accelerationChange / time;

    /// <summary> Jerk of a mass under a changing force, the time derivative of Newton's second law. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromForceChange<N>(N forceChange, N time, N mass)
        where N : IDivisionOperators<N, N, N>
        =>
        forceChange / time / mass;
}
