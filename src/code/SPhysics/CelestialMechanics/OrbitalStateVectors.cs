using System.Numerics;

namespace SPhysics.CelestialMechanics;

/// <summary>
/// Orbital state vectors, the position and velocity of a small body on an elliptic orbit around a central body.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Orbital_state_vectors">wikipedia</a>
/// <a href="https://en.wikipedia.org/wiki/Perifocal_coordinate_system">wikipedia</a>
/// The vectors are perifocal: the central body is in the origin, X points to the pericenter and Y follows the motion.
/// </remarks>
public static class OrbitalStateVectors
{
    /// <summary>
    /// Perifocal position on the orbit, (a * (cos(E) - e), b * sin(E)).
    /// </summary>
    /// <param name="semiMajorAxis"> Semi-major axis of the orbit. </param>
    /// <param name="eccentricity"> Orbital eccentricity, 0 &lt;= e &lt; 1. </param>
    /// <param name="eccentricAnomaly"> Eccentric anomaly in radians, see <see cref="EccentricAnomaly.FromMeanAnomaly{N}"/>. </param>
    public static (N X, N Y) Position<N>(N semiMajorAxis, N eccentricity, N eccentricAnomaly)
        where N : IFloatingPointIeee754<N>
    {
        (N sin, N cos) = N.SinCos(eccentricAnomaly);
        return (semiMajorAxis * (cos - eccentricity), semiMajorAxis * MinorAxisRatio(eccentricity) * sin);
    }

    /// <summary>
    /// Perifocal velocity on the orbit, sqrt(G * M / a) / (1 - e * cos(E)) * (-sin(E), sqrt(1 - e^2) * cos(E)).
    /// </summary>
    /// <param name="semiMajorAxis"> Semi-major axis of the orbit. </param>
    /// <param name="eccentricity"> Orbital eccentricity, 0 &lt;= e &lt; 1. </param>
    /// <param name="eccentricAnomaly"> Eccentric anomaly in radians, see <see cref="EccentricAnomaly.FromMeanAnomaly{N}"/>. </param>
    /// <param name="mass"> Mass of the central body; the sum of both masses for a body that is not small. </param>
    /// <param name="gravitationalConstant"> constant in real world G = 6.67430e-11; // m^3 kg^-1 s^-2 </param>
    public static (N X, N Y) Velocity<N>(N semiMajorAxis, N eccentricity, N eccentricAnomaly, N mass, N gravitationalConstant)
        where N : IFloatingPointIeee754<N>
    {
        (N sin, N cos) = N.SinCos(eccentricAnomaly);
        N speed = OrbitalVelocity.Circular(mass, semiMajorAxis, gravitationalConstant) / (N.One - eccentricity * cos);
        return (-speed * sin, speed * MinorAxisRatio(eccentricity) * cos);
    }

    /// <summary>
    /// Rotates a perifocal vector into the reference frame by the orientation elements of the orbit, R_z(Omega) * R_x(i) * R_z(omega).
    /// </summary>
    /// <param name="perifocal"> Perifocal position or velocity. </param>
    /// <param name="longitudeOfAscendingNode"> Longitude of the ascending node in radians. </param>
    /// <param name="inclination"> Inclination in radians. </param>
    /// <param name="argumentOfPericenter"> Argument of the pericenter in radians. </param>
    public static (N X, N Y, N Z) ToReferenceFrame<N>((N X, N Y) perifocal, N longitudeOfAscendingNode, N inclination, N argumentOfPericenter)
        where N : IFloatingPointIeee754<N>
    {
        (N sinNode, N cosNode) = N.SinCos(longitudeOfAscendingNode);
        (N sinInclination, N cosInclination) = N.SinCos(inclination);
        (N sinPericenter, N cosPericenter) = N.SinCos(argumentOfPericenter);

        // the perifocal vector turned by the argument of the pericenter within the orbital plane
        N x = perifocal.X * cosPericenter - perifocal.Y * sinPericenter;
        N y = perifocal.X * sinPericenter + perifocal.Y * cosPericenter;
        N tilted = y * cosInclination;

        return (x * cosNode - tilted * sinNode, x * sinNode + tilted * cosNode, y * sinInclination);
    }

    // b / a = sqrt(1 - e^2), the product form keeps the precision for e close to 1
    private static N MinorAxisRatio<N>(N eccentricity)
        where N : IFloatingPointIeee754<N>
        => N.Sqrt((N.One - eccentricity) * (N.One + eccentricity));
}
