using SMath;
using SMath.Geometry3D;

using Xunit;

namespace SPhysics.Tests;

public class VelocityVerletTests
{
    [Fact]
    public void HalfKickAndDrift3DAdvanceEveryAxis()
    {
        (double X, double Y, double Z)[] positions = [(1.0, 2.0, 3.0)];
        (double X, double Y, double Z)[] velocities = [(0.0, 0.0, 1.0)];
        (double X, double Y, double Z)[] accelerations = [(2.0, 0.0, -4.0)];

        var kicked = VelocityVerlet.HalfKick(velocities[0], accelerations[0], 0.5);
        VelocityVerlet.HalfKick<double>(velocities, accelerations, 0.5);
        var drifted = VelocityVerlet.Drift(positions[0], velocities[0], 2.0);
        VelocityVerlet.Drift<double>(positions, velocities, 2.0);

        Assert.Equal((0.5, 0.0, 0.0), velocities[0]);
        Assert.Equal((2.0, 2.0, 3.0), positions[0]);
        Assert.Equal(kicked, velocities[0]);
        Assert.Equal(drifted, positions[0]);
    }

    [Fact]
    public void HalfKickAndDrift3DMismatchedLengthsThrow()
    {
        Assert.Throws<ArgumentException>(() => VelocityVerlet.HalfKick<double>(new (double, double, double)[2], new (double, double, double)[1], 1.0));
        Assert.Throws<ArgumentException>(() => VelocityVerlet.Drift<double>(new (double, double, double)[2], new (double, double, double)[1], 1.0));
    }

    [Fact]
    public void StepGravity3DInclinedCircularOrbitKeepsTheRadiusAndTheEnergy()
    {
        // A light body on a circular orbit around a heavy one, in a plane tilted out of XY.
        const double mass = 1000.0;
        double speed = Math.Sqrt(mass / 10.0);
        (double X, double Y, double Z)[] positions = [(0.0, 0.0, 0.0), (10.0, 0.0, 0.0)];
        (double X, double Y, double Z)[] velocities = [(0.0, 0.0, 0.0), (0.0, speed * 0.6, speed * 0.8)];
        double[] masses = [mass, 1e-9];
        var accelerations = new (double X, double Y, double Z)[2];

        VelocityVerlet.StepGravity<double>(positions, velocities, accelerations, masses, 1.0, 0.0, 2.0, subSteps: 2000);

        Assert.Equal(10.0, Point3.Distance(positions[0], positions[1]), 1e-4);
        Assert.Equal(speed, PT.Hypotenuse(velocities[1]), 1e-4);
        double offPlane = ((positions[1].Y - positions[0].Y) * -0.8) + ((positions[1].Z - positions[0].Z) * 0.6);
        Assert.Equal(0.0, offPlane, 1e-9); // stays in the orbital plane
    }

    [Fact]
    public void StepGravity3DNoSubStepsThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            VelocityVerlet.StepGravity<double>(new (double, double, double)[2], new (double, double, double)[2], new (double, double, double)[2], [1.0, 1.0], 1.0, 0.0, 1.0, subSteps: 0));
    }

    [Fact]
    public void SoaVerletStepsAreBitwiseIdenticalToTheTupleSteps()
    {
        var random = new Random(9);
        int n = 37;
        float Value() => (float)((random.NextDouble() * 200) - 100);
        (float X, float Y, float Z)[] Vectors() => Enumerable.Range(0, n).Select(_ => (Value(), Value(), Value())).ToArray();

        var positions = Vectors();
        var velocities = Vectors();
        var accelerations = Vectors();
        var vx = velocities.Select(v => v.X).ToArray();
        var px = positions.Select(p => p.X).ToArray();

        VelocityVerlet.HalfKick<float>(velocities, accelerations, 0.37f);
        VelocityVerlet.Drift<float>(positions, velocities, 0.37f);
        VelocityVerlet.Soa.HalfKick<float>(vx, accelerations.Select(a => a.X).ToArray(), 0.37f);
        VelocityVerlet.Soa.Drift<float>(px, vx, 0.37f);

        Assert.Equal(velocities.Select(v => BitConverter.SingleToInt32Bits(v.X)), vx.Select(BitConverter.SingleToInt32Bits));
        Assert.Equal(positions.Select(p => BitConverter.SingleToInt32Bits(p.X)), px.Select(BitConverter.SingleToInt32Bits));
        Assert.Throws<ArgumentException>(() => VelocityVerlet.Soa.Drift<float>(px, new float[n - 1], 1f));
    }
}
