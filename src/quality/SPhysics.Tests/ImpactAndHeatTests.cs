using SMath.Geometry3D;
using Xunit;

namespace SPhysics.Tests;

/// <summary> Restitution, perfectly inelastic collisions, recoil, heat and the volume of a mass. </summary>
public class ImpactAndHeatTests
{
    private const int Precision = 12;

    [Fact]
    public void ElasticImpactReversesTheNormalVelocity()
    {
        Assert.Equal(-6.0, Restitution.NormalVelocityChange(1.0, -3.0), Precision);
        Assert.Equal(-3.0, Restitution.NormalVelocityChange(0.0, -3.0), Precision);
    }

    [Fact]
    public void ImpulseConservesMomentumAndKeepsTheRestitutionOfTheRelativeSpeed()
    {
        // head-on: body 1 at 4 against body 2 at rest, e = 0.5
        const double mass1 = 2.0, mass2 = 6.0, restitution = 0.5;
        double approach = 0.0 - 4.0;
        double impulse = Restitution.Impulse(restitution, ReducedMass.Eval(mass1, mass2), approach);

        double velocity1 = 4.0 + (impulse / mass1);
        double velocity2 = 0.0 - (impulse / mass2);

        Assert.Equal(2.0 * 4.0, (mass1 * velocity1) + (mass2 * velocity2), Precision);
        Assert.Equal(restitution * 4.0, velocity2 - velocity1, Precision);
    }

    [Fact]
    public void RetainedAndAbsorbedSharesMakeTheWholeEnergy()
    {
        Assert.Equal(0.36, Restitution.RetainedEnergyShare(0.6), Precision);
        Assert.Equal(1.0, Restitution.RetainedEnergyShare(0.6) + Restitution.AbsorbedEnergyShare(0.6), Precision);
    }

    [Fact]
    public void AbsorbedShareIsTheEnergyAnImpactLosesInTheFrameOfTheCenterOfMass()
    {
        const double mass1 = 2.0, mass2 = 6.0, restitution = 0.5;
        double impulse = Restitution.Impulse(restitution, ReducedMass.Eval(mass1, mass2), -4.0);
        double before = KineticEnergy.Linear(mass1, 4.0);
        double after = KineticEnergy.Linear(mass1, 4.0 + (impulse / mass1)) + KineticEnergy.Linear(mass2, -impulse / mass2);

        Assert.Equal(Restitution.AbsorbedEnergyShare(restitution) * KineticEnergy.Linear(ReducedMass.Eval(mass1, mass2), 4.0), before - after, Precision);
    }

    [Fact]
    public void MergedBodyKeepsTheMomentum()
    {
        var velocity = InelasticCollision.MergedVelocity((4.0, 0.0), 1.0, (0.0, 2.0), 3.0);

        Assert.Equal(1.0, velocity.X, Precision);
        Assert.Equal(1.5, velocity.Y, Precision);
    }

    [Fact]
    public void MergingTurnsTheRelativeMotionIntoHeat()
    {
        (double X, double Y, double Z) velocity1 = (4.0, 0.0, 1.0), velocity2 = (0.0, 2.0, 1.0);
        var merged = InelasticCollision.MergedVelocity(velocity1, 1.0, velocity2, 3.0);
        double before = KineticEnergy.Linear(1.0, velocity1) + KineticEnergy.Linear(3.0, velocity2);

        Assert.Equal(before - KineticEnergy.Linear(4.0, merged), InelasticCollision.EnergyLoss(velocity1, 1.0, velocity2, 3.0), Precision);
    }

    [Fact]
    public void CenterOfMassOfTwoPointsLiesOnTheLineBetweenThem()
    {
        var center = CenterOfMass.Eval((0.0, 0.0, 0.0), 3.0, (4.0, 8.0, -4.0), 1.0);

        Assert.Equal((1.0, 2.0, -1.0), center);
    }

    [Fact]
    public void VectorKineticEnergyEqualsTheOneOfTheSpeed()
    {
        Assert.Equal(KineticEnergy.Linear(2.0, 5.0), KineticEnergy.Linear(2.0, (3.0, 4.0)), Precision);
    }

    [Fact]
    public void RecoilCarriesTheNegatedMomentum()
    {
        var recoil = Velocity.FromMomentum((-6.0, 3.0), 3.0);

        Assert.Equal((-2.0, 1.0), recoil);
    }

    [Fact]
    public void HeatAndTemperatureChangeAreInverse()
    {
        double heat = Heat.FromTemperatureChange(2.0, 900.0, 10.0);

        Assert.Equal(18000.0, heat, Precision);
        Assert.Equal(10.0, Heat.TemperatureChange(heat, 2.0, 900.0), Precision);
    }

    [Fact]
    public void VolumeIsTheInverseOfTheMass()
    {
        Assert.Equal(0.5, Volume.FromMass(Mass.FromVolume(0.5, 3000.0), 3000.0), Precision);
    }

    [Fact]
    public void BallRadiusIsTheInverseOfItsMass()
    {
        double mass = Sphere.Region.Mass.FromDensity(5500.0, 6.371e6);

        Assert.Equal(1.0, Sphere.Region.Radius.FromMass(mass, 5500.0) / 6.371e6, Precision);
    }
}
