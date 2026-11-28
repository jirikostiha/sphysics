using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class SpecificOrbitalEnergyTests
{
    [Fact]
    public void CircularOrbitHasHalfOfThePotential()
    {
        // epsilon = -G * M / (2 * r)
        var speed = OrbitalVelocity.Circular(3.0, 4.0, 2.0);

        Assert.Equal(-0.75, SpecificOrbitalEnergy.Eval(3.0, 4.0, speed, 2.0), 12);
    }

    [Fact]
    public void EscapeSpeedGivesZero()
    {
        var speed = EscapeVelocity.Eval(3.0, 4.0, 2.0);

        Assert.Equal(0.0, SpecificOrbitalEnergy.Eval(3.0, 4.0, speed, 2.0), 12);
    }

    [Fact]
    public void VelocityInSpaceGivesTheSameAsItsSpeed()
        => Assert.Equal(SpecificOrbitalEnergy.Eval(3.0, 4.0, 3.0, 2.0), SpecificOrbitalEnergy.Eval(3.0, 4.0, (1.0, 2.0, 2.0), 2.0), 12);
}
