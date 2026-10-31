using Xunit;

namespace SPhysics.Tests;

public class RotationTests
{
    private const int Precision = 12;

    [Fact]
    public void TorqueFormulas()
    {
        Assert.Equal(12.0, Torque.Eval(force: 3.0, leverArm: 4.0), Precision);
        Assert.Equal(6.0, Torque.Eval(force: 3.0, leverArm: 4.0, angle: Math.PI / 6), Precision);
        Assert.Equal(12.0, Torque.FromAngularAcceleration(momentOfInertia: 3.0, angularAcceleration: 4.0), Precision);
        Assert.Equal(2.5, Torque.FromAngularMomentum(angularMomentumChange: 10.0, time: 4.0), Precision);
    }

    [Fact]
    public void PerpendicularForceGivesTheFullTorque()
    {
        Assert.Equal(Torque.Eval(3.0, 4.0), Torque.Eval(3.0, 4.0, Math.PI / 2), Precision);
    }

    [Fact]
    public void AngularAccelerationFormulas()
    {
        Assert.Equal(5.0, AngularAcceleration.Eval(angularVelocityChange: 10.0, time: 2.0), Precision);
        Assert.Equal(2.5, AngularAcceleration.FromTorque(torque: 10.0, momentOfInertia: 4.0), Precision);
        Assert.Equal(2.5, AngularAcceleration.FromTangentialAcceleration(tangentialAcceleration: 10.0, radius: 4.0), Precision);
    }

    [Fact]
    public void NewtonsSecondLawForRotationRoundTrips()
    {
        var torque = Torque.FromAngularAcceleration(momentOfInertia: 4.0, angularAcceleration: 2.5);

        Assert.Equal(2.5, AngularAcceleration.FromTorque(torque, 4.0), Precision);
    }

    [Fact]
    public void WorkFormulas()
    {
        Assert.Equal(12.0, Work.Eval(force: 3.0, displacement: 4.0), Precision);
        Assert.Equal(6.0, Work.Eval(force: 3.0, displacement: 4.0, angle: Math.PI / 3), Precision);
        Assert.Equal(12.0, Work.FromTorque(torque: 3.0, angle: 4.0), Precision);
        Assert.Equal(12.0, Work.FromPower(power: 3.0, time: 4.0), Precision);
    }

    [Fact]
    public void ForcePerpendicularToTheDisplacementDoesNoWork()
    {
        Assert.Equal(0.0, Work.Eval(3.0, 4.0, Math.PI / 2), Precision);
    }

    [Fact]
    public void WorkOfATorqueOnABodyFromRestIsItsSpinningKineticEnergy()
    {
        // constant torque on a body at rest over a time span: theta = alpha t^2 / 2, omega = alpha t
        const double inertia = 2.0, torque = 3.0, time = 4.0;
        double alpha = AngularAcceleration.FromTorque(torque, inertia);
        double omega = alpha * time;
        double angle = alpha * time * time / 2;

        Assert.Equal(KineticEnergy.Spinning(inertia, omega), Work.FromTorque(torque, angle), Precision);
    }

    [Fact]
    public void PowerOfATorque()
    {
        Assert.Equal(12.0, Power.FromTorque(torque: 3.0, angularVelocity: 4.0), Precision);
    }

    [Fact]
    public void ElasticPotentialEnergyIsHalfTheSpringConstantTimesTheSquaredDisplacement()
    {
        Assert.Equal(36.0, PotentialEnergy.Elastic(springConstant: 8.0, displacement: 3.0), Precision);
        Assert.Equal(36.0, PotentialEnergy.Elastic(springConstant: 8.0, displacement: -3.0), Precision);
    }

    [Fact]
    public void CentripetalAccelerationIsTheSquaredSpeedOverTheRadius()
    {
        Assert.Equal(4.5, Acceleration.Centripetal(velocity: 3.0, radius: 2.0), Precision);
    }

    [Fact]
    public void CentripetalAccelerationEqualsTheSquaredAngularVelocityTimesTheRadius()
    {
        const double omega = 1.5, radius = 2.0;

        Assert.Equal(omega * omega * radius, Acceleration.Centripetal(Velocity.FromAngularVelocity(omega, radius), radius), Precision);
    }
}
