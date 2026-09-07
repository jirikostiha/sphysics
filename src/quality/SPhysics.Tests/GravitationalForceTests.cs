using Xunit;

namespace SPhysics.Tests;

public class GravitationalForceTests
{
    [Fact]
    public void EvalAvoidsZeroDistance()
    {
        // Arrange: Set up test data with two points at the same location (0, 0)
        var points = new (double X, double Y, double Mass)[]
        {
            (0.0, 0.0, 1.0),  // First point (mass = 1.0 at origin)
            (0.0, 0.0, 1.0)   // Second point (mass = 1.0 at origin, same position)
        };

        int index = 0;  // We will calculate the force for the first point (0.0, 0.0)
        double gravitationConst = 1.0;  // Assume G = 1.0 for simplicity

        // Act: Call the Eval method
        var (Fx, Fy) = GravitationalForce.Eval(points, index, gravitationConst);

        // Assert: Verify that the gravitational force is not calculated for two points at the same position
        Assert.Equal(0.0, Fx);
        Assert.Equal(0.0, Fy);
    }

    [Fact]
    public void EvalUsesInverseSquareLaw()
    {
        // Two unit masses 2 units apart on the x axis, G = 1 => |F| = G*m1*m2/r^2 = 0.25
        var points = new (double X, double Y, double Mass)[]
        {
            (0.0, 0.0, 1.0),
            (2.0, 0.0, 1.0)
        };

        var (Fx, Fy) = GravitationalForce.Eval(points, 0, 1.0);

        Assert.Equal(0.25, Fx, 10);
        Assert.Equal(0.0, Fy, 10);
    }

    [Fact]
    public void EvalFromTargetPointUsesInverseSquareLaw()
    {
        var points = new (double X, double Y, double Mass)[]
        {
            (0.0, 2.0, 1.0)
        };

        var target = (X: 0.0, Y: 0.0, Mass: 1.0);
        var (Fx, Fy) = GravitationalForce.Eval<double>(target, points, 1.0);

        Assert.Equal(0.0, Fx, 10);
        Assert.Equal(0.25, Fy, 10);
    }

    [Fact]
    public void EvalObeysNewtonsThirdLaw()
    {
        var points = new (double X, double Y, double Mass)[]
        {
            (0.0, 0.0, 3.0),
            (2.0, 0.0, 4.0)
        };

        var (f0x, f0y) = GravitationalForce.Eval(points, 0, 1.0);
        var (f1x, f1y) = GravitationalForce.Eval(points, 1, 1.0);

        Assert.Equal(-f0x, f1x, 10);
        Assert.Equal(-f0y, f1y, 10);
    }

    [Fact]
    public void EvalAgreesWithNBodyGravityAccelerations()
    {
        // F_i = m_i * a_i, checked against the independent NBodyGravity implementation
        const double gravitationConst = 1.0;
        var points = new (double X, double Y, double Mass)[]
        {
            (0.0, 0.0, 5.0),
            (3.0, 0.0, 2.0),
            (0.0, 4.0, 7.0)
        };
        var positions = new (double X, double Y)[] { (0.0, 0.0), (3.0, 0.0), (0.0, 4.0) };
        var masses = new[] { 5.0, 2.0, 7.0 };
        var accelerations = new (double X, double Y)[3];

        NBodyGravity.ComputeAccelerations<double>(positions, masses, gravitationConst, 0.0, accelerations);

        for (int i = 0; i < points.Length; i++)
        {
            var (fx, fy) = GravitationalForce.Eval(points, i, gravitationConst);

            Assert.Equal(masses[i] * accelerations[i].X, fx, 10);
            Assert.Equal(masses[i] * accelerations[i].Y, fy, 10);
        }
    }
}
