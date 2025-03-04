using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Distance
/// </summary>
public static class Distance
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromGravitation<N>(N mass1, N mass2, N force, N gravitationConst)
        where N : IRootFunctions<N>
        =>
        N.Sqrt(gravitationConst * mass1 * mass2 / force);
}
