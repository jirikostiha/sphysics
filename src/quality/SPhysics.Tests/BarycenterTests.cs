using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class BarycenterTests
{
    [Fact]
    public void EvalReturnsDistanceFromFirstBodyCenter()
    {
        // r1 = d * m2 / (m1 + m2) = 3 * 1 / 3 = 1
        Assert.Equal(1.0, Barycenter.Eval(2.0, 1.0, 3.0), 10);
    }

    [Fact]
    public void EvalIsHalfwayForEqualMasses()
    {
        Assert.Equal(5.0, Barycenter.Eval(1.0, 1.0, 10.0), 10);
    }
}
