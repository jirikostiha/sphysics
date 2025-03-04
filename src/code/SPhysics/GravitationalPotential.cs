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
        - gravitationConst * mass / distance;

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
