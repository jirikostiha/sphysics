using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Gravitational potential
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Gravitational_potential">wikipedia</a>
/// </remarks>
public static class GravitationalPotential
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass, N distance, N gravitationConst)
        where N : IUnaryNegationOperators<N, N>, IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>
        =>
        -gravitationConst * mass / distance;

    /// <summary>
    /// Softened gravitational potential of a point mass, <c>-G * m / sqrt(r^2 + softening^2)</c>.
    /// It is the potential of the accelerations <see cref="NBodyGravity"/> computes, so an energy
    /// balance built on it agrees with a simulation integrated with the same softening.
    /// </summary>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="mass"> Mass of the point. </param>
    /// <param name="distanceSquared"> Squared distance from the point, so callers that already have it skip a square root. </param>
    /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
    /// <param name="gravitationConst"> Gravitational constant. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Softened<N>(N mass, N distanceSquared, N softening, N gravitationConst)
        where N : IRootFunctions<N>
        =>
        -gravitationConst * mass / N.Sqrt(distanceSquared + (softening * softening));

    //todo inside
}

///// <summary>
///// Mass attraction potential
///// </summary>
///// <remarks>
///// <a href="https://en.wikipedia.org/wiki/Gravitational_potential">wikipedia</a>
///// </remarks>
//public static class MassAtractionPotential //?? co jednotky?
//{
//    public static double f(double mass, double distance)
//        => - mass / distance;
//}
