using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class OrbitalStateVectorsTests
{
    private const double SemiMajorAxis = 5.0;
    private const double Eccentricity = 0.6;
    private const double Mass = 3.0;
    private const double GravitationalConstant = 2.0;

    [Fact]
    public void PositionIsOnTheAxisAtTheApsides()
    {
        var pericenter = OrbitalStateVectors.Position(SemiMajorAxis, Eccentricity, 0.0);
        var apocenter = OrbitalStateVectors.Position(SemiMajorAxis, Eccentricity, Math.PI);

        Assert.Equal(2.0, pericenter.X, 12);
        Assert.Equal(0.0, pericenter.Y, 12);
        Assert.Equal(-8.0, apocenter.X, 12);
        Assert.Equal(0.0, apocenter.Y, 12);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(2.5)]
    [InlineData(-2.0)]
    public void DistanceAndSpeedFollowTheOrbit(double eccentricAnomaly)
    {
        var position = OrbitalStateVectors.Position(SemiMajorAxis, Eccentricity, eccentricAnomaly);
        var velocity = OrbitalStateVectors.Velocity(SemiMajorAxis, Eccentricity, eccentricAnomaly, Mass, GravitationalConstant);
        var distance = Math.Sqrt(position.X * position.X + position.Y * position.Y);
        var speed = Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y);

        Assert.Equal(SemiMajorAxis * (1.0 - Eccentricity * Math.Cos(eccentricAnomaly)), distance, 12);
        Assert.Equal(OrbitalVelocity.VisViva(Mass, distance, SemiMajorAxis, GravitationalConstant), speed, 12);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(2.5)]
    [InlineData(-2.0)]
    public void ConservesTheSpecificAngularMomentum(double eccentricAnomaly)
    {
        var position = OrbitalStateVectors.Position(SemiMajorAxis, Eccentricity, eccentricAnomaly);
        var velocity = OrbitalStateVectors.Velocity(SemiMajorAxis, Eccentricity, eccentricAnomaly, Mass, GravitationalConstant);

        // h = sqrt(G * M * a * (1 - e^2)), counterclockwise
        var expected = Math.Sqrt(GravitationalConstant * Mass * SemiMajorAxis * (1.0 - Eccentricity * Eccentricity));

        Assert.Equal(expected, position.X * velocity.Y - position.Y * velocity.X, 12);
    }

    [Fact]
    public void ReferenceFrameWithoutRotationIsTheOrbitalPlane()
        => Assert.Equal((2.0, 3.0, 0.0), OrbitalStateVectors.ToReferenceFrame((2.0, 3.0), 0.0, 0.0, 0.0));

    [Fact]
    public void ReferenceFrameAppliesTheOrientationElements()
    {
        // pericenter turned a quarter into the line of nodes, tilted to the pole, node at the Y axis
        var vector = OrbitalStateVectors.ToReferenceFrame((2.0, 0.0), Math.PI / 2.0, Math.PI / 2.0, Math.PI / 2.0);

        Assert.Equal(0.0, vector.X, 12);
        Assert.Equal(0.0, vector.Y, 12);
        Assert.Equal(2.0, vector.Z, 12);

        var nodal = OrbitalStateVectors.ToReferenceFrame((2.0, 0.0), Math.PI / 2.0, 1.0, 0.0);

        Assert.Equal(0.0, nodal.X, 12);
        Assert.Equal(2.0, nodal.Y, 12);
        Assert.Equal(0.0, nodal.Z, 12);
    }

    [Fact]
    public void ReferenceFrameKeepsTheLength()
    {
        var vector = OrbitalStateVectors.ToReferenceFrame((3.0, 4.0), 0.7, 1.1, -2.3);

        Assert.Equal(5.0, Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z), 12);
    }
}
