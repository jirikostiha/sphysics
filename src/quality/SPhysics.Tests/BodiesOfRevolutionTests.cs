using SMath.Geometry3D;
using Xunit;

namespace SPhysics.Tests;

public class BodiesOfRevolutionTests
{
    private const int Precision = 12;

    // relative tolerance of the numerical integrals of InertiaIntegral
    private const double IntegralTolerance = 1e-4;

    private static void AssertRelative(double expected, double actual)
        => Assert.Equal(expected, actual, Math.Abs(expected) * IntegralTolerance);

    [Fact]
    public void CylinderMassIsTheDensityTimesTheVolume()
    {
        Assert.Equal(24.0 * Math.PI, Cylinder.Region.Mass.FromDensity(2.0, 2.0, 3.0), Precision);
    }

    [Fact]
    public void CylinderHasHalfTheMassTimesTheSquaredRadiusAboutItsAxis()
    {
        // axial 12 * 4 / 2, transverse 12 * (3 * 4 + 9) / 12
        Assert.Equal((24.0, 21.0), Cylinder.Region.MomentOfInertia.FromMass(12.0, 2.0, 3.0));
    }

    [Fact]
    public void CylinderMomentsMatchTheIntegralOfTheSquaredDistance()
    {
        const double radius = 1.5, height = 4.0;
        var integral = InertiaIntegral.OfRevolution(0, height, _ => (0, radius), zc: height / 2);
        var (axial, transverse) = Cylinder.Region.MomentOfInertia.FromDensity(1.0, radius, height);

        AssertRelative(integral.Volume, Cylinder.Region.Mass.FromDensity(1.0, radius, height));
        AssertRelative(integral.Axial, axial);
        AssertRelative(integral.Transverse, transverse);
    }

    [Fact]
    public void FlatCylinderIsADisc()
    {
        // a disc has m r^2 / 2 about its axis and m r^2 / 4 about a diameter
        Assert.Equal((9.0, 4.5), Cylinder.Region.MomentOfInertia.FromMass(2.0, 3.0, 0.0));
    }

    [Fact]
    public void ConeMassIsTheDensityTimesTheVolume()
    {
        Assert.Equal(8.0 * Math.PI, Cone.Region.Mass.FromDensity(2.0, 2.0, 3.0), Precision);
    }

    [Fact]
    public void ConeHasThreeTenthsOfTheMassTimesTheSquaredRadiusAboutItsAxis()
    {
        // axial 3/10 * 80 * 4, transverse 3 * 80 * (4 * 4 + 16) / 80
        Assert.Equal((96.0, 96.0), Cone.Region.MomentOfInertia.FromMass(80.0, 2.0, 4.0));
    }

    [Fact]
    public void ConeMomentsMatchTheIntegralOfTheSquaredDistance()
    {
        const double radius = 1.5, height = 4.0;

        // apex up at z = h, the centre of mass a quarter of the height above the base
        var integral = InertiaIntegral.OfRevolution(0, height, z => (0, radius * (1 - (z / height))), zc: height / 4);
        var (axial, transverse) = Cone.Region.MomentOfInertia.FromDensity(1.0, radius, height);

        AssertRelative(integral.Volume, Cone.Region.Mass.FromDensity(1.0, radius, height));
        AssertRelative(integral.Axial, axial);
        AssertRelative(integral.Transverse, transverse);
    }

    [Theory]
    [InlineData(0.75, 3.0 / 5.0)]   // through the apex, 3/4 h from the centre of mass: m (3/20 r^2 + 3/5 h^2)
    [InlineData(0.25, 1.0 / 10.0)]  // through the base, 1/4 h from the centre of mass: m (3/20 r^2 + 1/10 h^2)
    public void ConeMomentsAboutTheApexAndTheBaseFollowFromTheParallelAxis(double offsetOfHeight, double heightFactor)
    {
        const double mass = 5.0, radius = 2.0, height = 3.0;
        var (_, transverse) = Cone.Region.MomentOfInertia.FromMass(mass, radius, height);

        double expected = mass * ((3.0 / 20.0 * radius * radius) + (heightFactor * height * height));
        Assert.Equal(expected, MomentOfInertia.ParallelAxis(transverse, mass, offsetOfHeight * height), Precision);
    }

    [Fact]
    public void TorusMassIsTheDensityTimesTheVolume()
    {
        // 2 pi^2 * 3 * 1 * 2
        Assert.Equal(12.0 * Math.PI * Math.PI, Torus.Region.Mass.FromDensity(2.0, 3.0, 1.0), Precision);
    }

    [Fact]
    public void TorusHasAMomentAboutItsAxisAndAboutADiameter()
    {
        // axial 8 * (36 + 3) / 4, diametral 8 * (36 + 5) / 8
        Assert.Equal((78.0, 41.0), Torus.Region.MomentOfInertia.FromMass(8.0, 3.0, 1.0));
    }

    [Fact]
    public void TorusWithAnInfinitelyThinTubeIsAHoop()
    {
        // a hoop has m R^2 about its axis and m R^2 / 2 about a diameter
        Assert.Equal((18.0, 9.0), Torus.Region.MomentOfInertia.FromMass(2.0, 3.0, 0.0));
    }

    [Fact]
    public void TorusMomentsMatchTheIntegralOfTheSquaredDistance()
    {
        const double major = 2.0, minor = 0.75;
        var integral = InertiaIntegral.OfRevolution(-minor, minor,
            z => (major - Math.Sqrt((minor * minor) - (z * z)), major + Math.Sqrt((minor * minor) - (z * z))), zc: 0);
        var (axial, diametral) = Torus.Region.MomentOfInertia.FromDensity(1.0, major, minor);

        AssertRelative(integral.Volume, Torus.Region.Mass.FromDensity(1.0, major, minor));
        AssertRelative(integral.Axial, axial);
        AssertRelative(integral.Transverse, diametral);
    }

    [Fact]
    public void ParallelAxisAddsTheMassTimesTheSquaredOffset()
    {
        // a rod about its end: m L^2 / 12 + m (L / 2)^2 = m L^2 / 3
        Assert.Equal(12.0, MomentOfInertia.ParallelAxis(3.0, 9.0, 1.0), Precision);
    }
}
