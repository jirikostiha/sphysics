using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class SpecificAngularMomentumTests
{
    [Fact]
    public void InAPlaneIsPositiveForACounterclockwiseMotion()
    {
        Assert.Equal(6.0, SpecificAngularMomentum.Eval((2.0, 0.0), (0.0, 3.0)));
        Assert.Equal(-6.0, SpecificAngularMomentum.Eval((2.0, 0.0), (0.0, -3.0)));
    }

    [Fact]
    public void InSpaceIsPerpendicularToTheOrbit()
        => Assert.Equal((0.0, -6.0, 0.0), SpecificAngularMomentum.Eval((2.0, 0.0, 0.0), (0.0, 0.0, 3.0)));
}
