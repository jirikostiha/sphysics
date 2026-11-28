using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry2D;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary>
/// Bounces of a circular body in 2D, or a spherical one in 3D, off immovable obstacles of several shapes.
/// Every method pushes the body out to touching contact and reflects the part of its velocity along the
/// contact normal, scaled by the restitution; the part along the surface is kept.
/// Each returns null when the body does not touch the obstacle.
/// </summary>
/// <remarks>
/// Restitution is the ratio of the normal speed after the bounce to the one before it: 1 is elastic,
/// 0 stops the body on the surface, above 1 the surface adds energy (a bumper). A body already moving
/// away from the surface is only pushed out. An approach slower than the resting speed does not bounce at all,
/// so a body resting on a floor under gravity stays on it instead of hopping a little in every step.
/// <a href="https://en.wikipedia.org/wiki/Coefficient_of_restitution">wikipedia</a>
/// </remarks>
public static class Bounce
{
    /// <summary> Bounces a body off the outside of a circle (an immovable disc); a point when its radius is zero. </summary>
    /// <remarks> Null also when the centres coincide, as the direction of the push is then undefined. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="center"> Centre of the obstacle. </param>
    /// <param name="obstacleRadius"> Radius of the obstacle, zero or more. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    public static ((N X, N Y) Position, (N X, N Y) Velocity)? OffBall<N>(
        (N X, N Y) position, (N X, N Y) velocity, N radius,
        (N X, N Y) center, N obstacleRadius, N restitution, N restingSpeed)
        where N : IRootFunctions<N>, IComparisonOperators<N, N, bool>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(obstacleRadius);
        ThrowIfInvalidSurface(restitution, restingSpeed);

        var offset = GeometricVector2.Direction.FromCartesian(center, position);
        N distance = GeometricVector2.Magnitude.FromCartesian(offset);
        N reach = radius + obstacleRadius;
        if (distance >= reach || distance <= N.Zero)
            return null;

        return Respond(position, velocity, (offset.X / distance, offset.Y / distance), reach - distance, restitution, restingSpeed);
    }

    /// <summary> Bounces a body off the outside of a sphere (an immovable ball); a point when its radius is zero. </summary>
    /// <remarks> Null also when the centres coincide, as the direction of the push is then undefined. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="center"> Centre of the obstacle. </param>
    /// <param name="obstacleRadius"> Radius of the obstacle, zero or more. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    public static ((N X, N Y, N Z) Position, (N X, N Y, N Z) Velocity)? OffBall<N>(
        (N X, N Y, N Z) position, (N X, N Y, N Z) velocity, N radius,
        (N X, N Y, N Z) center, N obstacleRadius, N restitution, N restingSpeed)
        where N : IRootFunctions<N>, IComparisonOperators<N, N, bool>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(obstacleRadius);
        ThrowIfInvalidSurface(restitution, restingSpeed);

        var offset = GeometricVector3.Direction.FromCartesian(center, position);
        N distance = GeometricVector3.Magnitude.FromCartesian(offset);
        N reach = radius + obstacleRadius;
        if (distance >= reach || distance <= N.Zero)
            return null;

        return Respond(position, velocity, (offset.X / distance, offset.Y / distance, offset.Z / distance), reach - distance, restitution, restingSpeed);
    }

    /// <summary> Bounces a body off a line segment, a wall of no thickness at any angle. </summary>
    /// <remarks> Null also when the body's centre lies on the segment, as the side to push it to is then undefined. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="start"> One end of the segment. </param>
    /// <param name="end"> The other end of the segment; the same as <paramref name="start"/> makes it a point. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    public static ((N X, N Y) Position, (N X, N Y) Velocity)? OffSegment<N>(
        (N X, N Y) position, (N X, N Y) velocity, N radius,
        (N X, N Y) start, (N X, N Y) end, N restitution, N restingSpeed)
        where N : INumber<N>, IRootFunctions<N>
    {
        // the segment touches the body at its nearest point, which acts as a point obstacle
        return OffBall(position, velocity, radius, Line.Segment.And.Point.ClosestPoint.FromPoints(start, end, position), N.Zero, restitution, restingSpeed);
    }

    /// <summary> Bounces a body off a line segment in space, a rod of no thickness. </summary>
    /// <remarks> Null also when the body's centre lies on the segment, as the side to push it to is then undefined. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="start"> One end of the segment. </param>
    /// <param name="end"> The other end of the segment; the same as <paramref name="start"/> makes it a point. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    public static ((N X, N Y, N Z) Position, (N X, N Y, N Z) Velocity)? OffSegment<N>(
        (N X, N Y, N Z) position, (N X, N Y, N Z) velocity, N radius,
        (N X, N Y, N Z) start, (N X, N Y, N Z) end, N restitution, N restingSpeed)
        where N : INumber<N>, IRootFunctions<N>
    {
        // the segment touches the body at its nearest point, which acts as a point obstacle
        return OffBall(position, velocity, radius, Line3.Segment.And.Point.ClosestPoint.FromPoints(start, end, position), N.Zero, restitution, restingSpeed);
    }

    /// <summary> Bounces a body off a half-plane: the line through <paramref name="point"/> is its edge and everything behind it is solid. </summary>
    /// <remarks> A body that got behind the edge is pushed back out in front of it. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="point"> Any point on the edge. </param>
    /// <param name="normal"> Direction from the edge into the free side, of any non-zero length. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    /// <exception cref="ArgumentException"> The normal has zero length. </exception>
    public static ((N X, N Y) Position, (N X, N Y) Velocity)? OffPlane<N>(
        (N X, N Y) position, (N X, N Y) velocity, N radius,
        (N X, N Y) point, (N X, N Y) normal, N restitution, N restingSpeed)
        where N : IRootFunctions<N>, IComparisonOperators<N, N, bool>
    {
        ThrowIfInvalidSurface(restitution, restingSpeed);
        N length = NormalLength(GeometricVector2.Magnitude.FromCartesian(normal), nameof(normal));

        (N X, N Y) unit = (normal.X / length, normal.Y / length);
        N height = GeometricVector2.DotProduct.FromCartesian(GeometricVector2.Direction.FromCartesian(point, position), unit);
        if (height >= radius)
            return null;

        return Respond(position, velocity, unit, radius - height, restitution, restingSpeed);
    }

    /// <summary> Bounces a body off a half-space: the plane through <paramref name="point"/> is its face and everything behind it is solid. </summary>
    /// <remarks> A body that got behind the face is pushed back out in front of it. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="point"> Any point on the face. </param>
    /// <param name="normal"> Direction from the face into the free side, of any non-zero length. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    /// <exception cref="ArgumentException"> The normal has zero length. </exception>
    public static ((N X, N Y, N Z) Position, (N X, N Y, N Z) Velocity)? OffPlane<N>(
        (N X, N Y, N Z) position, (N X, N Y, N Z) velocity, N radius,
        (N X, N Y, N Z) point, (N X, N Y, N Z) normal, N restitution, N restingSpeed)
        where N : IRootFunctions<N>, IComparisonOperators<N, N, bool>
    {
        ThrowIfInvalidSurface(restitution, restingSpeed);
        N length = NormalLength(GeometricVector3.Magnitude.FromCartesian(normal), nameof(normal));

        (N X, N Y, N Z) unit = (normal.X / length, normal.Y / length, normal.Z / length);
        N height = GeometricVector3.DotProduct.FromCartesian(GeometricVector3.Direction.FromCartesian(point, position), unit);
        if (height >= radius)
            return null;

        return Respond(position, velocity, unit, radius - height, restitution, restingSpeed);
    }

    /// <summary> Bounces a body off the outside of an axis-aligned rectangle spanned by two opposite corners, given in any order. </summary>
    /// <remarks> A body whose centre got inside leaves through the nearest edge. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="corner1"> One corner of the rectangle. </param>
    /// <param name="corner2"> The opposite corner. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    public static ((N X, N Y) Position, (N X, N Y) Velocity)? OffBox<N>(
        (N X, N Y) position, (N X, N Y) velocity, N radius,
        (N X, N Y) corner1, (N X, N Y) corner2, N restitution, N restingSpeed)
        where N : INumber<N>, IRootFunctions<N>
    {
        var nearest = Rectangle.ClosestPointByCorners(corner1, corner2, position);
        if (nearest != position)
        {
            // outside, the nearest point of the rectangle acts as a point obstacle; that covers the corners too
            return OffBall(position, velocity, radius, nearest, N.Zero, restitution, restingSpeed);
        }

        ThrowIfInvalidSurface(restitution, restingSpeed);
        // the centre leaves through the nearest face; of equally near faces the one on the earlier axis
        var x = NearestFace(position.X, corner1.X, corner2.X);
        var y = NearestFace(position.Y, corner1.Y, corner2.Y);
        return y.Depth < x.Depth
            ? Respond(position, velocity, (N.Zero, y.Sign), y.Depth + radius, restitution, restingSpeed)
            : Respond(position, velocity, (x.Sign, N.Zero), x.Depth + radius, restitution, restingSpeed);
    }

    /// <summary> Bounces a body off the outside of an axis-aligned box spanned by two opposite corners, given in any order. </summary>
    /// <remarks> A body whose centre got inside leaves through the nearest face. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="corner1"> One corner of the box. </param>
    /// <param name="corner2"> The opposite corner. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    public static ((N X, N Y, N Z) Position, (N X, N Y, N Z) Velocity)? OffBox<N>(
        (N X, N Y, N Z) position, (N X, N Y, N Z) velocity, N radius,
        (N X, N Y, N Z) corner1, (N X, N Y, N Z) corner2, N restitution, N restingSpeed)
        where N : INumber<N>, IRootFunctions<N>
    {
        var nearest = Cuboid.ClosestPointByCorners(corner1, corner2, position);
        if (nearest != position)
        {
            // outside, the nearest point of the box acts as a point obstacle; that covers the edges and corners too
            return OffBall(position, velocity, radius, nearest, N.Zero, restitution, restingSpeed);
        }

        ThrowIfInvalidSurface(restitution, restingSpeed);
        // the centre leaves through the nearest face; of equally near faces the one on the earlier axis
        var x = NearestFace(position.X, corner1.X, corner2.X);
        var y = NearestFace(position.Y, corner1.Y, corner2.Y);
        var z = NearestFace(position.Z, corner1.Z, corner2.Z);
        if (z.Depth < x.Depth && z.Depth < y.Depth)
            return Respond(position, velocity, (N.Zero, N.Zero, z.Sign), z.Depth + radius, restitution, restingSpeed);

        return y.Depth < x.Depth
            ? Respond(position, velocity, (N.Zero, y.Sign, N.Zero), y.Depth + radius, restitution, restingSpeed)
            : Respond(position, velocity, (x.Sign, N.Zero, N.Zero), x.Depth + radius, restitution, restingSpeed);
    }

    /// <summary> Bounces a body off the outside of a triangle, given by its vertices in any order. </summary>
    /// <remarks> A body whose centre got inside leaves through the nearest edge. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="a"> One vertex of the triangle. </param>
    /// <param name="b"> The second vertex. </param>
    /// <param name="c"> The third vertex. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    /// <exception cref="ArgumentException"> The vertices lie on one line. </exception>
    public static ((N X, N Y) Position, (N X, N Y) Velocity)? OffTriangle<N>(
        (N X, N Y) position, (N X, N Y) velocity, N radius,
        (N X, N Y) a, (N X, N Y) b, (N X, N Y) c, N restitution, N restingSpeed)
        where N : INumber<N>, IRootFunctions<N>
    {
        ThrowIfInvalidSurface(restitution, restingSpeed);
        N area = Triangle.Region.SignedArea.FromVertices(a, b, c);
        if (!(N.Abs(area) > N.Zero))
            throw new ArgumentException("The vertices must not lie on one line.", nameof(c));

        // seen from inside, the centre is on the same side of every edge; the orientation of the triangle tells which
        N orientation = area > N.Zero ? N.One : -N.One;
        N sideAB = Triangle.Region.SignedArea.FromVertices(a, b, position) * orientation;
        N sideBC = Triangle.Region.SignedArea.FromVertices(b, c, position) * orientation;
        N sideCA = Triangle.Region.SignedArea.FromVertices(c, a, position) * orientation;
        if (sideAB < N.Zero || sideBC < N.Zero || sideCA < N.Zero)
        {
            // outside, the nearest point of the edges acts as a point obstacle; that covers the vertices too
            var nearest = Line.Segment.And.Point.ClosestPoint.FromPoints(a, b, position);
            var other = Line.Segment.And.Point.ClosestPoint.FromPoints(b, c, position);
            if (GeometricVector2.Distance.FromCartesian(position, other) < GeometricVector2.Distance.FromCartesian(position, nearest))
                nearest = other;

            other = Line.Segment.And.Point.ClosestPoint.FromPoints(c, a, position);
            if (GeometricVector2.Distance.FromCartesian(position, other) < GeometricVector2.Distance.FromCartesian(position, nearest))
                nearest = other;

            return OffBall(position, velocity, radius, nearest, N.Zero, restitution, restingSpeed);
        }

        // the centre leaves through the nearest edge, its distance being twice the area over the edge; of equally near edges the earlier one
        var edge = (a, b);
        N depth = sideAB / GeometricVector2.Distance.FromCartesian(a, b);
        N next = sideBC / GeometricVector2.Distance.FromCartesian(b, c);
        if (next < depth)
        {
            depth = next;
            edge = (b, c);
        }

        next = sideCA / GeometricVector2.Distance.FromCartesian(c, a);
        if (next < depth)
        {
            depth = next;
            edge = (c, a);
        }

        depth += depth;
        N length = GeometricVector2.Distance.FromCartesian(edge.Item1, edge.Item2);
        var outward = ((edge.Item2.Y - edge.Item1.Y) * orientation / length, (edge.Item1.X - edge.Item2.X) * orientation / length);
        return Respond(position, velocity, outward, depth + radius, restitution, restingSpeed);
    }

    /// <summary> Bounces a body off the outside of a flat triangle in space, given by its vertices in any order. </summary>
    /// <remarks> A body whose centre lies on the triangle leaves on the side it came from. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="a"> One vertex of the triangle. </param>
    /// <param name="b"> The second vertex. </param>
    /// <param name="c"> The third vertex. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    /// <exception cref="ArgumentException"> The vertices lie on one line. </exception>
    public static ((N X, N Y, N Z) Position, (N X, N Y, N Z) Velocity)? OffTriangle<N>(
        (N X, N Y, N Z) position, (N X, N Y, N Z) velocity, N radius,
        (N X, N Y, N Z) a, (N X, N Y, N Z) b, (N X, N Y, N Z) c, N restitution, N restingSpeed)
        where N : INumber<N>, IRootFunctions<N>
    {
        ThrowIfInvalidSurface(restitution, restingSpeed);
        var normal = GeometricVector3.CrossProduct.FromCartesian(GeometricVector3.Direction.FromCartesian(a, b), GeometricVector3.Direction.FromCartesian(a, c));
        if (!(GeometricVector3.MagnitudeSquared.FromCartesian(normal) > N.Zero))
            throw new ArgumentException("The vertices must not lie on one line.", nameof(c));

        // the centre is over the triangle when it is on the inner side of all three edges
        var offset = GeometricVector3.Direction.FromCartesian(a, position);
        bool over = GeometricVector3.TripleProduct.FromCartesian(normal, GeometricVector3.Direction.FromCartesian(a, b), offset) >= N.Zero
            && GeometricVector3.TripleProduct.FromCartesian(normal, GeometricVector3.Direction.FromCartesian(b, c), GeometricVector3.Direction.FromCartesian(b, position)) >= N.Zero
            && GeometricVector3.TripleProduct.FromCartesian(normal, GeometricVector3.Direction.FromCartesian(c, a), GeometricVector3.Direction.FromCartesian(c, position)) >= N.Zero;
        if (!over)
        {
            // beside it, the nearest point of the edges acts as a point obstacle; that covers the vertices too
            var nearest = Line3.Segment.And.Point.ClosestPoint.FromPoints(a, b, position);
            var other = Line3.Segment.And.Point.ClosestPoint.FromPoints(b, c, position);
            if (GeometricVector3.Distance.FromCartesian(position, other) < GeometricVector3.Distance.FromCartesian(position, nearest))
                nearest = other;

            other = Line3.Segment.And.Point.ClosestPoint.FromPoints(c, a, position);
            if (GeometricVector3.Distance.FromCartesian(position, other) < GeometricVector3.Distance.FromCartesian(position, nearest))
                nearest = other;

            return OffBall(position, velocity, radius, nearest, N.Zero, restitution, restingSpeed);
        }

        if (GeometricVector3.DotProduct.FromCartesian(offset, normal) != N.Zero)
        {
            // above or below, the foot of the perpendicular acts as a point obstacle
            var foot = GeometricVector3.Direction.FromCartesian(GeometricVector3.Projection.FromCartesian(offset, normal), position);
            return OffBall(position, velocity, radius, foot, N.Zero, restitution, restingSpeed);
        }

        // on the triangle itself, the body leaves on the side it came from
        var unit = GeometricVector3.Cartesian.Normalized(normal);
        if (GeometricVector3.DotProduct.FromCartesian(velocity, unit) > N.Zero)
            unit = (-unit.X, -unit.Y, -unit.Z);

        return Respond(position, velocity, unit, radius, restitution, restingSpeed);
    }

    /// <summary> Keeps a body inside an axis-aligned rectangle spanned by two opposite corners, given in any order. </summary>
    /// <remarks> Each axis bounces on its own, so a body hitting a corner bounces off both walls. A body wider than the rectangle is held at its middle. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="corner1"> One corner of the rectangle. </param>
    /// <param name="corner2"> The opposite corner. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    public static ((N X, N Y) Position, (N X, N Y) Velocity)? InsideBox<N>(
        (N X, N Y) position, (N X, N Y) velocity, N radius,
        (N X, N Y) corner1, (N X, N Y) corner2, N restitution, N restingSpeed)
        where N : INumber<N>, IRootFunctions<N>
    {
        ThrowIfInvalidSurface(restitution, restingSpeed);
        bool touched = false;
        var (px, vx) = Confine(position.X, velocity.X, radius, corner1.X, corner2.X, restitution, restingSpeed, ref touched);
        var (py, vy) = Confine(position.Y, velocity.Y, radius, corner1.Y, corner2.Y, restitution, restingSpeed, ref touched);

        return touched ? ((px, py), (vx, vy)) : null;
    }

    /// <summary> Keeps a body inside an axis-aligned box spanned by two opposite corners, given in any order. </summary>
    /// <remarks> Each axis bounces on its own, so a body hitting an edge or a corner bounces off every wall it meets. A body wider than the box is held at its middle. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="corner1"> One corner of the box. </param>
    /// <param name="corner2"> The opposite corner. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    public static ((N X, N Y, N Z) Position, (N X, N Y, N Z) Velocity)? InsideBox<N>(
        (N X, N Y, N Z) position, (N X, N Y, N Z) velocity, N radius,
        (N X, N Y, N Z) corner1, (N X, N Y, N Z) corner2, N restitution, N restingSpeed)
        where N : INumber<N>, IRootFunctions<N>
    {
        ThrowIfInvalidSurface(restitution, restingSpeed);
        bool touched = false;
        var (px, vx) = Confine(position.X, velocity.X, radius, corner1.X, corner2.X, restitution, restingSpeed, ref touched);
        var (py, vy) = Confine(position.Y, velocity.Y, radius, corner1.Y, corner2.Y, restitution, restingSpeed, ref touched);
        var (pz, vz) = Confine(position.Z, velocity.Z, radius, corner1.Z, corner2.Z, restitution, restingSpeed, ref touched);

        return touched ? ((px, py, pz), (vx, vy, vz)) : null;
    }

    /// <summary> Keeps a body inside a circular container. </summary>
    /// <remarks> A body wider than the container is held at its centre. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="center"> Centre of the container. </param>
    /// <param name="containerRadius"> Inner radius of the container, zero or more. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    public static ((N X, N Y) Position, (N X, N Y) Velocity)? InsideBall<N>(
        (N X, N Y) position, (N X, N Y) velocity, N radius,
        (N X, N Y) center, N containerRadius, N restitution, N restingSpeed)
        where N : INumber<N>, IRootFunctions<N>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(containerRadius);
        ThrowIfInvalidSurface(restitution, restingSpeed);

        var offset = GeometricVector2.Direction.FromCartesian(center, position);
        N distance = GeometricVector2.Magnitude.FromCartesian(offset);
        N limit = N.Max(containerRadius - radius, N.Zero);
        if (distance <= limit)
            return null;

        // the wall faces the centre, so its normal points inwards
        return Respond(position, velocity, (-offset.X / distance, -offset.Y / distance), distance - limit, restitution, restingSpeed);
    }

    /// <summary> Keeps a body inside a spherical container. </summary>
    /// <remarks> A body wider than the container is held at its centre. </remarks>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="position"> Body position. </param>
    /// <param name="velocity"> Body velocity. </param>
    /// <param name="radius"> Body radius. </param>
    /// <param name="center"> Centre of the container. </param>
    /// <param name="containerRadius"> Inner radius of the container, zero or more. </param>
    /// <param name="restitution"> Coefficient of restitution, zero or more. </param>
    /// <param name="restingSpeed"> Approach speed, zero or more, below which the body does not bounce but stays on the surface. </param>
    public static ((N X, N Y, N Z) Position, (N X, N Y, N Z) Velocity)? InsideBall<N>(
        (N X, N Y, N Z) position, (N X, N Y, N Z) velocity, N radius,
        (N X, N Y, N Z) center, N containerRadius, N restitution, N restingSpeed)
        where N : INumber<N>, IRootFunctions<N>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(containerRadius);
        ThrowIfInvalidSurface(restitution, restingSpeed);

        var offset = GeometricVector3.Direction.FromCartesian(center, position);
        N distance = GeometricVector3.Magnitude.FromCartesian(offset);
        N limit = N.Max(containerRadius - radius, N.Zero);
        if (distance <= limit)
            return null;

        // the wall faces the centre, so its normal points inwards
        return Respond(position, velocity, (-offset.X / distance, -offset.Y / distance, -offset.Z / distance), distance - limit, restitution, restingSpeed);
    }

    // Written as !(x >= 0), so that NaN is rejected as well.
    private static void ThrowIfInvalidSurface<N>(N restitution, N restingSpeed)
        where N : INumberBase<N>, IComparisonOperators<N, N, bool>
    {
        if (!(restitution >= N.Zero))
            throw new ArgumentOutOfRangeException(nameof(restitution), restitution, "The restitution must be zero or more.");

        if (!(restingSpeed >= N.Zero))
            throw new ArgumentOutOfRangeException(nameof(restingSpeed), restingSpeed, "The resting speed must be zero or more.");
    }

    // Pushes the body along the unit normal by the depth and reflects its normal velocity when it approaches the surface.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ((N X, N Y) Position, (N X, N Y) Velocity) Respond<N>(
        (N X, N Y) position, (N X, N Y) velocity, (N X, N Y) normal, N depth, N restitution, N restingSpeed)
        where N : INumberBase<N>, IComparisonOperators<N, N, bool>
    {
        var pushed = (position.X + normal.X * depth, position.Y + normal.Y * depth);
        N approach = GeometricVector2.DotProduct.FromCartesian(velocity, normal);
        if (approach >= N.Zero)
            return (pushed, velocity);

        // v' = v - (1 + e) * (v . n) * n, with e = 0 for a slow approach so that a body resting on the surface stays there
        N change = Restitution.NormalVelocityChange(EffectiveRestitution(approach, restitution, restingSpeed), approach);
        return (pushed, (velocity.X - change * normal.X, velocity.Y - change * normal.Y));
    }

    // Pushes the body along the unit normal by the depth and reflects its normal velocity when it approaches the surface.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ((N X, N Y, N Z) Position, (N X, N Y, N Z) Velocity) Respond<N>(
        (N X, N Y, N Z) position, (N X, N Y, N Z) velocity, (N X, N Y, N Z) normal, N depth, N restitution, N restingSpeed)
        where N : INumberBase<N>, IComparisonOperators<N, N, bool>
    {
        var pushed = (position.X + normal.X * depth, position.Y + normal.Y * depth, position.Z + normal.Z * depth);
        N approach = GeometricVector3.DotProduct.FromCartesian(velocity, normal);
        if (approach >= N.Zero)
            return (pushed, velocity);

        // v' = v - (1 + e) * (v . n) * n, with e = 0 for a slow approach so that a body resting on the surface stays there
        N change = Restitution.NormalVelocityChange(EffectiveRestitution(approach, restitution, restingSpeed), approach);
        return (pushed, (velocity.X - change * normal.X, velocity.Y - change * normal.Y, velocity.Z - change * normal.Z));
    }

    // The restitution for an approach at the (negative) normal velocity: none at all when the approach is slower than the resting speed.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static N EffectiveRestitution<N>(N approach, N restitution, N restingSpeed)
        where N : INumberBase<N>, IComparisonOperators<N, N, bool>
        => -approach < restingSpeed ? N.Zero : restitution;

    // One axis of a body held between two walls: the position clamped between them and the velocity reflected off the wall it hit.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (N Position, N Velocity) Confine<N>(N position, N velocity, N radius, N wall1, N wall2, N restitution, N restingSpeed, ref bool touched)
        where N : INumber<N>
    {
        N min = N.Min(wall1, wall2);
        N max = N.Max(wall1, wall2);
        N low = min + radius;
        N high = max - radius;
        if (low > high)
        {
            low = high = (min + max) / (N.One + N.One);
        }

        if (position < low)
        {
            touched = true;
            return (low, velocity < N.Zero ? -EffectiveRestitution(velocity, restitution, restingSpeed) * velocity : velocity);
        }

        if (position > high)
        {
            touched = true;
            return (high, velocity > N.Zero ? -EffectiveRestitution(-velocity, restitution, restingSpeed) * velocity : velocity);
        }

        return (position, velocity);
    }

    // The nearer of the two faces of one axis, given in any order, to a centre between them: its distance and the sign of its normal; the lower face when equally near.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (N Depth, N Sign) NearestFace<N>(N position, N face1, N face2)
        where N : INumber<N>
    {
        N min = N.Min(face1, face2);
        N max = N.Max(face1, face2);
        return position - min <= max - position ? (position - min, -N.One) : (max - position, N.One);
    }

    private static N NormalLength<N>(N length, string paramName)
        where N : INumberBase<N>, IComparisonOperators<N, N, bool>
        => length > N.Zero ? length : throw new ArgumentException("The normal must have a non-zero length.", paramName);
}
