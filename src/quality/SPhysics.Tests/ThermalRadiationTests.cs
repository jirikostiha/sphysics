using Xunit;

namespace SPhysics.Tests;

public class ThermalRadiationTests
{
    [Fact]
    public void PowerFollowsTheFourthPowerOfTheTemperature()
    {
        Assert.Equal(0.5 * 2.0 * 3.0 * 16.0, ThermalRadiation.Power(3.0, 2.0, 0.5, 2.0), 12);
        Assert.Equal(16.0 * ThermalRadiation.Power(1.0, 1.0, 1.0, 1.0), ThermalRadiation.Power(1.0, 2.0, 1.0, 1.0), 12);
    }

    [Fact]
    public void TemperatureFromPowerInvertsThePower()
    {
        var power = ThermalRadiation.Power(4.0, 300.0, 0.9, ThermalRadiation.StefanBoltzmannConstant);

        Assert.Equal(300.0, ThermalRadiation.TemperatureFromPower(power, 4.0, 0.9, ThermalRadiation.StefanBoltzmannConstant), 9);
    }

    [Fact]
    public void IrradianceFallsWithTheSquareOfTheDistance()
    {
        Assert.Equal(1.0, ThermalRadiation.Irradiance(4.0 * Math.PI, 1.0), 12);
        Assert.Equal(0.25, ThermalRadiation.Irradiance(4.0 * Math.PI, 2.0), 12);
    }

    [Fact]
    public void EquilibriumTemperatureOfTheEarthIsAbout254Kelvin()
    {
        // solar constant 1361 W/m^2 and the Bond albedo of the Earth 0.306
        var temperature = ThermalRadiation.EquilibriumTemperature(1361.0, 0.306, ThermalRadiation.StefanBoltzmannConstant);

        Assert.Equal(254.0, temperature, 0);
    }

    [Fact]
    public void EquilibriumTemperatureBalancesTheAbsorbedAndTheRadiatedPower()
    {
        const double radius = 2.0;
        const double irradiance = 50.0;
        const double albedo = 0.3;

        var temperature = ThermalRadiation.EquilibriumTemperature(irradiance, albedo, 1.0);

        double absorbed = (1.0 - albedo) * irradiance * Math.PI * radius * radius;
        double radiated = ThermalRadiation.Power(4.0 * Math.PI * radius * radius, temperature, 1.0, 1.0);
        Assert.Equal(absorbed, radiated, 10);
    }
}
