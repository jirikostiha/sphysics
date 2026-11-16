using SMath.Geometry3D;
using Xunit;

namespace SPhysics.Tests;

/// <summary> Gravitational field of a uniform ball, inside it and around it. </summary>
public class BallGravityTests
{
    private const int Precision = 12;

    [Fact]
    public void IntensityOutsideIsThatOfAPointMass()
    {
        Assert.Equal(GravitationalIntensity.Scalar(8.0, 4.0, 2.0), Sphere.Region.GravitationalIntensity.FromMass(8.0, 2.0, 4.0, 2.0), Precision);
    }

    [Fact]
    public void IntensityInsideGrowsLinearlyFromTheCentre()
    {
        // only the mass within half the radius, an eighth, attracts at half the radius
        double surface = Sphere.Region.GravitationalIntensity.FromMass(8.0, 2.0, 2.0, 1.0);
        double half = Sphere.Region.GravitationalIntensity.FromMass(8.0, 2.0, 1.0, 1.0);

        Assert.Equal(0.0, Sphere.Region.GravitationalIntensity.FromMass(8.0, 2.0, 0.0, 1.0), Precision);
        Assert.Equal(surface / 2.0, half, Precision);
        Assert.Equal(GravitationalIntensity.Scalar(1.0, 1.0, 1.0), half, Precision);
    }

    [Fact]
    public void IntensityIsContinuousAtTheSurface()
    {
        double inside = Sphere.Region.GravitationalIntensity.FromMass(5.0, 3.0, 3.0 - 1e-9, 1.0);
        double outside = Sphere.Region.GravitationalIntensity.FromMass(5.0, 3.0, 3.0, 1.0);

        Assert.Equal(outside, inside, 8);
    }

    [Fact]
    public void PotentialOutsideIsThatOfAPointMass()
    {
        Assert.Equal(GravitationalPotential.Eval(8.0, 4.0, 2.0), Sphere.Region.GravitationalPotential.FromMass(8.0, 2.0, 4.0, 2.0), Precision);
    }

    [Fact]
    public void PotentialAtTheCentreIsOneAndAHalfTimesThatOnTheSurface()
    {
        double surface = Sphere.Region.GravitationalPotential.FromMass(6.0, 2.0, 2.0, 1.0);

        Assert.Equal(1.5 * surface, Sphere.Region.GravitationalPotential.FromMass(6.0, 2.0, 0.0, 1.0), Precision);
    }

    [Fact]
    public void PotentialIsContinuousAtTheSurface()
    {
        double inside = Sphere.Region.GravitationalPotential.FromMass(5.0, 3.0, 3.0 - 1e-9, 1.0);
        double outside = Sphere.Region.GravitationalPotential.FromMass(5.0, 3.0, 3.0, 1.0);

        Assert.Equal(outside, inside, 8);
    }

    [Fact]
    public void IntensityInsideIsTheSlopeOfThePotential()
    {
        // g = -dphi/dr, checked by a central difference halfway to the surface
        const double step = 1e-6;
        double above = Sphere.Region.GravitationalPotential.FromMass(5.0, 3.0, 1.5 + step, 1.0);
        double below = Sphere.Region.GravitationalPotential.FromMass(5.0, 3.0, 1.5 - step, 1.0);

        Assert.Equal(-(above - below) / (2 * step), Sphere.Region.GravitationalIntensity.FromMass(5.0, 3.0, 1.5, 1.0), 6);
    }
}
