using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Coefficient of restitution <c>e</c>: the ratio of the normal speed of separation after an impact to the normal speed
/// of approach before it, 1 for an elastic impact and 0 for one that leaves the bodies together.
/// </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Coefficient_of_restitution">wikipedia</a></remarks>
public static class Restitution
{
    /// <summary>
    /// Change of the normal velocity in an impact, <c>(1 + e) * v_n</c>: the approach is stopped and a share
    /// <paramref name="restitution"/> of it is returned in the opposite direction.
    /// </summary>
    /// <param name="restitution"> Coefficient of restitution. </param>
    /// <param name="normalVelocity"> Normal velocity of approach, of a body against an immovable surface or of two bodies relative to each other. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N NormalVelocityChange<N>(N restitution, N normalVelocity)
        where N : INumberBase<N>
        =>
        (N.One + restitution) * normalVelocity;

    /// <summary> Impulse two bodies exchange along the normal of their impact, <c>(1 + e) * mu * v_n</c>. </summary>
    /// <param name="restitution"> Coefficient of restitution. </param>
    /// <param name="reducedMass"> Reduced mass of the two bodies, <see cref="ReducedMass.Eval{N}"/>. </param>
    /// <param name="normalVelocity"> Normal velocity of the bodies relative to each other before the impact. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Impulse<N>(N restitution, N reducedMass, N normalVelocity)
        where N : INumberBase<N>
        =>
        reducedMass * NormalVelocityChange(restitution, normalVelocity);

    /// <summary> Share of the kinetic energy of the normal approach that the bodies keep after the impact, <c>e^2</c>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N RetainedEnergyShare<N>(N restitution)
        where N : INumberBase<N>
        =>
        restitution * restitution;

    /// <summary> Share of the kinetic energy of the normal approach that the impact absorbs, <c>1 - e^2</c>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N AbsorbedEnergyShare<N>(N restitution)
        where N : INumberBase<N>
        =>
        N.One - RetainedEnergyShare(restitution);
}
