using Xunit;

namespace SPhysics.Tests;

public class MomentumTests
{
    [Fact]
    public void TotalSumsMomentumOfAllBodies()
    {
        var bodies = new (double vx, double vy, double m)[]
        {
            (1.0, 2.0, 3.0),
            (4.0, 5.0, 6.0)
        };

        var (px, py) = Momentum.Total(bodies);

        // (1*3) + (4*6) = 27 ; (2*3) + (5*6) = 36
        Assert.Equal(27.0, px, 10);
        Assert.Equal(36.0, py, 10);
    }
}
