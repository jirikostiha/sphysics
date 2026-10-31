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
    public void SquarePlateMassIsTheDensityTimesTheArea()
    {
        Assert.Equal(18.0, Square.Region.Mass.FromDensity(2.0, 3.0), Precision);
    }

    [Fact]
    public void SquarePlateHasASixthOfTheMassTimesTheSquaredEdge()
    {
        Assert.Equal(15.0, Square.Region.MomentOfInertia.FromMass(10.0, 3.0), Precision);
    }

    [Fact]
    public void SquarePlateMomentFromTheDensityTakesTheMassOfThePlate()
    {
        // 18 * 9 / 6
        Assert.Equal(27.0, Square.Region.MomentOfInertia.FromDensity(2.0, 3.0), Precision);
    }

    [Fact]
    public void EllipsePlateMassIsTheDensityTimesTheArea()
    {
        Assert.Equal(12.0 * Math.PI, Ellipse.Region.Mass.FromDensity(2.0, 3.0, 2.0), Precision);
    }

    [Fact]
    public void EllipsePlateHasTheMassTimesTheSquaredSemiAxesOverFour()
    {
        Assert.Equal(13.0, Ellipse.Region.MomentOfInertia.FromMass(4.0, 3.0, 2.0), Precision);
    }

    [Fact]
    public void EllipsePlateWithEqualSemiAxesIsADisc()
    {
        Assert.Equal(Circle.Region.MomentOfInertia.FromMass(2.0, 3.0), Ellipse.Region.MomentOfInertia.FromMass(2.0, 3.0, 3.0), Precision);
    }

    [Fact]
    public void EllipsePlateMomentMatchesTheIntegralOfTheSquaredDistance()
    {
        const double a = 3.0, b = 1.25;
        var (area, polar) = InertiaIntegral.OfEllipse(a, b);

        Assert.Equal(area, Ellipse.Region.Mass.FromDensity(1.0, a, b), area * 1e-6);
        Assert.Equal(polar, Ellipse.Region.MomentOfInertia.FromDensity(1.0, a, b), polar * 1e-6);
    }

    [Fact]
    public void SinglePrecisionGivesTheSameMoment()
    {
        Assert.Equal(9.0f, Circle.Region.MomentOfInertia.FromMass(2.0f, 3.0f));
    }
}
