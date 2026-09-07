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

    /// <summary> Distance travelled at a constant velocity over a time span. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromVelocity<N>(N velocity, N time)
        where N : IMultiplyOperators<N, N, N>
        =>
        velocity * time;
}
