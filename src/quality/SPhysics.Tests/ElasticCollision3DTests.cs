using Xunit;

namespace SPhysics.Tests;

public class ElasticCollision3DTests
{
    [Fact]
    public void ResolveReturnsNullWhenBodiesDoNotOverlapOrAreImmovable()
    {
        Assert.Null(ElasticCollision.Resolve((0.0, 0.0, 0.0), (1.0, 0.0, 0.0), 1.0, 1.0,
                                             (0.0, 0.0, 5.0), (0.0, 0.0, 0.0), 1.0, 1.0));
        Assert.Null(ElasticCollision.Resolve((0.0, 0.0, 0.0), (0.0, 0.0, 0.0), 0.0, 1.0,
                                             (0.0, 0.0, 1.0), (0.0, 0.0, -1.0), 0.0, 1.0));
        Assert.Null(ElasticCollision.Resolve((1.0, 2.0, 3.0), (0.0, 0.0, 0.0), 1.0, 1.0,
                                             (1.0, 2.0, 3.0), (0.0, 0.0, 0.0), 1.0, 1.0));
    }

    [Fact]
    public void ResolveSwapsVelocitiesOfEqualMassesHeadOnAlongZ()
    {
        var result = ElasticCollision.Resolve((0.0, 0.0, 0.0), (0.0, 0.0, 1.0), 1.0, 1.0,
                                              (0.0, 0.0, 1.0), (0.0, 0.0, -1.0), 1.0, 1.0);

        Assert.NotNull(result);
        var (p1, v1, p2, v2) = result.Value;
        Assert.Equal((0.0, 0.0, -1.0), v1);
        Assert.Equal((0.0, 0.0, 1.0), v2);
        Assert.Equal((0.0, 0.0, -0.5), p1);
        Assert.Equal((0.0, 0.0, 1.5), p2);
    }

    [Fact]
    public void ResolvePlanarBodiesMatchThe2DResult()
    {
        var planar = ElasticCollision.Resolve((0.0, 0.0), (1.0, 0.5), 2.0, 1.0,
                                              (1.2, 0.7), (-0.5, 0.25), 3.0, 0.5);
        var spatial = ElasticCollision.Resolve((0.0, 0.0, 0.0), (1.0, 0.5, 0.0), 2.0, 1.0,
                                               (1.2, 0.7, 0.0), (-0.5, 0.25, 0.0), 3.0, 0.5);

        Assert.NotNull(planar);
        Assert.NotNull(spatial);
        Assert.Equal(planar.Value.P1, (spatial.Value.P1.X, spatial.Value.P1.Y));
        Assert.Equal(planar.Value.V1, (spatial.Value.V1.X, spatial.Value.V1.Y));
        Assert.Equal(planar.Value.P2, (spatial.Value.P2.X, spatial.Value.P2.Y));
        Assert.Equal(planar.Value.V2, (spatial.Value.V2.X, spatial.Value.V2.Y));
    }

    [Fact]
    public void ResolveConservesMomentumAndKineticEnergy()
    {
        const double m1 = 1.5;
        const double m2 = 4.0;
        (double X, double Y, double Z) v1 = (2.0, -1.0, 0.5);
        (double X, double Y, double Z) v2 = (-0.5, 0.3, -1.0);

        var result = ElasticCollision.Resolve((0.0, 0.0, 0.0), v1, m1, 1.0, (0.6, 0.8, 0.9), v2, m2, 1.0);

        Assert.NotNull(result);
        var (_, u1, _, u2) = result.Value;
        Assert.Equal(m1 * v1.X + m2 * v2.X, m1 * u1.X + m2 * u2.X, 12);
        Assert.Equal(m1 * v1.Y + m2 * v2.Y, m1 * u1.Y + m2 * u2.Y, 12);
        Assert.Equal(m1 * v1.Z + m2 * v2.Z, m1 * u1.Z + m2 * u2.Z, 12);
        Assert.Equal(Energy(m1, v1) + Energy(m2, v2), Energy(m1, u1) + Energy(m2, u2), 12);
    }

    [Fact]
    public void ResolveKeepsVelocitiesOfSeparatingBodies()
    {
        var result = ElasticCollision.Resolve((0.0, 0.0, 0.0), (0.0, 0.0, -1.0), 1.0, 1.0,
                                              (0.0, 0.0, 1.0), (0.0, 0.0, 1.0), 1.0, 1.0);

        Assert.NotNull(result);
        Assert.Equal((0.0, 0.0, -1.0), result.Value.V1);
        Assert.Equal((0.0, 0.0, 1.0), result.Value.V2);
    }

    [Fact]
    public void ResolveWithObstacleReflectsAnApproachingBody()
    {
        var result = ElasticCollision.ResolveWithObstacle((0.0, 0.0, 0.0), (1.0, 2.0, 3.0), 1.0, (0.0, 0.0, 0.5));

        Assert.NotNull(result);
        Assert.Equal((0.0, 0.0, -0.5), result.Value.Position);
        Assert.Equal((1.0, 2.0, -3.0), result.Value.Velocity);
    }

    [Fact]
    public void ResolveWithObstacleKeepsTheVelocityOfALeavingBodyOrReturnsNull()
    {
        var leaving = ElasticCollision.ResolveWithObstacle((0.0, 0.0, 0.0), (0.0, 0.0, -1.0), 1.0, (0.0, 0.0, 0.5));

        Assert.NotNull(leaving);
        Assert.Equal((0.0, 0.0, -1.0), leaving.Value.Velocity);
        Assert.Null(ElasticCollision.ResolveWithObstacle((0.0, 0.0, 0.0), (0.0, 0.0, 1.0), 1.0, (0.0, 0.0, 2.0)));
        Assert.Null(ElasticCollision.ResolveWithObstacle((0.0, 0.0, 0.0), (0.0, 0.0, 1.0), 1.0, (0.0, 0.0, 0.0)));
    }

    private static double Energy(double mass, (double X, double Y, double Z) velocity)
        => KineticEnergy.Linear(mass, Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y + velocity.Z * velocity.Z));
}
