using Xunit;

namespace SPhysics.Tests;

public class PressureTests
{
    [Fact]
    public void CentralOfUniformSphereMatchesTheIntegratedHydrostaticEquation()
    {
        const double g = 6.67430e-11;
        const double mass = 5.972e24;
        const double radius = 6.371e6;
        double density = mass / (4.0 / 3.0 * Math.PI * radius * radius * radius);

        // dP/dr = -G m(r) rho / r^2 with m(r) = 4/3 pi rho r^3, so P(0) = 2/3 pi G rho^2 R^2
        double expected = 2.0 / 3.0 * Math.PI * g * density * density * radius * radius;

        Assert.Equal(expected, Pressure.CentralOfUniformSphere(mass, radius, g), expected * 1e-12);
    }

    [Fact]
    public void CentralOfUniformSphereOfTheEarthIsAbout170Gigapascals()
    {
        var pressure = Pressure.CentralOfUniformSphere(5.972e24, 6.371e6, 6.67430e-11);

        Assert.InRange(pressure, 1.6e11, 1.8e11);
    }

    [Fact]
    public void CentralOfUniformSphereScalesWithSquaredMass()
    {
        Assert.Equal(4.0 * Pressure.CentralOfUniformSphere(1.0, 1.0, 1.0), Pressure.CentralOfUniformSphere(2.0, 1.0, 1.0), 12);
    }

    [Fact]
    public void CentralOfUniformSphereScalesWithInverseFourthPowerOfRadius()
    {
        Assert.Equal(Pressure.CentralOfUniformSphere(1.0, 1.0, 1.0) / 16.0, Pressure.CentralOfUniformSphere(1.0, 2.0, 1.0), 12);
    }
}
