using Xunit;

namespace SPhysics.Tests;

public class GravitationalEnergyTests
{
    [Fact]
    public void TotalUsesDistanceNotSquaredDistance()
    {
        var points = new (double X, double Y, double Mass)[]
        {
            (0.0, 0.0, 1.0),
            (2.0, 0.0, 1.0)
        };

        var energy = GravitationalEnergy.Total<double>(points, 1.0);

        // -G * m1 * m2 / r = -1 * 1 * 1 / 2
        Assert.Equal(-0.5, energy, 10);
    }

    [Fact]
    public void SatisfiesTheVirialTheoremOnACircularOrbit()
    {
        // for a circular orbit 2K + U = 0; with U ~ 1/r^2 it would be 9900 instead
        const double gravitationConst = 1.0;
        const double centralMass = 1.0e6;
        const double orbitingMass = 1.0;
        const double radius = 100.0;

        var speed = System.Math.Sqrt(gravitationConst * centralMass / radius);
        var kinetic = KineticEnergy.Linear(orbitingMass, speed);
        var points = new (double X, double Y, double Mass)[]
        {
            (0.0, 0.0, centralMass),
            (radius, 0.0, orbitingMass)
        };

        var potential = GravitationalEnergy.Total<double>(points, gravitationConst);

        Assert.Equal(0.0, 2 * kinetic + potential, 10);
        Assert.Equal(-gravitationConst * centralMass * orbitingMass / (2 * radius), kinetic + potential, 10);
    }

    [Fact]
    public void TotalEnergyIsConservedAlongAnEccentricOrbit()
    {
        // the separation sweeps from 100 down to ~32, so an energy built on r^2
        // instead of r swings by several hundred percent over the same run
        const double gravitationConst = 1.0;
        var masses = new[] { 1.0e6, 1.0 };
        var positions = new (double X, double Y)[] { (0.0, 0.0), (100.0, 0.0) };
        var velocities = new (double X, double Y)[] { (0.0, 0.0), (0.0, 70.0) };
        var accelerations = new (double X, double Y)[2];

        var initial = TotalEnergy(positions, velocities, masses, gravitationConst);

        for (int step = 0; step < 20; step++)
        {
            VelocityVerlet.StepGravity<double>(
                positions, velocities, accelerations, masses,
                gravitationConst, softening: 0.0, timeDelta: 0.5, subSteps: 100);
        }

        var final = TotalEnergy(positions, velocities, masses, gravitationConst);

        Assert.Equal(0.0, System.Math.Abs((final - initial) / initial), 5);
    }

    private static double TotalEnergy(
        (double X, double Y)[] positions,
        (double X, double Y)[] velocities,
        double[] masses,
        double gravitationConst)
    {
        var kinetic = 0.0;
        var points = new (double X, double Y, double Mass)[masses.Length];

        for (int i = 0; i < masses.Length; i++)
        {
            var speed = System.Math.Sqrt(velocities[i].X * velocities[i].X + velocities[i].Y * velocities[i].Y);

            kinetic += KineticEnergy.Linear(masses[i], speed);
            points[i] = (positions[i].X, positions[i].Y, masses[i]);
        }

        return kinetic + GravitationalEnergy.Total<double>(points, gravitationConst);
    }
}
