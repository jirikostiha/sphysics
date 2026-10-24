using System.Numerics;

namespace SPhysics.CelestialMechanics;

/// <summary>Eccentric anomaly, the angle that places a body on its elliptic orbit seen from the center of the ellipse.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Eccentric_anomaly">wikipedia</a></remarks>
public static class EccentricAnomaly
{
    private const int MaxIterations = 32;

    /// <summary>Eccentric anomaly from the mean anomaly, the solution of Kepler's equation M = E - e * sin(E).</summary>
    /// <remarks>
    /// <a href="https://en.wikipedia.org/wiki/Kepler%27s_equation">wikipedia</a>
    /// The inverse of <see cref="MeanAnomaly.Eval{N}"/>, solved by Newton's method to the full precision of <typeparamref name="N"/>.
    /// </remarks>
    /// <param name="meanAnomaly"> Mean anomaly in radians. </param>
    /// <param name="eccentricity"> Orbital eccentricity, 0 &lt;= e &lt; 1. </param>
    /// <returns> Eccentric anomaly in radians, in the same turn as the mean anomaly. </returns>
    public static N FromMeanAnomaly<N>(N meanAnomaly, N eccentricity)
        where N : IFloatingPointIeee754<N>
    {
        // solve on [-pi, pi], where the starting guess is reliable, then add the whole turns back
        N reduced = N.Ieee754Remainder(meanAnomaly, N.Tau);
        N turns = meanAnomaly - reduced;

        // Danby's starting guess keeps Newton's method convergent for every 0 <= e < 1,
        // the sign of sin(M) is the sign of M on [-pi, pi]
        N anomaly = reduced + N.CreateTruncating(0.85) * eccentricity * N.CopySign(N.One, reduced);
        for (int i = 0; i < MaxIterations; i++)
        {
            (N sin, N cos) = N.SinCos(anomaly);
            N next = anomaly - (anomaly - eccentricity * sin - reduced) / (N.One - eccentricity * cos);
            if (next == anomaly)
            {
                break;
            }

            anomaly = next;
        }

        return anomaly + turns;
    }
}
