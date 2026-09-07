using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class SemiMajorAxisTests
{
    [Theory]
    [InlineData(1.0)]
    [InlineData(4.0)]
    public void InvertsTheOrbitalPeriod(double semiMajorAxis)
    {
        const double mass = 1.0;
        const double gravitationalConstant = 1.0;

        var period = OrbitalPeriod.Eval(mass, semiMajorAxis, gravitationalConstant);

        Assert.Equal(semiMajorAxis, SemiMajorAxis.FromOrbitalPeriod(mass, period, gravitationalConstant), 10);
    }

    [Fact]
    public void MatchesKeplersThirdLaw()
    {
        // a = cbrt(G * M * T^2 / (4 * pi^2))
        const double mass = 2.0;
        const double period = 3.0;
        const double gravitationalConstant = 5.0;

        var expected = Math.Cbrt(gravitationalConstant * mass * period * period / (4 * Math.PI * Math.PI));

        Assert.Equal(expected, SemiMajorAxis.FromOrbitalPeriod(mass, period, gravitationalConstant), 10);
    }
}
