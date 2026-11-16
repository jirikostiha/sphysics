using Xunit;

namespace SPhysics.Tests;

/// <summary> Quantities of a whole system of bodies: kinetic energy, angular momentum, moment of inertia and its radius. </summary>
public class SystemOfBodiesTests
{
    [Fact]
    public void KineticEnergyTotalSumsTheEnergyOfEveryBody()
    {
        (double X, double Y)[] velocities = [(3.0, 4.0), (0.0, -2.0)];
        double[] masses = [2.0, 1.0];

        double energy = KineticEnergy.Total<double>(velocities, masses);

        Assert.Equal(KineticEnergy.Linear(2.0, 5.0) + KineticEnergy.Linear(1.0, 2.0), energy, 10);
    }

    [Fact]
    public void KineticEnergyTotal3DCountsEveryComponent()
    {
        (double X, double Y, double Z)[] velocities = [(1.0, 2.0, 2.0)];
        double[] masses = [4.0];

        Assert.Equal(KineticEnergy.Linear(4.0, 3.0), KineticEnergy.Total<double>(velocities, masses), 10);
    }

    [Fact]
    public void AngularMomentumTotalOfACounterclockwiseOrbitIsPositive()
    {
        // a body circling the origin counterclockwise, L = m * r * v
        (double X, double Y)[] positions = [(2.0, 0.0)];
        (double X, double Y)[] velocities = [(0.0, 3.0)];
        double[] masses = [5.0];

        double momentum = AngularMomentum.Total<double>(positions, velocities, masses, (0.0, 0.0));

        Assert.Equal(AngularMomentum.Eval(5.0, 3.0, 2.0), momentum, 10);
    }

    [Fact]
    public void AngularMomentumTotalAboutTheCenterOfMassIgnoresTheDriftOfTheSystem()
    {
        // two equal bodies circling each other, then the whole pair moving at (7, -4)
        (double X, double Y)[] positions = [(-1.0, 0.0), (1.0, 0.0)];
        (double X, double Y)[] spinning = [(0.0, -1.0), (0.0, 1.0)];
        (double X, double Y)[] drifting = [(7.0, -5.0), (7.0, -3.0)];
        double[] masses = [2.0, 2.0];
        var center = CenterOfMass.Eval<double>(positions, masses);

        double still = AngularMomentum.Total<double>(positions, spinning, masses, center);
        double moving = AngularMomentum.Total<double>(positions, drifting, masses, center);

        Assert.Equal(4.0, still, 10);
        Assert.Equal(still, moving, 10);
    }

    [Fact]
    public void AngularMomentumTotal3DIsTheCrossProductSummed()
    {
        (double X, double Y, double Z)[] positions = [(1.0, 0.0, 0.0), (0.0, 0.0, 2.0)];
        (double X, double Y, double Z)[] velocities = [(0.0, 1.0, 0.0), (1.0, 0.0, 0.0)];
        double[] masses = [3.0, 1.0];

        var (x, y, z) = AngularMomentum.Total<double>(positions, velocities, masses, (0.0, 0.0, 0.0));

        // 3 * (x × y) = (0, 0, 3) ; 1 * (2z × x) = (0, 2, 0)
        Assert.Equal(0.0, x, 10);
        Assert.Equal(2.0, y, 10);
        Assert.Equal(3.0, z, 10);
    }

    [Fact]
    public void MomentOfInertiaTotalAddsThePointMasses()
    {
        (double X, double Y)[] positions = [(4.0, 3.0), (1.0, 1.0)];
        double[] masses = [2.0, 7.0];

        double moment = MomentOfInertia.Total<double>(positions, masses, (1.0, 1.0));

        Assert.Equal(MomentOfInertia.Point(2.0, Math.Sqrt(13.0)), moment, 10);
    }

    [Fact]
    public void MomentOfInertiaPolarOfTwoPointsAboutTheirCenterUsesTheReducedMass()
    {
        (double X, double Y, double Z)[] positions = [(0.0, 0.0, 0.0), (0.0, 0.0, 6.0)];
        double[] masses = [1.0, 2.0];
        var center = CenterOfMass.Eval<double>(positions, masses);

        double moment = MomentOfInertia.Polar<double>(positions, masses, center);

        Assert.Equal(MomentOfInertia.TwoPoints(1.0, 2.0, 6.0), moment, 10);
    }

    [Fact]
    public void RadiusOfGyrationOfARingIsItsRadius()
    {
        (double X, double Y)[] positions = [(5.0, 0.0), (0.0, 5.0), (-5.0, 0.0), (0.0, -5.0)];
        double[] masses = [1.0, 1.0, 1.0, 1.0];

        double moment = MomentOfInertia.Total<double>(positions, masses, (0.0, 0.0));

        Assert.Equal(5.0, RadiusOfGyration.Eval(moment, 4.0), 10);
    }
}
