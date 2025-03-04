using System.Numerics;

namespace SPhysics.CelestialMechanics;

/// <summary>
/// Escape velocity
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Escape_velocity">wikipedia</a>
/// </remarks>
public static class EscapeVelocity
{
    /// <summary>
    /// For a spherically symmetric, massive body such as a star, or planet, the escape velocity for that body, at a given distance.
    /// </summary>
    public static N Eval<N>(N mass, N distance, N gravitationalConstant)
        where N : IRootFunctions<N>
        =>
        N.Sqrt(N.CreateTruncating(2) * mass * gravitationalConstant / distance);
}
