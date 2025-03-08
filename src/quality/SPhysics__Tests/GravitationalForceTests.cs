using SPhysics;
using Xunit;

namespace SMath.Geometry2D;

public class GravitationalForceTests
{
    [Fact]
    public void Eval_AvoidsZeroDistance()
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
}

