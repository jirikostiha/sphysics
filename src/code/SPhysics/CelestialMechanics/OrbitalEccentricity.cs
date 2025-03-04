using System.Numerics;

namespace SPhysics.CelestialMechanics;

/// <summary>
/// Orbital eccentricity
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Orbital_eccentricity">wikipedia</a>
/// </remarks>
public static class OrbitalEccentricity
{
    public static N Eval<N>(N reducedMass, N totalOrbitalEnergy, N angularMomentum, N inverseSquare)
        where N : IRootFunctions<N>
        =>
        N.Sqrt(N.One + N.CreateTruncating(2) * totalOrbitalEnergy * (angularMomentum * angularMomentum)
            / (reducedMass * (inverseSquare * inverseSquare)));
}
