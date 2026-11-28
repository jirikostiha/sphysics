using System.Numerics;
using SMath.Geometry2D;
using SMath.Geometry3D;

namespace SPhysics.CelestialMechanics;

/// <summary> Keplerian orbital elements of a small body around a central body, the inverse of <see cref="OrbitalStateVectors"/>. </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Orbital_elements">wikipedia</a>
/// Angles are in radians. The semi-major axis is negative on a hyperbolic orbit and infinite on a parabolic one.
/// A circular orbit has no pericenter and an orbit in the reference plane no line of nodes; there the angles
/// that start at them follow from rounding, and only their sum with the true anomaly is reliable.
/// </remarks>
public static class OrbitalElements
{
    /// <summary> Elements of an orbit in a plane, from the position and velocity of the body. </summary>
    /// <param name="position"> Position relative to the central body. </param>
    /// <param name="velocity"> Velocity relative to the central body. </param>
    /// <param name="mass"> Mass of the central body; the sum of both masses for a body that is not small. </param>
    /// <param name="gravitationalConstant"> Gravitational constant, in the real world <see cref="GravitationalConstant.Value"/>. </param>
    /// <returns> Elements whose argument of the pericenter starts at the X axis; both angles follow the motion and the true anomaly is in (-pi, pi]. </returns>
    public static (N SemiMajorAxis, N Eccentricity, N ArgumentOfPericenter, N TrueAnomaly) FromStateVectors<N>(
        (N X, N Y) position, (N X, N Y) velocity, N mass, N gravitationalConstant)
        where N : IFloatingPointIeee754<N>
    {
        // the plane is the reference plane, so a clockwise motion is an inclination of pi with the node on the X axis
        var elements = InOrbitalPlane((position.X, position.Y, N.Zero), (velocity.X, velocity.Y, N.Zero), mass, gravitationalConstant);
        return (elements.SemiMajorAxis, elements.Eccentricity, elements.ArgumentOfPericenter, elements.TrueAnomaly);
    }

    /// <summary> Elements of an orbit in space, from the position and velocity of the body. </summary>
    /// <param name="position"> Position relative to the central body. </param>
    /// <param name="velocity"> Velocity relative to the central body. </param>
    /// <param name="mass"> Mass of the central body; the sum of both masses for a body that is not small. </param>
    /// <param name="gravitationalConstant"> Gravitational constant, in the real world <see cref="GravitationalConstant.Value"/>. </param>
    /// <returns>
    /// Elements for <see cref="OrbitalStateVectors.ToReferenceFrame{N}"/>: the inclination is in [0, pi], the longitude of the
    /// ascending node and the true anomaly in (-pi, pi]; the line of nodes of an orbit in the reference plane is the X axis.
    /// </returns>
    public static (N SemiMajorAxis, N Eccentricity, N Inclination, N LongitudeOfAscendingNode, N ArgumentOfPericenter, N TrueAnomaly) FromStateVectors<N>(
        (N X, N Y, N Z) position, (N X, N Y, N Z) velocity, N mass, N gravitationalConstant)
        where N : IFloatingPointIeee754<N>
    {
        var elements = InOrbitalPlane(position, velocity, mass, gravitationalConstant);

        return (
            elements.SemiMajorAxis,
            elements.Eccentricity,
            GeometricVector3.PolarAngle.FromCartesian(elements.Momentum),
            GeometricVector3.Azimuth.FromCartesian(elements.Node),
            elements.ArgumentOfPericenter,
            elements.TrueAnomaly);
    }

    // the elements within the plane of the orbit, with the angular momentum and the line of nodes that orient it
    private static (N SemiMajorAxis, N Eccentricity, N ArgumentOfPericenter, N TrueAnomaly, (N X, N Y, N Z) Momentum, (N X, N Y, N Z) Node) InOrbitalPlane<N>(
        (N X, N Y, N Z) position, (N X, N Y, N Z) velocity, N mass, N gravitationalConstant)
        where N : IFloatingPointIeee754<N>
    {
        N standardGravitationalParameter = gravitationalConstant * mass;
        N distance = GeometricVector3.Magnitude.FromCartesian(position);
        var momentum = SpecificAngularMomentum.Eval(position, velocity);

        // e = v x h / (G * M) - r / |r|, pointing to the pericenter
        var turn = GeometricVector3.CrossProduct.FromCartesian(velocity, momentum);
        (N X, N Y, N Z) eccentricity = (
            turn.X / standardGravitationalParameter - position.X / distance,
            turn.Y / standardGravitationalParameter - position.Y / distance,
            turn.Z / standardGravitationalParameter - position.Z / distance);

        // the line of nodes Z x h points to the ascending node
        (N X, N Y, N Z) node = momentum.X == N.Zero && momentum.Y == N.Zero
            ? (N.One, N.Zero, N.Zero)
            : (-momentum.Y, momentum.X, N.Zero);

        // both angles about the angular momentum, so in the sense of the motion
        N argumentOfPericenter = GeometricVector3.Angle.FromCartesian(node, eccentricity, momentum);
        N argumentOfLatitude = GeometricVector3.Angle.FromCartesian(node, position, momentum);
        N energy = SpecificOrbitalEnergy.Eval(mass, distance, velocity, gravitationalConstant);

        return (
            SemiMajorAxis.FromSpecificOrbitalEnergy(mass, energy, gravitationalConstant),
            GeometricVector3.Magnitude.FromCartesian(eccentricity),
            argumentOfPericenter,
            Interpolation.Angular.Difference(argumentOfPericenter, argumentOfLatitude),
            momentum,
            node);
    }
}
