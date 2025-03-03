namespace SPhysics;

using System.Numerics;
using System.Runtime.CompilerServices;

/// <summary>
/// Intensity of gravitational field
///   is equal to gravitational acceleration in that point of field
/// </summary>
/// <remarks>
/// https://blog.myrank.co.in/intensity-of-gravitational-field/
/// <a href="">wikipedia</a>
/// </remarks>
public static class GravitationalIntensity
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Scalar<N>(N mass, N distance, N gravitationConst)
        where N : INumberBase<N>
        =>
        gravitationConst * mass / (distance * distance);

    //todo inside
}
