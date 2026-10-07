using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Reduced mass, <c>m1 * m2 / (m1 + m2)</c>.
/// The inertia of the relative motion of two bodies: the two-body problem reduces to one body of
/// this mass, and the kinetic energy of the relative motion is <c>1/2 * mu * v^2</c>.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Reduced_mass">wikipedia</a>
/// </remarks>
public static class ReducedMass
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass1, N mass2)
        where N : IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>, IAdditionOperators<N, N, N>
        =>
        mass1 * mass2 / (mass1 + mass2);
}
