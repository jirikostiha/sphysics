using Xunit;

namespace SPhysics.Tests;

public class CenterOfGravityTests
{
    [Fact]
    public void EvalSumsWeightedPositionsOfAllPoints()
    {
        var points = new (double x, double y, double m, double g)[]
        {
            (1.0, 1.0, 1.0, 2.0),
            (3.0, 5.0, 3.0, 2.0)
        };

        var (x, y) = CenterOfGravity.Eval(points);

        // ((1*1*2) + (3*3*2)) / 8 = 2.5 ; ((1*1*2) + (5*3*2)) / 8 = 4
        Assert.Equal(2.5, x, 10);
        Assert.Equal(4.0, y, 10);
    }
}
