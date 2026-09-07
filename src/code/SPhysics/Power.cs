using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Power quantity
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Power_(physics)">wikipedia</a>
/// </remarks>
public static class Power
{
    public const string Name = "power";
    public const string DefaultSymbol = "P";
    public const string Dimension = Mass.Dimension + " " + Length.Dimension + "+2 " + Time.Dimension + "-3";

    /// <summary> Power as work done over a time span. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N work, N time)
        where N : IDivisionOperators<N, N, N>
        =>
        work / time;

    /// <summary> Power delivered by a force acting on a body moving at the given velocity. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromForce<N>(N force, N velocity)
        where N : IMultiplyOperators<N, N, N>
        =>
        force * velocity;
}
