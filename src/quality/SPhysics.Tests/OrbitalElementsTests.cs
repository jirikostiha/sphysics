using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class OrbitalElementsTests
{
    private const double SemiMajorAxis = 5.0;
    private const double Eccentricity = 0.6;
    private const double Mass = 3.0;
    private const double GravitationalConstant = 2.0;

    [Theory]
    [InlineData(0.4, 1.2, 0.7, 1.0)]
    [InlineData(1.1, -2.0, -1.3, 2.5)]
    [InlineData(2.5, 3.0, 2.0, -2.0)]
    [InlineData(Math.PI / 2.0, 0.0, 0.0, 0.0)]
    public void InvertTheStateVectorsInSpace(double inclination, double ascendingNode, double argumentOfPericenter, double eccentricAnomaly)
    {
        var (position, velocity) = State(inclination, ascendingNode, argumentOfPericenter, eccentricAnomaly);

        var elements = OrbitalElements.FromStateVectors(position, velocity, Mass, GravitationalConstant);

        Assert.Equal(SemiMajorAxis, elements.SemiMajorAxis, 10);
        Assert.Equal(Eccentricity, elements.Eccentricity, 10);
        Assert.Equal(inclination, elements.Inclination, 10);
        Assert.Equal(ascendingNode, elements.LongitudeOfAscendingNode, 10);
        Assert.Equal(argumentOfPericenter, elements.ArgumentOfPericenter, 10);
        Assert.Equal(TrueAnomaly.FromEccentricAnomaly(eccentricAnomaly, Eccentricity), elements.TrueAnomaly, 10);
    }

    [Fact]
    public void OrbitInTheReferencePlaneHasItsNodeOnTheXAxis()
    {
        var (position, velocity) = State(0.0, 1.0, 0.5, 1.0);

        var elements = OrbitalElements.FromStateVectors(position, velocity, Mass, GravitationalConstant);

        Assert.Equal(0.0, elements.Inclination, 10);
        Assert.Equal(0.0, elements.LongitudeOfAscendingNode, 10);
        Assert.Equal(1.5, elements.ArgumentOfPericenter, 10);
        Assert.Equal(TrueAnomaly.FromEccentricAnomaly(1.0, Eccentricity), elements.TrueAnomaly, 10);
    }

    [Theory]
    [InlineData(0.7, 1.0)]
    [InlineData(-2.0, 2.5)]
    [InlineData(3.0, -2.0)]
    public void InvertTheStateVectorsInAPlane(double argumentOfPericenter, double eccentricAnomaly)
    {
        var (position, velocity) = State(0.0, 0.0, argumentOfPericenter, eccentricAnomaly);

        var elements = OrbitalElements.FromStateVectors((position.X, position.Y), (velocity.X, velocity.Y), Mass, GravitationalConstant);

        Assert.Equal(SemiMajorAxis, elements.SemiMajorAxis, 10);
        Assert.Equal(Eccentricity, elements.Eccentricity, 10);
        Assert.Equal(argumentOfPericenter, elements.ArgumentOfPericenter, 10);
        Assert.Equal(TrueAnomaly.FromEccentricAnomaly(eccentricAnomaly, Eccentricity), elements.TrueAnomaly, 10);
    }

    [Fact]
    public void AnglesOfAClockwiseOrbitInAPlaneFollowTheMotion()
    {
        var (position, velocity) = State(0.0, 0.0, 0.7, 1.0);

        // the mirror image of the orbit has its pericenter 0.7 clockwise from the X axis
        var elements = OrbitalElements.FromStateVectors((position.X, -position.Y), (velocity.X, -velocity.Y), Mass, GravitationalConstant);

        Assert.Equal(0.7, elements.ArgumentOfPericenter, 10);
        Assert.Equal(TrueAnomaly.FromEccentricAnomaly(1.0, Eccentricity), elements.TrueAnomaly, 10);
    }

    [Fact]
    public void HyperbolicOrbitHasANegativeSemiMajorAxis()
    {
        // above the escape speed sqrt(2 * G * M / r) = sqrt(6) at the pericenter
        var elements = OrbitalElements.FromStateVectors((2.0, 0.0), (0.0, 3.0), Mass, GravitationalConstant);

        Assert.Equal(-2.0, elements.SemiMajorAxis, 12);
        Assert.Equal(2.0, elements.Eccentricity, 12);
        Assert.Equal(0.0, elements.ArgumentOfPericenter, 12);
        Assert.Equal(0.0, elements.TrueAnomaly, 12);
        Assert.Equal(2.0, Apsis.Pericenter(elements.SemiMajorAxis, elements.Eccentricity), 12);
    }

    private static ((double X, double Y, double Z) Position, (double X, double Y, double Z) Velocity) State(
        double inclination, double ascendingNode, double argumentOfPericenter, double eccentricAnomaly)
    {
        var position = OrbitalStateVectors.Position(SemiMajorAxis, Eccentricity, eccentricAnomaly);
        var velocity = OrbitalStateVectors.Velocity(SemiMajorAxis, Eccentricity, eccentricAnomaly, Mass, GravitationalConstant);

        return (
            OrbitalStateVectors.ToReferenceFrame(position, ascendingNode, inclination, argumentOfPericenter),
            OrbitalStateVectors.ToReferenceFrame(velocity, ascendingNode, inclination, argumentOfPericenter));
    }
}
