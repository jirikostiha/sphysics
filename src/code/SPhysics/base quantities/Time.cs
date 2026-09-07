using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Time quantity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Time">wikipedia</a>
/// </remarks>
public static class Time
{
    public const string Name = "time";
    public const string DefaultSymbol = "t";
    public const string Dimension = "T";

    /// <summary> Period of a periodic event of the given frequency. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromFrequency<N>(N frequency)
        where N : INumberBase<N>
        =>
        N.One / frequency;
}
