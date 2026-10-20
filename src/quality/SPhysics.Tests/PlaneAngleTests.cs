using Xunit;

namespace SPhysics.Tests;

public class PlaneAngleTests
{
    [Fact]
    public void PlaneAngleFormulas()
    {
        Assert.Equal(Math.PI, PlaneAngle.FromArcLength(arcLength: 2.0 * Math.PI, radius: 2.0), 10);
        Assert.Equal(3.0, PlaneAngle.FromAngularVelocity(angularVelocity: 1.5, time: 2.0), 10);
    }
}
