using Xunit;

namespace SPhysics.Tests;

public class SolidAngleTests
{
    [Fact]
    public void SolidAngleOfTheWholeSphereIsFourPi()
    {
        Assert.Equal(4.0 * Math.PI, SolidAngle.FromArea(area: 4.0 * Math.PI * 9.0, radius: 3.0), 10);
        Assert.Equal(4.0 * Math.PI, SolidAngle.FromConeHalfAngle(Math.PI), 10);
        Assert.Equal(2.0 * Math.PI, SolidAngle.FromConeHalfAngle(Math.PI / 2.0), 10);
    }

    [Fact]
    public void SolidAngleOfANarrowConeMatchesItsCapArea()
    {
        const double halfAngle = 1e-4;

        // series of the cap area, pi * theta^2 * (1 - theta^2 / 12), the next term is far below the precision
        var expected = Math.PI * halfAngle * halfAngle * (1.0 - halfAngle * halfAngle / 12.0);

        Assert.Equal(1.0, SolidAngle.FromConeHalfAngle(halfAngle) / expected, 14);
    }
}
