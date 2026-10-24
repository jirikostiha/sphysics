using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics.CelestialMechanics;

/// <summary>Mean longitude, the longitude at which a body would be if its orbit were circular.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Mean_longitude">wikipedia</a></remarks>
public static class MeanLongitude
{
    /// <summary>Mean longitude from the longitude of the pericenter and the mean anomaly, L = varpi + M.</summary>
    /// <remarks> The inverse of <see cref="MeanAnomaly.Eval2{N}"/>. </remarks>
    /// <param name="longitudeOfThePericenter"> Longitude of the pericenter, the sum of the longitude of the ascending node and the argument of the pericenter. </param>
    /// <param name="meanAnomaly"> Mean anomaly. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N longitudeOfThePericenter, N meanAnomaly)
        where N : IAdditionOperators<N, N, N>
        =>
        longitudeOfThePericenter + meanAnomaly;
}
