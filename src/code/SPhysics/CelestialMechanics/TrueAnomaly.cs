using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics.CelestialMechanics;

/// <summary>
/// True anomaly
///   is the angle between the direction of periapsis and the current position of the body.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/True_anomaly">wikipedia</a>
/// </remarks>
public static class TrueAnomaly
{
    /// <summary>
    /// True anomaly from the eccentric anomaly.
    /// </summary>
    /// <remarks>
    /// Half angle form evaluated with Atan2, which keeps the result in the correct
    /// quadrant over the whole orbit, unlike tan(v/2) = sqrt((1+e)/(1-e)) * tan(E/2).
    /// </remarks>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="eccentricAnomaly"> Eccentric anomaly in radians. </param>
    /// <param name="eccentricity"> Orbital eccentricity, 0 &lt;= e &lt; 1. </param>
    /// <returns> True anomaly in radians, in (-pi, pi]. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromEccentricAnomaly<N>(N eccentricAnomaly, N eccentricity)
        where N : IFloatingPointIeee754<N>
    {
        N two = N.One + N.One;
        N halfAnomaly = eccentricAnomaly / two;

        return two * N.Atan2(
            N.Sqrt(N.One + eccentricity) * N.Sin(halfAnomaly),
            N.Sqrt(N.One - eccentricity) * N.Cos(halfAnomaly));
    }
}
