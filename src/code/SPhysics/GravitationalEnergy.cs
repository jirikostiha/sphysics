using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Gravitational potential energy
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Gravitational_energy">wikipedia</a>
/// <a href="https://www.calculatorsoup.com/calculators/physics/gravitational-potential.php">calculator</a>
/// <a href="https://cnx.org/contents/1AJmkLE0@2.9:nDPdI3O4@3/Gravitational-potential-energy"> cnx</a>
/// </remarks>
public static class GravitationalEnergy
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass1, N mass2, N distance, N gravitationConst)
        where N : INumberBase<N>
        =>
        - gravitationConst * mass1 * mass2 / distance;

    //todo inside

    //of system in 2d cartesian coord system
    public static N OfSystem<N>(IList<((N X1, N X2) Coords, N Mass)> bodies, N gravitationConst)
    {
        //todo
        return default;
    }
}
