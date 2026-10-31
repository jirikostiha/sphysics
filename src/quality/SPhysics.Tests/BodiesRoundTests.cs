using SMath.Geometry3D;
using Xunit;

namespace SPhysics.Tests;

public class BodiesRoundTests
{
    private const int Precision = 12;

    // relative tolerance of the numerical integrals of InertiaIntegral
    private const double IntegralTolerance = 1e-4;

    private static void AssertRelative(double expected, double actual)
        => Assert.Equal(expected, actual, Math.Abs(expected) * IntegralTolerance);

    [Fact]
    public void SphericalShellMassIsTheDensityTimesTheVolume()
    {
        // 4/3 pi (8 - 1) * 3
        Assert.Equal(28.0 * Math.PI, SphericalShell.Region.Mass.FromDensity(3.0, 1.0, 2.0), Precision);
    }

    [Fact]
    public void SphericalShellHasTwoFifthsOfTheMassTimesTheRatioOfTheFifthAndTheThirdPowers()
    {
        // 2/5 * 7 * (32 - 1) / (8 - 1)
        Assert.Equal(12.4, SphericalShell.Region.MomentOfInertia.FromMass(7.0, 1.0, 2.0), Precision);
    }

    [Fact]
    public void SphericalShellWithoutACavityIsABall()
    {
        Assert.Equal(Sphere.Region.MomentOfInertia.FromMass(5.0, 2.0), SphericalShell.Region.MomentOfInertia.FromMass(5.0, 0.0, 2.0), Precision);
    }

    [Fact]
    public void InfinitelyThinSphericalShellIsAHollowSphere()
    {
        // 2/3 m r^2
        Assert.Equal(2.0 / 3.0 * 3.0 * 4.0, SphericalShell.Region.MomentOfInertia.FromMass(3.0, 2.0, 2.0), Precision);
    }

    [Fact]
    public void ThinSphericalShellKeepsItsPrecision()
    {
        // for r1 = 1 - e the moment is 2/3 m (1 - e) + O(e^2); the direct form would lose about 7 of these digits
        // to the differences of the powers
        const double thickness = 1e-9;
        double moment = SphericalShell.Region.MomentOfInertia.FromMass(1.0, 1.0 - thickness, 1.0);

        Assert.Equal(2.0 / 3.0 * (1.0 - thickness), moment, 15);
    }

    [Fact]
    public void SphericalShellMomentMatchesTheIntegralOfTheSquaredDistance()
    {
        const double inner = 0.6, outer = 1.5;
        var integral = InertiaIntegral.OfRevolution(-outer, outer,
            z => (Math.Sqrt(Math.Max(0, (inner * inner) - (z * z))), Math.Sqrt((outer * outer) - (z * z))), zc: 0);

        AssertRelative(integral.Volume, SphericalShell.Region.Mass.FromDensity(1.0, inner, outer));
        AssertRelative(integral.Axial, SphericalShell.Region.MomentOfInertia.FromDensity(1.0, inner, outer));
    }

    [Fact]
    public void EllipsoidMassIsTheDensityTimesTheVolume()
    {
        // 4/3 pi * 1 * 2 * 3 * 2
        Assert.Equal(16.0 * Math.PI, Ellipsoid.Region.Mass.FromDensity(2.0, 1.0, 2.0, 3.0), Precision);
    }

    [Fact]
    public void EllipsoidHasAMomentAboutEachAxisFromTheOtherTwoSemiAxes()
    {
        Assert.Equal((13.0, 10.0, 5.0), Ellipsoid.Region.MomentOfInertia.FromMass(5.0, 1.0, 2.0, 3.0));
    }

    [Fact]
    public void EllipsoidWithEqualSemiAxesIsABall()
    {
        var expected = Sphere.Region.MomentOfInertia.FromMass(5.0, 2.0);

        Assert.Equal((expected, expected, expected), Ellipsoid.Region.MomentOfInertia.FromMass(5.0, 2.0, 2.0, 2.0));
    }

    [Fact]
    public void EllipsoidMomentsMatchTheIntegralOfTheSquaredDistance()
    {
        const double a = 1.0, b = 1.5, c = 2.5;
        var integral = InertiaIntegral.OfEllipsoid(a, b, c);
        var (x, y, z) = Ellipsoid.Region.MomentOfInertia.FromDensity(1.0, a, b, c);

        AssertRelative(integral.Volume, Ellipsoid.Region.Mass.FromDensity(1.0, a, b, c));
        AssertRelative(integral.X, x);
        AssertRelative(integral.Y, y);
        AssertRelative(integral.Z, z);
    }
}
