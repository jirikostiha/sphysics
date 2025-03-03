using System.Numerics;

namespace SPhysics.CelestialMechanics;

/// <summary>
/// Barycenter
///   is the center of mass of two or more bodies that orbit one another and is the point about which the bodies orbit.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Barycenter">wikipedia</a>
/// </remarks>
public static class Barycenter
{
    /// <summary>
    /// Barycenter of two bodies.
    /// </summary>
    /// <param name="mass1"> mass of body 1 </param>
    /// <param name="mass2"> mass of body 2 </param>
    /// <param name="distance"> the distance between the centers of the two bodies </param>
    /// <returns> the distance from body 1's center to the barycenter </returns>
    public static N Eval<N>(N mass1, N mass2, N distance)
        where N : INumberBase<N>
        =>
        distance * (N.One + (mass1 / mass2));
}
