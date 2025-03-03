using System.Numerics;

namespace SPhysics.CelestialMechanics;

/// <summary>
/// True anomaly
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/True_anomaly">wikipedia</a>
/// </remarks>
public static class TrueAnomaly
{
    public static N Eval<N>(N mass, N orbitalPeriod, N gravitationalConstant)
        where N : ITrigonometricFunctions<N>, IPowerFunctions<N>
        =>
        N.Pow(gravitationalConstant * mass * orbitalPeriod * orbitalPeriod
            / (N.CreateTruncating(4) * N.Pi * N.Pi), N.CreateTruncating(1 / 3d)); //todo test
}
