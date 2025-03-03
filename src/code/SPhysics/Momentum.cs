using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Momentum quantity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Momentum">wikipedia</a>
/// </remarks>
public static class Momentum
{
    public const string Name = "Momentum";
    public const string DefaultSymbol = "p";
    public const string Dimension = Mass.Dimension + " " + Length.Dimension + " " + Time.Dimension + "-1";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Scalar<N>(N mass, N velocity)
        where N : INumberBase<N>
        =>
        mass * velocity;
}
