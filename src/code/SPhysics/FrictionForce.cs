using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Friction force.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Friction">wikipedia</a>
/// </remarks>
public static class FrictionForce
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Static<N>(N normalForce, N staticFrictionCoef)
        where N : INumberBase<N>
        =>
        normalForce * staticFrictionCoef;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Kinetic<N>(N normalForce, N kineticFrictionCoef)
        where N : INumberBase<N>
        =>
        normalForce * kineticFrictionCoef;
}
