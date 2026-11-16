using SPhysics.CelestialMechanics;

using Xunit;

namespace SPhysics.Tests;

public class OrbitalPeriodTests
{
    [Fact]
    public void EarthCirclesTheSunInAYear()
    {
        const double sunMass = 1.98847e30;
        const double astronomicalUnit = 1.495978707e11;
        const double siderealYear = 365.256363004 * 86400;

        double period = OrbitalPeriod.Eval(sunMass, astronomicalUnit, GravitationalConstant.Value);

        // within 2e-5: G times the Sun's mass is known only to that, and the Earth's own mass is left out
        Assert.Equal(1.0, period / siderealYear, 4);
    }

    [Fact]
    public void SquareOfThePeriodGrowsWithTheCubeOfTheAxis()
    {
        double near = OrbitalPeriod.Eval(1.0, 1.0, 1.0);
        double far = OrbitalPeriod.Eval(1.0, 4.0, 1.0);

        Assert.Equal(8.0, far / near, 12);
        Assert.Equal(2.0 * Math.PI, near, 12);
    }
}
