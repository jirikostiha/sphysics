using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Acceleration quantity
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Acceleration">wikipedia</a>
/// </remarks>
public static class Acceleration
{
    public const string Name = "acceleration";
    public const string DefaultSymbol = "a";
    public const string Dimension = Length.Dimension + " " + Time.Dimension + "-2";

    /// <summary> Acceleration as a change of velocity over a time span. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N velocityChange, N time)
        where N : IDivisionOperators<N, N, N>
        =>
        velocityChange / time;

    /// <summary> Acceleration of a mass under a force, from Newton's second law. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromForce<N>(N force, N mass)
        where N : IDivisionOperators<N, N, N>
        =>
        force / mass;
}
