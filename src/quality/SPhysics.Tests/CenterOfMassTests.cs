using Xunit;

namespace SPhysics.Tests;

public class CenterOfMassTests
{
    [Fact]
    public void EvalSumsWeightedPositionsOfAllPoints()
    {
        (double X, double Y)[] positions = [(1.0, 1.0), (3.0, 5.0)];
        double[] masses = [1.0, 3.0];

        var (x, y) = CenterOfMass.Eval<double>(positions, masses);

        // ((1*1) + (3*3)) / 4 = 2.5 ; ((1*1) + (5*3)) / 4 = 4
        Assert.Equal(2.5, x, 10);
        Assert.Equal(4.0, y, 10);
    }

    [Fact]
    public void Eval3DSumsWeightedPositionsOfAllPoints()
    {
        (double X, double Y, double Z)[] positions = [(1.0, 1.0, -2.0), (3.0, 5.0, 2.0)];
        double[] masses = [1.0, 3.0];

        var (x, y, z) = CenterOfMass.Eval<double>(positions, masses);

        Assert.Equal(2.5, x, 10);
        Assert.Equal(4.0, y, 10);
        Assert.Equal(1.0, z, 10);
    }

    [Fact]
    public void EvalWithoutMassIsNotANumber()
    {
        var (x, y) = CenterOfMass.Eval(ReadOnlySpan<(double X, double Y)>.Empty, ReadOnlySpan<double>.Empty);

        Assert.True(double.IsNaN(x));
        Assert.True(double.IsNaN(y));
    }

    [Fact]
    public void EvalRejectsAMassMissing()
    {
        Assert.Throws<ArgumentException>(() => CenterOfMass.Eval<double>([(1.0, 1.0), (2.0, 2.0)], [1.0]));
    }

    [Fact]
    public void VelocityIsTheMomentumOverTheMass()
    {
        (double X, double Y, double Z)[] velocities = [(2.0, 0.0, 1.0), (0.0, -1.0, 1.0)];
        double[] masses = [1.0, 3.0];

        var (vx, vy, vz) = CenterOfMass.Velocity<double>(velocities, masses);

        Assert.Equal(0.5, vx, 10);
        Assert.Equal(-0.75, vy, 10);
        Assert.Equal(1.0, vz, 10);
    }
}
