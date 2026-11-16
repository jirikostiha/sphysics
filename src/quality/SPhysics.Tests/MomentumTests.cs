using Xunit;

namespace SPhysics.Tests;

public class MomentumTests
{
    [Fact]
    public void TotalSumsMomentumOfAllBodies()
    {
        (double X, double Y)[] velocities = [(1.0, 2.0), (4.0, 5.0)];
        double[] masses = [3.0, 6.0];

        var (px, py) = Momentum.Total<double>(velocities, masses);

        // (1*3) + (4*6) = 27 ; (2*3) + (5*6) = 36
        Assert.Equal(27.0, px, 10);
        Assert.Equal(36.0, py, 10);
    }

    [Fact]
    public void Total3DSumsMomentumOfAllBodies()
    {
        (double X, double Y, double Z)[] velocities = [(1.0, 2.0, -1.0), (4.0, 5.0, 1.0)];
        double[] masses = [3.0, 6.0];

        var (px, py, pz) = Momentum.Total<double>(velocities, masses);

        Assert.Equal(27.0, px, 10);
        Assert.Equal(36.0, py, 10);
        Assert.Equal(3.0, pz, 10);
    }
}
