using Xunit;

namespace SPhysics.Tests;

public class GravitationalEnergyTests
{
    [Fact]
    public void TotalUsesDistanceNotSquaredDistance()
    {
        (double X, double Y)[] positions = [(0.0, 0.0), (2.0, 0.0)];
        double[] masses = [1.0, 1.0];

        var energy = GravitationalEnergy.Total<double>(positions, masses, 1.0, 0.0);

        // -G * m1 * m2 / r = -1 * 1 * 1 / 2
        Assert.Equal(-0.5, energy, 10);
    }

    [Fact]
    public void SoftenedTotalTakesTheSofteningIntoTheDistance()
    {
        (double X, double Y, double Z)[] positions = [(0.0, 0.0, 0.0), (3.0, 0.0, 0.0), (3.0, 0.0, 0.0)];
        double[] masses = [2.0, 1.0, 1.0];

        var energy = GravitationalEnergy.Total<double>(positions, masses, 1.0, 4.0);

        // two pairs 3 apart and one pair at the same place: 2 * (-2 / 5) - 1 / 4
        Assert.Equal(-1.05, energy, 10);
        Assert.Equal(-0.4, GravitationalEnergy.Softened(2.0, 1.0, 9.0, 4.0, 1.0), 10);
    }

    [Fact]
    public void TotalWithoutSofteningLeavesOutBodiesAtTheSamePlace()
    {
        (double X, double Y)[] positions = [(0.0, 0.0), (0.0, 0.0), (1.0, 0.0)];
        double[] masses = [1.0, 1.0, 1.0];

        Assert.Equal(-2.0, GravitationalEnergy.Total<double>(positions, masses, 1.0, 0.0), 10);
    }

    [Fact]
    public void SatisfiesTheVirialTheoremOnACircularOrbit()
    {
        // for a circular orbit 2K + U = 0; with U ~ 1/r^2 it would be 9900 instead
        const double gravitationalConstant = 1.0;
        const double centralMass = 1.0e6;
        const double orbitingMass = 1.0;
        const double radius = 100.0;

        var speed = System.Math.Sqrt(gravitationalConstant * centralMass / radius);
        var kinetic = KineticEnergy.Linear(orbitingMass, speed);
        (double X, double Y)[] positions = [(0.0, 0.0), (radius, 0.0)];
        double[] masses = [centralMass, orbitingMass];

        var potential = GravitationalEnergy.Total<double>(positions, masses, gravitationalConstant, 0.0);

        Assert.Equal(0.0, 2 * kinetic + potential, 10);
        Assert.Equal(-gravitationalConstant * centralMass * orbitingMass / (2 * radius), kinetic + potential, 10);
    }

    [Fact]
    public void TotalEnergyIsConservedAlongAnEccentricOrbit()
    {
        // the separation sweeps from 100 down to ~32, so an energy built on r^2
        // instead of r swings by several hundred percent over the same run
        const double gravitationalConstant = 1.0;
        var masses = new[] { 1.0e6, 1.0 };
        var positions = new (double X, double Y)[] { (0.0, 0.0), (100.0, 0.0) };
        var velocities = new (double X, double Y)[] { (0.0, 0.0), (0.0, 70.0) };
        var accelerations = new (double X, double Y)[2];

        var initial = TotalEnergy(positions, velocities, masses, gravitationalConstant);

        for (int step = 0; step < 20; step++)
        {
            VelocityVerlet.StepGravity<double>(
                positions, velocities, accelerations, masses,
                gravitationalConstant, softening: 0.0, timeDelta: 0.5, subSteps: 100);
        }

        var final = TotalEnergy(positions, velocities, masses, gravitationalConstant);

        Assert.Equal(0.0, System.Math.Abs((final - initial) / initial), 5);
    }

    private static double TotalEnergy(
        (double X, double Y)[] positions,
        (double X, double Y)[] velocities,
        double[] masses,
        double gravitationalConstant)
    {
        return KineticEnergy.Total<double>(velocities, masses)
            + GravitationalEnergy.Total<double>(positions, masses, gravitationalConstant, 0.0);
    }
}
