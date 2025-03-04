using System.Numerics;

namespace SPhysics.CelestialMechanics;

/// <summary>
/// Mean motion
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Mean_motion">wikipedia</a>
/// </remarks>
public static class MeanMotion
{
    public static N Eval<N>(N mass1, N mass2, N semiMajorAxis, N gravitationalConstant)
        where N : IRootFunctions<N>
        =>
        N.Sqrt(gravitationalConstant * (mass1 + mass2) / (semiMajorAxis * semiMajorAxis *semiMajorAxis));

    public static N Eval<N>(N orbitalPeriod)
        where N : ITrigonometricFunctions<N>
        =>
        N.CreateTruncating(2) * N.Pi / orbitalPeriod;
}
