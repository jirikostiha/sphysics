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
}
