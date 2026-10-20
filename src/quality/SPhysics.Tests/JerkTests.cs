using Xunit;

namespace SPhysics.Tests;

public class JerkTests
{
    [Fact]
    public void JerkFormulas()
    {
        Assert.Equal(5.0, Jerk.Eval(accelerationChange: 10.0, time: 2.0), 10);
        Assert.Equal(1.25, Jerk.FromForceChange(forceChange: 10.0, time: 2.0, mass: 4.0), 10);
    }
}
