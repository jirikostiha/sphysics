using System.Numerics;

namespace SPhysics.CelestialMechanics;

/// <summary>
/// Orbital period
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Orbital_period">wikipedia</a>
/// </remarks>
public static class OrbitalPeriod
{
    /// <summary>
    /// Small body orbiting a central body
    /// </summary>
    public static N Eval<N>(N mass, N semiMajorAxis, N gravitationalConstant)
        where N : ITrigonometricFunctions<N>, IRootFunctions<N>
        =>
        N.CreateTruncating(2) * N.Pi * N.Sqrt(semiMajorAxis * semiMajorAxis * semiMajorAxis / (gravitationalConstant * mass));
    //todo test
}
