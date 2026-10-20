using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class MeanLongitudeTests
{
    [Fact]
    public void IsTheInverseOfTheMeanAnomaly()
    {
        var meanLongitude = MeanLongitude.Eval(1.25, 0.5);

        Assert.Equal(1.75, meanLongitude);
        Assert.Equal(0.5, MeanAnomaly.Eval2(1.25, meanLongitude));
    }
}
