using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary> Buoyant force on a body in a fluid at rest. </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Archimedes%27_principle">wikipedia</a></remarks>
public static class BuoyantForce
{
    /// <summary> Archimedes' principle: the weight of the displaced fluid, <c>rho * V * g</c>, directed against gravity. </summary>
    /// <param name="fluidDensity"> Density of the fluid. </param>
    /// <param name="gravity"> Gravitational acceleration. </param>
    /// <param name="displacedVolume"> Volume of the fluid the body displaces, the submerged part of its volume. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N fluidDensity, N gravity, N displacedVolume)
        where N : IMultiplyOperators<N, N, N>
        =>
        Force.FromAcceleration(Mass.FromVolume(displacedVolume, fluidDensity), gravity);
}
