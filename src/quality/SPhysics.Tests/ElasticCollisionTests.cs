using Xunit;

namespace SPhysics.Tests;

public class ElasticCollisionTests
{
    [Fact]
    public void ResolveReturnsNullWhenBodiesDoNotOverlap()
    {
        Assert.Null(ElasticCollision.Resolve((0.0, 0.0), (1.0, 0.0), 1.0, 1.0,
                                             (5.0, 0.0), (0.0, 0.0), 1.0, 1.0));
    }

    [Fact]
    public void ResolveReturnsNullWhenBothBodiesAreImmovable()
    {
        Assert.Null(ElasticCollision.Resolve((0.0, 0.0), (0.0, 0.0), 0.0, 1.0,
                                             (1.0, 0.0), (-1.0, 0.0), 0.0, 1.0));
    }

    [Fact]
    public void ResolveSwapsVelocitiesOfEqualMassesHeadOn()
    {
        var result = ElasticCollision.Resolve((0.0, 0.0), (1.0, 0.0), 1.0, 1.0,
                                              (1.0, 0.0), (-1.0, 0.0), 1.0, 1.0);

        Assert.NotNull(result);
        var (p1, v1, p2, v2) = result.Value;

        Assert.Equal(-1.0, v1.X, 10);
        Assert.Equal(1.0, v2.X, 10);
        // the overlap is shared equally, leaving the bodies exactly touching
        Assert.Equal(-0.5, p1.X, 10);
        Assert.Equal(1.5, p2.X, 10);
    }

    [Fact]
    public void ResolveConservesMomentumAndKineticEnergy()
    {
        const double m1 = 1.0;
        const double m2 = 3.0;

        var result = ElasticCollision.Resolve((0.0, 0.0), (2.0, 0.0), m1, 1.0,
                                              (1.0, 0.0), (0.0, 0.0), m2, 1.0);

        Assert.NotNull(result);
        var (_, v1, _, v2) = result.Value;

        Assert.Equal(m1 * 2.0 + m2 * 0.0, m1 * v1.X + m2 * v2.X, 10);
        Assert.Equal(KineticEnergy.Linear(m1, 2.0),
                     KineticEnergy.Linear(m1, v1.X) + KineticEnergy.Linear(m2, v2.X), 10);
    }

    [Fact]
    public void ResolveTreatsZeroMassAsImmovable()
    {
        var result = ElasticCollision.Resolve((0.0, 0.0), (0.0, 0.0), 0.0, 1.0,
                                              (1.0, 0.0), (-1.0, 0.0), 2.0, 1.0);

        Assert.NotNull(result);
        var (p1, v1, p2, v2) = result.Value;

        Assert.Equal(0.0, p1.X, 10);
        Assert.Equal(0.0, v1.X, 10);
        Assert.Equal(2.0, p2.X, 10);
        Assert.Equal(1.0, v2.X, 10);
    }

    [Fact]
    public void ResolveWithObstacleReturnsNullWhenNotOverlapping()
    {
        Assert.Null(ElasticCollision.ResolveWithObstacle((0.0, 0.0), (1.0, 0.0), 1.0, (5.0, 0.0)));
        Assert.Null(ElasticCollision.ResolveWithObstacle((0.0, 0.0), (1.0, 0.0), 1.0, (0.0, 0.0)));
    }

    [Fact]
    public void ResolveWithObstacleReflectsAnApproachingBody()
    {
        // obstacle to the right, body moving right: it is approaching
        var result = ElasticCollision.ResolveWithObstacle((0.0, 0.0), (2.0, 1.0), 2.0, (1.0, 0.0));

        Assert.NotNull(result);
        var (position, velocity) = result.Value;

        Assert.Equal(-2.0, velocity.X, 10);   // normal component reversed
        Assert.Equal(1.0, velocity.Y, 10);    // tangential component kept
        Assert.Equal(-1.0, position.X, 10);   // pushed out to exactly the radius
    }

    [Fact]
    public void ResolveWithObstacleKeepsTheVelocityOfASeparatingBody()
    {
        // obstacle to the right, body moving left: it is already moving away
        var result = ElasticCollision.ResolveWithObstacle((0.0, 0.0), (-2.0, 1.0), 2.0, (1.0, 0.0));

        Assert.NotNull(result);
        var (_, velocity) = result.Value;

        // reflecting here would turn the body back into the obstacle and give +2
        Assert.Equal(-2.0, velocity.X, 10);
        Assert.Equal(1.0, velocity.Y, 10);
    }

    [Fact]
    public void ResolveWithObstacleConservesSpeed()
    {
        var result = ElasticCollision.ResolveWithObstacle((0.0, 0.0), (3.0, 4.0), 2.0, (1.0, 0.0));

        Assert.NotNull(result);
        var (_, velocity) = result.Value;

        Assert.Equal(5.0, System.Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y), 10);
    }

    [Theory]
    [InlineData(1.0, 2.0, 1.0, -1.0, -1.0, 2.0)]   // equal masses exchange velocities
    [InlineData(1.0, 2.0, 3.0, 0.0, -1.0, 1.0)]    // ((m1-m2)v1 + 2*m2*v2) / (m1+m2)
    [InlineData(0.0, 0.0, 1.0, -1.0, 0.0, 1.0)]    // zero mass acts as a wall
    public void HeadOn1DMatchesTheClosedFormSolution(
        double m1, double v1, double m2, double v2, double expected1, double expected2)
    {
        var (r1, r2) = ElasticCollision.HeadOn1D(m1, v1, m2, v2);

        Assert.Equal(expected1, r1, 10);
        Assert.Equal(expected2, r2, 10);
    }

    [Fact]
    public void HeadOn1DConservesMomentumAndKineticEnergy()
    {
        const double m1 = 2.0;
        const double m2 = 5.0;
        const double v1 = 3.0;
        const double v2 = -1.0;

        var (r1, r2) = ElasticCollision.HeadOn1D(m1, v1, m2, v2);

        Assert.Equal(m1 * v1 + m2 * v2, m1 * r1 + m2 * r2, 10);
        Assert.Equal(KineticEnergy.Linear(m1, v1) + KineticEnergy.Linear(m2, v2),
                     KineticEnergy.Linear(m1, r1) + KineticEnergy.Linear(m2, r2), 10);
    }
}
