using System.Numerics;

namespace SPhysics;

/// <summary>Potential energy</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Potential_energy">wikipedia</a></remarks>
public static class PotentialEnergy
{
    public static N Eval<N>(N mass, N g, N height)
        where N : IMultiplyOperators<N, N, N>
        =>
        mass * g * height;

    /// <summary> Elastic potential energy of a linear spring stretched or compressed by the displacement, <c>1/2 * k * x^2</c>. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/Elastic_energy">wikipedia</a></remarks>
    public static N Elastic<N>(N springConstant, N displacement)
        where N : INumberBase<N>
        =>
        springConstant * (displacement * displacement) / N.CreateChecked(2);
}
