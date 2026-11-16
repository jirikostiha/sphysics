using Xunit;

namespace SPhysics.Tests;

public class GravitationalForceTests
{
    [Fact]
    public void EvalAvoidsZeroDistance()
    {
        // two points at the same place exert no force on each other
        (double X, double Y)[] positions = [(0.0, 0.0), (0.0, 0.0)];
        double[] masses = [1.0, 1.0];

        var (Fx, Fy) = GravitationalForce.Eval<double>(positions, masses, 0, 1.0);

        Assert.Equal(0.0, Fx);
        Assert.Equal(0.0, Fy);
    }

    [Fact]
    public void EvalUsesInverseSquareLaw()
    {
        // Two unit masses 2 units apart on the x axis, G = 1 => |F| = G*m1*m2/r^2 = 0.25
        (double X, double Y)[] positions = [(0.0, 0.0), (2.0, 0.0)];
        double[] masses = [1.0, 1.0];

        var (Fx, Fy) = GravitationalForce.Eval<double>(positions, masses, 0, 1.0);

        Assert.Equal(0.25, Fx, 10);
        Assert.Equal(0.0, Fy, 10);
    }

    [Fact]
    public void EvalFromTargetPointUsesInverseSquareLaw()
    {
        (double X, double Y)[] positions = [(0.0, 2.0)];
        double[] masses = [1.0];

        var (Fx, Fy) = GravitationalForce.Eval<double>((0.0, 0.0), 1.0, positions, masses, 1.0);

        Assert.Equal(0.0, Fx, 10);
        Assert.Equal(0.25, Fy, 10);
    }

    [Fact]
    public void EvalObeysNewtonsThirdLaw()
    {
        (double X, double Y)[] positions = [(0.0, 0.0), (2.0, 0.0)];
        double[] masses = [3.0, 4.0];

        var (f0x, f0y) = GravitationalForce.Eval<double>(positions, masses, 0, 1.0);
        var (f1x, f1y) = GravitationalForce.Eval<double>(positions, masses, 1, 1.0);

        Assert.Equal(-f0x, f1x, 10);
        Assert.Equal(-f0y, f1y, 10);
    }

    [Fact]
    public void EvalRejectsAMassMissing()
    {
        Assert.Throws<ArgumentException>(() => GravitationalForce.Eval<double>([(0.0, 0.0), (1.0, 0.0)], [1.0], 0, 1.0));
    }

    [Fact]
    public void EvalAgreesWithNBodyGravityAccelerations()
    {
        // F_i = m_i * a_i, checked against the independent NBodyGravity implementation
        const double gravitationConst = 1.0;
        (double X, double Y)[] positions = [(0.0, 0.0), (3.0, 0.0), (0.0, 4.0)];
        double[] masses = [5.0, 2.0, 7.0];
        var accelerations = new (double X, double Y)[3];

        NBodyGravity.ComputeAccelerations<double>(positions, masses, gravitationConst, 0.0, accelerations);

        for (int i = 0; i < positions.Length; i++)
        {
            var (fx, fy) = GravitationalForce.Eval<double>(positions, masses, i, gravitationConst);

            Assert.Equal(masses[i] * accelerations[i].X, fx, 10);
            Assert.Equal(masses[i] * accelerations[i].Y, fy, 10);
        }
    }
}
