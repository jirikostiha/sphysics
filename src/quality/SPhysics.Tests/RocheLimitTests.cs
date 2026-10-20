using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class RocheLimitTests
{
    [Fact]
    public void RigidFromDensities()
        => Assert.Equal(2.0 * 3.0, RocheLimit.Rigid(3.0, 4.0, 1.0), 12);

    [Fact]
    public void RigidFromMassesMatchesTheDensities()
    {
        // primary of radius 10 and density 2, satellite of radius 1 and density 1: the mass ratio is 2 * 10^3
        double primaryMass = 2.0 * 1000.0;
        double satelliteMass = 1.0;

        Assert.Equal(RocheLimit.Rigid(10.0, 2.0, 1.0), RocheLimit.RigidFromMasses(1.0, primaryMass, satelliteMass), 12);
    }

    [Fact]
    public void FluidLimitOfTheMoonAroundTheEarthIsAboutEighteenThousandKilometers()
    {
        var limit = RocheLimit.Fluid(6371.0, 5513.0, 3344.0);

        Assert.InRange(limit, 18300.0, 18450.0);
    }
}
