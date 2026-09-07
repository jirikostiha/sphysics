using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Impulse quantity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Impulse_(physics)">wikipedia</a>
/// </remarks>
public static class Impulse
{
    public const string Name = "Impulse";
    public const string DefaultSymbol = "J";
    public const string Dimension = Length.Dimension + " " + Mass.Dimension + " " + Time.Dimension + "-1";

    /// <summary> Impulse of a constant force acting over a time span. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N force, N time)
        where N : IMultiplyOperators<N, N, N>
        =>
        force * time;
}
