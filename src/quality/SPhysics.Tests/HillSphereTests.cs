using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class HillSphereTests
{
    [Fact]
    public void RadiusOfTheEarthIsAboutOneAndAHalfMillionKilometers()
    {
        var radius = HillSphere.Eval(5.972e24, 1.989e30, 1.496e11, 0.0167);

        Assert.InRange(radius, 1.46e9, 1.48e9);
    }

    [Fact]
    public void RadiusScalesWithTheCubeRootOfTheMassRatio()
    {
        Assert.Equal(10.0, HillSphere.Eval(3.0, 1000.0, 100.0), 12);
        Assert.Equal(5.0, HillSphere.Eval(3.0, 1000.0, 100.0, 0.5), 12);
    }
}
