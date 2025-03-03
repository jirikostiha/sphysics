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
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="N"></typeparam>
    /// <param name="mass"></param>
    /// <param name="radius">Distance from center</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Scalar<N>(N mass, N radius)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(-6.67430e-11) * mass / (radius * radius);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Scalar<N>(N mass, N radius, N gravitationConst)
        where N : INumberBase<N>
        =>
        -gravitationConst * mass / (radius * radius);


    //todo inside
}
