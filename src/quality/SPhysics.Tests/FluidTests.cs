using Xunit;

namespace SPhysics.Tests;

public class FluidTests
{
    private const double WaterDensity = 1_000.0;
    private const double AirDensity = 1.225;
    private const double G = 9.81;

    [Fact]
    public void BuoyantForceEqualsWeightOfDisplacedFluid()
    {
        // A litre of water displaced weighs 9.81 N
        Assert.Equal(9.81, BuoyantForce.Eval(fluidDensity: WaterDensity, gravity: G, displacedVolume: 0.001), 10);
    }

    [Fact]
    public void HydrostaticPressureAtTenMetresOfWaterIsAboutOneAtmosphere()
    {
        Assert.Equal(98_100.0, Pressure.Hydrostatic(fluidDensity: WaterDensity, gravity: G, depth: 10.0), 10);
    }

    [Fact]
    public void HydrostaticPressureTimesAreaIsBuoyancyOfSubmergedColumn()
    {
        // The pressure difference over the height of a submerged column, times its base, is its buoyancy
        const double area = 0.3, top = 2.0, height = 0.5;
        var lift = (Pressure.Hydrostatic(WaterDensity, G, top + height) - Pressure.Hydrostatic(WaterDensity, G, top)) * area;

        Assert.Equal(BuoyantForce.Eval(WaterDensity, G, area * height), lift, 8);
    }

    [Fact]
    public void DragAtTerminalVelocityBalancesWeight()
    {
        const double mass = 80.0, area = 0.7, dragCoef = 1.0;
        var terminal = Velocity.Terminal(mass, G, AirDensity, area, dragCoef);

        Assert.Equal(mass * G, DragForce.InFluid(AirDensity, terminal, area, dragCoef), 8);
    }

    [Fact]
    public void ReynoldsNumberOfWaterInPipe()
    {
        // Water at 20 °C, 1 m/s in a pipe of 0.1 m diameter
        Assert.Equal(99_800.0, ReynoldsNumber.Eval(fluidDensity: 998.0, flowVelocity: 1.0, characteristicLength: 0.1, dynamicViscosity: 1.0e-3), 6);
    }

    [Fact]
    public void FormulasAreGenericOverFloat()
    {
        Assert.Equal(98_100f, Pressure.Hydrostatic(1_000f, 9.81f, 10f), 1);
        Assert.Equal(9.81f, BuoyantForce.Eval(1_000f, 9.81f, 0.001f), 4);
    }
}
