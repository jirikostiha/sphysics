using System.Numerics;

namespace SPhysics;

/// <summary>
/// Potential energy
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Potential_energy">wikipedia</a>
/// </remarks>
public static class PotentialEnergy
{
    public static N Eval<N>(N mass, N g, N height)
        where N : IMultiplyOperators<N, N, N>
        =>
        mass * g * height;
}
