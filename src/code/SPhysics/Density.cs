using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Volumetric mass density or specific mass.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Density">wikipedia</a>
/// </remarks>
public static class Density
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Average<N>(N volume, N mass)
        where N : INumberBase<N>
        =>
        mass / volume;
}
