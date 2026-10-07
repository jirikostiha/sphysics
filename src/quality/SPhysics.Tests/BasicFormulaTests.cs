using Xunit;

namespace SPhysics.Tests;

public class BasicFormulaTests
{
    [Fact]
    public void AccelerationFormulas()
    {
        Assert.Equal(5.0, Acceleration.Eval(velocityChange: 10.0, time: 2.0), 10);
        Assert.Equal(2.5, Acceleration.FromForce(force: 10.0, mass: 4.0), 10);
    }

    [Fact]
    public void NewtonsSecondLawRoundTrips()
    {
        var force = Force.FromAcceleration(mass: 4.0, acceleration: 2.5);

        Assert.Equal(10.0, force, 10);
        Assert.Equal(2.5, Acceleration.FromForce(force, 4.0), 10);
        Assert.Equal(4.0, Mass.FromForce(force, 2.5), 10);
    }

    [Fact]
    public void VelocityAndDistanceFormulas()
    {
        Assert.Equal(2.5, Velocity.Eval(distance: 10.0, time: 4.0), 10);
        Assert.Equal(2.5, Velocity.FromMomentum(momentum: 10.0, mass: 4.0), 10);
        Assert.Equal(3.0, Velocity.FromKineticEnergy(kineticEnergy: 18.0, mass: 4.0), 10);
        Assert.Equal(12.0, Distance.FromVelocity(velocity: 3.0, time: 4.0), 10);
    }

    [Fact]
    public void KineticEnergyAndSpeedRoundTrip()
    {
        var energy = KineticEnergy.Linear(mass: 7.35e22, velocity: 1.022e3);

        Assert.Equal(1.022e3, Velocity.FromKineticEnergy(energy, 7.35e22), 9);
    }

    [Fact]
    public void ReducedMassFormulas()
    {
        Assert.Equal(2.4, ReducedMass.Eval(mass1: 4.0, mass2: 6.0), 10);

        // A body much heavier than its partner leaves the partner's own mass.
        Assert.Equal(1.0, ReducedMass.Eval(mass1: 1.0, mass2: 1e15), 10);
    }

    [Fact]
    public void MomentumAndImpulseFormulas()
    {
        Assert.Equal(2.5, Force.FromMomentum(momentumChange: 10.0, time: 4.0), 10);
        Assert.Equal(12.0, Impulse.Eval(force: 3.0, time: 4.0), 10);
    }

    [Fact]
    public void PowerAndPressureFormulas()
    {
        Assert.Equal(2.5, Power.Eval(work: 10.0, time: 4.0), 10);
        Assert.Equal(12.0, Power.FromForce(force: 3.0, velocity: 4.0), 10);
        Assert.Equal(2.5, Pressure.Eval(force: 10.0, area: 4.0), 10);
        Assert.Equal(2.5, AbsorbedDose.Eval(energy: 10.0, mass: 4.0), 10);
    }

    [Fact]
    public void KineticEnergyFromMomentumMatchesLinearForm()
    {
        const double mass = 3.0;
        const double velocity = 4.0;
        var momentum = Momentum.Scalar(mass, velocity);

        Assert.Equal(KineticEnergy.Linear(mass, velocity), KineticEnergy.FromMomentum(momentum, mass), 10);
        Assert.Equal(24.0, KineticEnergy.FromMomentum(momentum, mass), 10);
    }

    [Fact]
    public void PeriodAndFrequencyAreInverses()
    {
        Assert.Equal(0.25, Time.FromFrequency(4.0), 10);
        Assert.Equal(4.0, Frequency.Eval(Time.FromFrequency(4.0)), 10);
    }

    [Fact]
    public void AngularVelocityFrequencyConversions()
    {
        var full = 2 * System.Math.PI;

        Assert.Equal(full, AngularVelocity.FromFrequency(1.0), 10);
        Assert.Equal(System.Math.PI, AngularVelocity.FromPeriod(2.0), 10);
        Assert.Equal(1.0, Frequency.FromAngularVelocity(full), 10);
    }
}
