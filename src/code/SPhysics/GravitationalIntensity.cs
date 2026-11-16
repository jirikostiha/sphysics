namespace SPhysics;

using System.Numerics;
using System.Runtime.CompilerServices;

/// <summary> Intensity of the gravitational field, equal to the gravitational acceleration at a point of the field. </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Gravitational_field">wikipedia</a></remarks>
public static class GravitationalIntensity
{
    /// <summary> Intensity at <paramref name="radius"/> from a point mass in the real world, <c>-G * m / r^2</c>, pointing to the mass. </summary>
    /// <param name="mass"> Mass of the point. </param>
    /// <param name="radius"> Distance from the point. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Scalar<N>(N mass, N radius)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(-GravitationalConstant.Value) * mass / (radius * radius);

    /// <summary> Intensity at <paramref name="radius"/> from a point mass, <c>-G * m / r^2</c>, pointing to the mass. </summary>
    /// <param name="mass"> Mass of the point. </param>
    /// <param name="radius"> Distance from the point. </param>
    /// <param name="gravitationalConstant"> Gravitational constant, in the real world <see cref="GravitationalConstant.Value"/>. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Scalar<N>(N mass, N radius, N gravitationalConstant)
        where N : IUnaryNegationOperators<N, N>, IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>
        =>
        -gravitationalConstant * mass / (radius * radius);
}
