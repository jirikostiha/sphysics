using SMath.Geometry3D;
using Xunit;

namespace SPhysics.Tests;

public class Bodies3DTests
{
    private const int Precision = 12;

    [Fact]
    public void BallMassIsTheDensityTimesTheVolume()
    {
        Assert.Equal(4.0 * Math.PI, Sphere.Region.Mass.FromDensity(3.0, 1.0), Precision);
    }

    [Fact]
    public void BallHasTwoFifthsOfTheMassTimesTheSquaredRadius()
    {
        Assert.Equal(8.0, Sphere.Region.MomentOfInertia.FromMass(5.0, 2.0), Precision);
    }

    [Fact]
    public void BallMomentFromTheDensityTakesTheMassOfTheBall()
    {
        Assert.Equal(1.6 * Math.PI, Sphere.Region.MomentOfInertia.FromDensity(3.0, 1.0), Precision);
    }

    [Fact]
    public void BoxHasAMomentAboutEachAxisFromTheOtherTwoEdges()
    {
        var (a, b, c) = Cuboid.Region.MomentOfInertia.FromMass(12.0, 1.0, 2.0, 3.0);

        Assert.Equal(13.0, a, Precision);
        Assert.Equal(10.0, b, Precision);
        Assert.Equal(5.0, c, Precision);
    }

    [Fact]
    public void BoxMassIsTheDensityTimesTheVolume()
    {
        Assert.Equal(12.0, Cuboid.Region.Mass.FromDensity(2.0, 1.0, 2.0, 3.0), Precision);
    }

    [Fact]
    public void BoxMomentFromTheDensityTakesTheMassOfTheBox()
    {
        Assert.Equal((13.0, 10.0, 5.0), Cuboid.Region.MomentOfInertia.FromDensity(2.0, 1.0, 2.0, 3.0));
    }
}
