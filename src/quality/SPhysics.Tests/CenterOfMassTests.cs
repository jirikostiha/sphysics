using Xunit;

namespace SPhysics.Tests;

public class CenterOfMassTests
{
    [Fact]
    public void EvalSumsWeightedPositionsOfAllPoints()
    {
        var points = new (double x, double y, double m)[]
        {
            (1.0, 1.0, 1.0),
            (3.0, 5.0, 3.0)
        };

        var (x, y) = CenterOfMass.Eval(points);

        // ((1*1) + (3*3)) / 4 = 2.5 ; ((1*1) + (5*3)) / 4 = 4
        Assert.Equal(2.5, x, 10);
        Assert.Equal(4.0, y, 10);
    }
}
