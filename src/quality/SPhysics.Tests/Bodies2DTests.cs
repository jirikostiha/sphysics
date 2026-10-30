using SMath.Geometry2D;
using Xunit;

namespace SPhysics.Tests;

public class Bodies2DTests
{
    private const int Precision = 12;

    [Fact]
    public void DiscMassIsTheDensityTimesTheArea()
    {
        Assert.Equal(2.0 * Math.PI, Circle.Region.Mass.FromDensity(2.0, 1.0), Precision);
    }

    [Fact]
    public void DiscHasHalfTheMassTimesTheSquaredRadius()
    {
        Assert.Equal(9.0, Circle.Region.MomentOfInertia.FromMass(2.0, 3.0), Precision);
    }

    [Fact]
    public void DiscMomentFromTheDensityTakesTheMassOfTheDisc()
    {
        Assert.Equal(Math.PI, Circle.Region.MomentOfInertia.FromDensity(2.0, 1.0), Precision);
    }

    [Fact]
    public void RectanglePlateMassIsTheDensityTimesTheArea()
    {
        Assert.Equal(12.0, Rectangle.Region.Mass.FromDensity(2.0, 2.0, 3.0), Precision);
    }

    [Fact]
    public void RectanglePlateHasTheMassTimesTheSquaredEdgesOverTwelve()
    {
        Assert.Equal(13.0, Rectangle.Region.MomentOfInertia.FromMass(12.0, 2.0, 3.0), Precision);
    }

    [Fact]
    public void RectanglePlateMomentFromTheDensityTakesTheMassOfThePlate()
    {
        Assert.Equal(13.0, Rectangle.Region.MomentOfInertia.FromDensity(2.0, 2.0, 3.0), Precision);
    }

    [Fact]
    public void TrianglePlateMassIsTheDensityTimesTheArea()
    {
        Assert.Equal(12.0, Triangle.Region.Mass.FromDensity(2.0, 3.0, 4.0, 5.0), Precision);
    }

    [Fact]
    public void TrianglePlateHasTheMassTimesTheSquaredEdgesOverThirtySix()
    {
        Assert.Equal(50.0, Triangle.Region.MomentOfInertia.FromMass(36.0, 3.0, 4.0, 5.0), Precision);
    }

    [Fact]
    public void TrianglePlateMomentFromTheDensityTakesTheMassOfThePlate()
    {
        Assert.Equal(50.0 / 3.0, Triangle.Region.MomentOfInertia.FromDensity(2.0, 3.0, 4.0, 5.0), Precision);
    }

    [Fact]
    public void SinglePrecisionGivesTheSameMoment()
    {
        Assert.Equal(9.0f, Circle.Region.MomentOfInertia.FromMass(2.0f, 3.0f));
    }
}
