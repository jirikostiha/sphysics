using Xunit;

namespace SPhysics.Tests;

public class GravitationalPotentialTests
{
    [Fact]
    public void SoftenedWithoutSofteningEqualsThePointPotential()
    {
        double softened = GravitationalPotential.Softened(mass: 5.0, distanceSquared: 16.0, softening: 0.0, gravitationConst: 2.0);

        Assert.Equal(GravitationalPotential.Eval(5.0, 4.0, 2.0), softened, 12);
    }

    [Fact]
    public void SoftenedAtTheSourcePointStaysFinite()
    {
        double softened = GravitationalPotential.Softened(mass: 3.0, distanceSquared: 0.0, softening: 0.5, gravitationConst: 1.0);

        // -G * m / softening
        Assert.Equal(-6.0, softened, 12);
    }

    [Fact]
    public void SoftenedIsThePotentialOfTheNBodyAccelerations()
    {
        // The pull NBodyGravity computes has to be minus the gradient of the softened potential.
        const double mass = 7.0;
        const double gravitationConst = 1.5;
        const double softening = 0.3;
        const double x = 1.2;
        const double step = 1e-6;

        var positions = new (double X, double Y)[] { (0.0, 0.0), (x, 0.0) };
        var masses = new[] { mass, 0.0 };
        var accelerations = new (double X, double Y)[2];
        NBodyGravity.ComputeAccelerations<double>(positions, masses, gravitationConst, softening, accelerations);

        double gradient = (Potential(x + step) - Potential(x - step)) / (2 * step);

        Assert.Equal(-gradient, accelerations[1].X, 8);

        static double Potential(double distance)
            => GravitationalPotential.Softened(mass, distance * distance, softening, gravitationConst);
    }
}
