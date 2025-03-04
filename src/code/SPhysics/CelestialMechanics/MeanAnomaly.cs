namespace SPhysics.CelestialMechanics;

using System;
using System.Numerics;

/// <summary>
/// Mean anomaly
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Mean_anomaly">wikipedia</a>
/// </remarks>
public static class MeanAnomaly
{
    /// <summary>
    /// From the eccentric anomaly
    /// </summary>
    public static N Eval<N>(N eccentricAnomaly, N eccentricity)
        where N : ITrigonometricFunctions<N>
        =>
        eccentricAnomaly - eccentricity * N.Sin(eccentricAnomaly);

    public static N Eval2<N>(N longitudeOfThePericenter, N meanLongitude)
        where N : ISubtractionOperators<N,N,N>
        =>
        meanLongitude - longitudeOfThePericenter;
}
