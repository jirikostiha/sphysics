using Xunit;

namespace SPhysics.Tests;

public class CenterOfGravityTests
{
    [Fact]
    public void EvalSumsWeightedPositionsOfAllPoints()
    {
        (double X, double Y)[] positions = [(1.0, 1.0), (3.0, 5.0)];
        double[] masses = [1.0, 3.0];
        double[] gravities = [2.0, 2.0];

        var (x, y) = CenterOfGravity.Eval<double>(positions, masses, gravities);

        // ((1*1*2) + (3*3*2)) / 8 = 2.5 ; ((1*1*2) + (5*3*2)) / 8 = 4
        Assert.Equal(2.5, x, 10);
        Assert.Equal(4.0, y, 10);
    }

    [Fact]
    public void EvalLeansToTheStrongerGravity()
    {
        (double X, double Y)[] positions = [(0.0, 0.0), (4.0, 0.0)];
        double[] masses = [1.0, 1.0];
        double[] gravities = [1.0, 3.0];

        var (x, _) = CenterOfGravity.Eval<double>(positions, masses, gravities);

        Assert.Equal(3.0, x, 10);
    }
}
