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

    [Fact]
    public void CentralPressureMatchesTheIntegratedHydrostaticEquation()
    {
        const double g = 6.67430e-11;
        const double mass = 5.972e24;
        const double radius = 6.371e6;
        double density = Density.Average(Sphere.Region.Volume.FromRadius(radius), mass);

        // dP/dr = -G m(r) rho / r^2 with m(r) = 4/3 pi rho r^3, so P(0) = 2/3 pi G rho^2 R^2
        double expected = 2.0 / 3.0 * Math.PI * g * density * density * radius * radius;

        Assert.Equal(expected, Sphere.Region.CentralPressure.FromMass(mass, radius, g), expected * 1e-12);
    }

    [Fact]
    public void CentralPressureOfTheEarthIsAbout170Gigapascals()
    {
        var pressure = Sphere.Region.CentralPressure.FromMass(5.972e24, 6.371e6, 6.67430e-11);

        Assert.InRange(pressure, 1.6e11, 1.8e11);
    }

    [Fact]
    public void CentralPressureScalesWithSquaredMass()
    {
        Assert.Equal(4.0 * Sphere.Region.CentralPressure.FromMass(1.0, 1.0, 1.0), Sphere.Region.CentralPressure.FromMass(2.0, 1.0, 1.0), Precision);
    }

    [Fact]
    public void CentralPressureScalesWithInverseFourthPowerOfRadius()
    {
        Assert.Equal(Sphere.Region.CentralPressure.FromMass(1.0, 1.0, 1.0) / 16.0, Sphere.Region.CentralPressure.FromMass(1.0, 2.0, 1.0), Precision);
    }

    [Fact]
    public void GravitationalEnergyHoldsThreeFifthsOfThePointEnergy()
    {
        // -3/5 * G * m^2 / r = -0.6 * 1.5 * 2 * 2 / 4
        Assert.Equal(-0.9, Sphere.Region.GravitationalEnergy.FromMass(mass: 2.0, radius: 4.0, gravitationConst: 1.5), Precision);
    }

    [Fact]
    public void GravitationalEnergyGivesTheBindingEnergyOfTheEarth()
    {
        double energy = Sphere.Region.GravitationalEnergy.FromMass(mass: 5.972e24, radius: 6.371e6, gravitationConst: 6.6743e-11);

        // About 2.24e32 J for a uniform Earth.
        Assert.Equal(-2.2417e32, energy, tolerance: 0.0001e32);
    }

    [Fact]
    public void EquilibriumTemperatureOfTheEarthIsAbout254Kelvin()
    {
        // solar constant 1361 W/m^2 and the Bond albedo of the Earth 0.306
        var temperature = Sphere.Region.EquilibriumTemperature.FromIrradiance(1361.0, 0.306, ThermalRadiation.StefanBoltzmannConstant);

        Assert.Equal(254.0, temperature, 0);
    }

    [Fact]
    public void EquilibriumTemperatureBalancesTheAbsorbedAndTheRadiatedPower()
    {
        const double radius = 2.0;
        const double irradiance = 50.0;
        const double albedo = 0.3;

        var temperature = Sphere.Region.EquilibriumTemperature.FromIrradiance(irradiance, albedo, 1.0);

        double absorbed = (1.0 - albedo) * irradiance * Math.PI * radius * radius;
        double radiated = ThermalRadiation.Power(4.0 * Math.PI * radius * radius, temperature, 1.0, 1.0);
        Assert.Equal(absorbed, radiated, 10);
    }
}
