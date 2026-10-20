using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class OrbitalVelocityTests
{
    [Fact]
    public void CircularMatchesTheOrbitalPeriod()
    {
        const double mass = 5.0;
        const double radius = 3.0;
        const double gravitationalConstant = 2.0;

        var speed = OrbitalVelocity.Circular(mass, radius, gravitationalConstant);

        Assert.Equal(2.0 * Math.PI * radius / OrbitalPeriod.Eval(mass, radius, gravitationalConstant), speed, 12);
    }

    [Fact]
    public void VisVivaOnACircleIsTheCircularSpeed()
        => Assert.Equal(OrbitalVelocity.Circular(5.0, 3.0, 2.0), OrbitalVelocity.VisViva(5.0, 3.0, 3.0, 2.0), 12);

    [Fact]
    public void VisVivaAtInfinityOfAParabolaIsTheEscapeVelocity()
        => Assert.Equal(EscapeVelocity.Eval(5.0, 3.0, 2.0), OrbitalVelocity.VisViva(5.0, 3.0, double.PositiveInfinity, 2.0), 12);

    [Fact]
    public void VisVivaConservesTheAngularMomentumBetweenTheApsides()
    {
        // ellipse with a = 5 and e = 0.6: pericenter 2, apocenter 8
        var pericenterSpeed = OrbitalVelocity.VisViva(1.0, 2.0, 5.0, 1.0);
        var apocenterSpeed = OrbitalVelocity.VisViva(1.0, 8.0, 5.0, 1.0);

        Assert.Equal(2.0 * pericenterSpeed, 8.0 * apocenterSpeed, 12);
    }
}
