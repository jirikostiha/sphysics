using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Force quantity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Force">wikipedia</a>
/// </remarks>
public static class Force
{
    public const string Name = "force";
    public const string DefaultSymbol = "F";
    public const string Dimension = Length.Dimension + " " + Mass.Dimension + " " + Time.Dimension + "-2";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromAcceleration<N>(N mass, N acceleration)
        where N : INumberBase<N>
        =>
        mass * acceleration;
}
