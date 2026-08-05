using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Elastic collision math for circular bodies in 2D.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Elastic_collision">wikipedia</a>
/// </remarks>
public static class ElasticCollision
{
    /// <summary>
    /// Resolves an elastic collision between two circular bodies.
    /// A body with zero mass is treated as having infinite mass (does not move).
    /// Returns null when bodies do not overlap or overlap at the same point.
    /// </summary>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="p1"> Position of body 1. </param>
    /// <param name="v1"> Velocity of body 1. </param>
    /// <param name="m1"> Mass of body 1. Zero means immovable. </param>
    /// <param name="r1"> Radius of body 1. </param>
    /// <param name="p2"> Position of body 2. </param>
    /// <param name="v2"> Velocity of body 2. </param>
    /// <param name="m2"> Mass of body 2. Zero means immovable. </param>
    /// <param name="r2"> Radius of body 2. </param>
    public static ((N X, N Y) P1, (N X, N Y) V1, (N X, N Y) P2, (N X, N Y) V2)? Resolve<N>(
        (N X, N Y) p1, (N X, N Y) v1, N m1, N r1,
        (N X, N Y) p2, (N X, N Y) v2, N m2, N r2)
        where N : IRootFunctions<N>, IComparisonOperators<N, N, bool>
    {
        N dx = p2.X - p1.X;
        N dy = p2.Y - p1.Y;
        N distance = N.Sqrt(dx * dx + dy * dy);
        N minDistance = r1 + r2;

        if (distance >= minDistance || distance <= N.Zero)
            return null;

        N nx = dx / distance;
        N ny = dy / distance;

        // Mass handling: 0 mass is treated as infinite/static (invMass = 0).
        N invMass1 = m1 > N.Zero ? N.One / m1 : N.Zero;
        N invMass2 = m2 > N.Zero ? N.One / m2 : N.Zero;
        N totalInverseMass = invMass1 + invMass2;

        if (totalInverseMass <= N.Zero)
            return null;

        // 1. Position correction (separation) distributed by inverse mass.
        N overlap = minDistance - distance;
        N corr1 = overlap * (invMass1 / totalInverseMass);
        N corr2 = overlap * (invMass2 / totalInverseMass);

        var p1Out = (p1.X - nx * corr1, p1.Y - ny * corr1);
        var p2Out = (p2.X + nx * corr2, p2.Y + ny * corr2);

        // 2. Velocity update (impulse-based elastic collision).
        N rvx = v2.X - v1.X;
        N rvy = v2.Y - v1.Y;
        N velocityAlongNormal = rvx * nx + rvy * ny;

        var v1Out = v1;
        var v2Out = v2;

        // Apply impulse only when bodies are approaching; when already separating keep velocities.
        if (velocityAlongNormal <= N.Zero)
        {
            N two = N.One + N.One;
            N impulseMagnitude = -(two * velocityAlongNormal) / totalInverseMass;
            N jx = impulseMagnitude * nx;
            N jy = impulseMagnitude * ny;

            v1Out = (v1.X - jx * invMass1, v1.Y - jy * invMass1);
            v2Out = (v2.X + jx * invMass2, v2.Y + jy * invMass2);
        }

        return (p1Out, v1Out, p2Out, v2Out);
    }

    /// <summary>
    /// Resolves an elastic collision between a circular body and an immovable point obstacle.
    /// Returns null when the body does not overlap the obstacle or coincides with it.
    /// </summary>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="obstaclePosition"> Obstacle position (treated as a point). </param>
    public static ((N X, N Y) Position, (N X, N Y) Velocity)? ResolveWithObstacle<N>(
        (N X, N Y) position, (N X, N Y) velocity, N radius,
        (N X, N Y) obstaclePosition)
        where N : IRootFunctions<N>, IComparisonOperators<N, N, bool>
    {
        N dx = obstaclePosition.X - position.X;
        N dy = obstaclePosition.Y - position.Y;
        N distance = N.Sqrt(dx * dx + dy * dy);

        if (distance >= radius || distance <= N.Zero)
            return null;

        N nx = dx / distance;
        N ny = dy / distance;

        // Position correction (separation): only the body moves, obstacle is infinite mass.
        N overlap = radius - distance;
        var posOut = (position.X - nx * overlap, position.Y - ny * overlap);

        // If already separating from the obstacle, keep velocity.
        // (-v) . n > 0  <=>  v . n < 0  <=>  moving toward obstacle.
        N approach = -(velocity.X * nx + velocity.Y * ny);
        if (approach > N.Zero)
            return (posOut, velocity);

        // Reflection: v' = v - 2 * (v . n) * n
        N two = N.One + N.One;
        N vDotN = velocity.X * nx + velocity.Y * ny;
        N vx = velocity.X - two * vDotN * nx;
        N vy = velocity.Y - two * vDotN * ny;

        return (posOut, (vx, vy));
    }

    /// <summary>
    /// Post-collision velocities for a 1D head-on elastic collision (no position correction).
    /// A body with zero mass is treated as infinite mass and its velocity is unchanged.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N V1, N V2) HeadOn1D<N>(N m1, N v1, N m2, N v2)
        where N : INumberBase<N>, IComparisonOperators<N, N, bool>
    {
        N invMass1 = m1 > N.Zero ? N.One / m1 : N.Zero;
        N invMass2 = m2 > N.Zero ? N.One / m2 : N.Zero;
        N total = invMass1 + invMass2;
        if (total <= N.Zero)
            return (v1, v2);

        N two = N.One + N.One;
        N impulse = -(two * (v2 - v1)) / total;

        return (v1 - impulse * invMass1, v2 + impulse * invMass2);
    }
}
