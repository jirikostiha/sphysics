using Xunit;

namespace SPhysics.Tests;

public class BounceTests
{
    private const int Precision = 12;

    [Fact]
    public void OffBallReflectsAnApproachingBodyAndPushesItToContact()
    {
        var result = Bounce.OffBall((1.5, 0.0), (-2.0, 1.0), 1.0, (0.0, 0.0), 1.0, 1.0, 0.0);

        Assert.NotNull(result);
        var (position, velocity) = result.Value;
        AssertClose((2.0, 0.0), position);
        AssertClose((2.0, 1.0), velocity);
    }

    [Fact]
    public void OffBallReturnsNullOutOfReach()
    {
        Assert.Null(Bounce.OffBall((2.0, 0.0), (-1.0, 0.0), 1.0, (0.0, 0.0), 1.0, 1.0, 0.0));
    }

    [Fact]
    public void OffBallReturnsNullAtTheCentre()
    {
        Assert.Null(Bounce.OffBall((0.0, 0.0), (-1.0, 0.0), 1.0, (0.0, 0.0), 1.0, 1.0, 0.0));
    }

    [Fact]
    public void OffBallWithZeroRadiusMatchesThePointObstacle()
    {
        var ball = Bounce.OffBall((0.0, 0.0), (1.0, 0.5), 1.0, (0.6, 0.2), 0.0, 1.0, 0.0);
        var point = ElasticCollision.ResolveWithObstacle((0.0, 0.0), (1.0, 0.5), 1.0, (0.6, 0.2));

        Assert.Equal(point, ball);
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.5, 1.5)]
    [InlineData(1.0, 3.0)]
    [InlineData(1.5, 4.5)]
    public void RestitutionScalesTheNormalSpeedAndKeepsTheTangentialOne(double restitution, double normalSpeedAfter)
    {
        var result = Bounce.OffPlane((0.0, 0.5), (2.0, -3.0), 1.0, (0.0, 0.0), (0.0, 1.0), restitution, 0.0);

        Assert.NotNull(result);
        AssertClose((2.0, normalSpeedAfter), result.Value.Velocity);
    }

    [Fact]
    public void ApproachSlowerThanTheRestingSpeedStopsOnTheSurfaceAndKeepsTheTangentialSpeed()
    {
        var result = Bounce.OffPlane((0.0, 0.5), (2.0, -0.05), 1.0, (0.0, 0.0), (0.0, 1.0), 0.8, 0.1);

        Assert.NotNull(result);
        AssertClose((0.0, 1.0), result.Value.Position);
        AssertClose((2.0, 0.0), result.Value.Velocity);
    }

    [Fact]
    public void ApproachAtTheRestingSpeedStillBounces()
    {
        var result = Bounce.OffPlane((0.0, 0.5), (0.0, -0.1), 1.0, (0.0, 0.0), (0.0, 1.0), 0.5, 0.1);

        Assert.NotNull(result);
        AssertClose((0.0, 0.05), result.Value.Velocity);
    }

    [Fact]
    public void InsideBoxStopsASlowApproachOnTheWall()
    {
        var result = Bounce.InsideBox((0.0, -4.5), (1.0, -0.05), 1.0, (-5.0, -5.0), (5.0, 5.0), 0.8, 0.1);

        Assert.NotNull(result);
        AssertClose((0.0, -4.0), result.Value.Position);
        AssertClose((1.0, 0.0), result.Value.Velocity);
    }

    /// <summary> Under gravity a body on a floor approaches it by g * dt in every step; above that resting speed it settles instead of hopping. </summary>
    [Fact]
    public void BodyDroppedOnAFloorComesToRestOnIt()
    {
        const double g = 9.81, dt = 0.001, radius = 0.5;
        (double X, double Y) position = (0.0, 2.0), velocity = (0.0, 0.0);
        for (int step = 0; step < 20_000; step++)
        {
            velocity = (velocity.X, velocity.Y - g * dt);
            position = (position.X, position.Y + velocity.Y * dt);
            (position, velocity) = Bounce.OffPlane(position, velocity, radius, (0.0, 0.0), (0.0, 1.0), 0.8, 2 * g * dt) ?? (position, velocity);
        }

        Assert.Equal(radius, position.Y, Precision);
        Assert.Equal(0.0, velocity.Y);
    }

    [Fact]
    public void BodyMovingAwayIsPushedOutWithItsVelocityKept()
    {
        var result = Bounce.OffPlane((0.0, 0.5), (2.0, 3.0), 1.0, (0.0, 0.0), (0.0, 1.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((0.0, 1.0), result.Value.Position);
        Assert.Equal((2.0, 3.0), result.Value.Velocity);
    }

    [Fact]
    public void OffPlaneAcceptsANormalOfAnyLengthAndPushesABodyFromBehindToTheFront()
    {
        // a wall along the diagonal, free side towards +x+y, the body sunk 1 behind it
        var result = Bounce.OffPlane((-0.5, -0.5), (1.0, 1.0), 1.0, (0.0, 0.0), (3.0, 3.0), 1.0, 0.0);

        Assert.NotNull(result);
        double front = Math.Sqrt(0.5);
        AssertClose((front, front), result.Value.Position);
        AssertClose((1.0, 1.0), result.Value.Velocity);
    }

    [Fact]
    public void OffPlaneReturnsNullInFrontOfTheReach()
    {
        Assert.Null(Bounce.OffPlane((0.0, 1.0), (0.0, -1.0), 1.0, (0.0, 0.0), (0.0, 1.0), 1.0, 0.0));
    }

    [Fact]
    public void OffPlaneRejectsANormalOfZeroLength()
    {
        Assert.Throws<ArgumentException>(() => Bounce.OffPlane((0.0, 0.0), (0.0, 0.0), 1.0, (0.0, 0.0), (0.0, 0.0), 1.0, 0.0));
    }

    [Fact]
    public void OffSegmentBouncesOffItsMiddleLikeAWall()
    {
        var result = Bounce.OffSegment((1.0, 0.5), (0.5, -2.0), 1.0, (0.0, 0.0), (4.0, 0.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((1.0, 1.0), result.Value.Position);
        AssertClose((0.5, 2.0), result.Value.Velocity);
    }

    [Fact]
    public void OffSegmentBouncesOffItsEndLikeAPoint()
    {
        var segment = Bounce.OffSegment((-0.6, 0.3), (1.0, 0.0), 1.0, (0.0, 0.0), (4.0, 0.0), 1.0, 0.0);
        var point = Bounce.OffBall((-0.6, 0.3), (1.0, 0.0), 1.0, (0.0, 0.0), 0.0, 1.0, 0.0);

        Assert.Equal(point, segment);
    }

    [Fact]
    public void OffSegmentOfNoLengthIsAPoint()
    {
        var segment = Bounce.OffSegment((0.5, 0.0), (-1.0, 0.0), 1.0, (0.0, 0.0), (0.0, 0.0), 1.0, 0.0);
        var point = Bounce.OffBall((0.5, 0.0), (-1.0, 0.0), 1.0, (0.0, 0.0), 0.0, 1.0, 0.0);

        Assert.Equal(point, segment);
    }

    [Fact]
    public void OffBoxBouncesOffAFaceWithTheCornersInAnyOrder()
    {
        var result = Bounce.OffBox((-0.5, 0.0), (3.0, 0.5), 1.0, (4.0, 1.0), (0.0, -1.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((-1.0, 0.0), result.Value.Position);
        AssertClose((-3.0, 0.5), result.Value.Velocity);
    }

    [Fact]
    public void OffBoxBouncesOffACornerLikeAPoint()
    {
        var box = Bounce.OffBox((-0.3, -1.3), (1.0, 1.0), 1.0, (0.0, -1.0), (4.0, 1.0), 1.0, 0.0);
        var point = Bounce.OffBall((-0.3, -1.3), (1.0, 1.0), 1.0, (0.0, -1.0), 0.0, 1.0, 0.0);

        Assert.Equal(point, box);
    }

    [Fact]
    public void OffBoxLetsACentreInsideLeaveThroughTheNearestFace()
    {
        var result = Bounce.OffBox((3.5, 0.2), (-2.0, 0.0), 1.0, (0.0, -1.0), (4.0, 1.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((5.0, 0.2), result.Value.Position);
        AssertClose((2.0, 0.0), result.Value.Velocity);
    }

    /// <summary> Equally near faces are taken in the order x before y and the lower face before the upper one. </summary>
    [Theory]
    [InlineData(1.0, 0.0, -1.0, 0.0)]
    [InlineData(2.0, 0.0, 2.0, -2.0)]
    public void OffBoxLetsACentreEquallyNearToTwoFacesLeaveThroughTheFirst(double x, double y, double exitX, double exitY)
    {
        var result = Bounce.OffBox((x, y), (0.0, 0.0), 1.0, (0.0, -1.0), (4.0, 1.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((exitX, exitY), result.Value.Position);
    }

    [Fact]
    public void OffBoxReturnsNullOutOfReach()
    {
        Assert.Null(Bounce.OffBox((-1.0, 0.0), (3.0, 0.0), 1.0, (0.0, -1.0), (4.0, 1.0), 1.0, 0.0));
    }

    [Fact]
    public void InsideBoxBouncesOffBothWallsOfACorner()
    {
        var result = Bounce.InsideBox((4.5, -4.8), (1.0, -2.0), 1.0, (-5.0, -5.0), (5.0, 5.0), 0.5, 0.0);

        Assert.NotNull(result);
        AssertClose((4.0, -4.0), result.Value.Position);
        AssertClose((-0.5, 1.0), result.Value.Velocity);
    }

    [Fact]
    public void InsideBoxReturnsNullClearOfTheWalls()
    {
        Assert.Null(Bounce.InsideBox((0.0, 0.0), (1.0, 1.0), 1.0, (-5.0, -5.0), (5.0, 5.0), 1.0, 0.0));
    }

    [Fact]
    public void InsideBoxHoldsABodyWiderThanTheBoxAtItsMiddle()
    {
        var result = Bounce.InsideBox((0.3, 0.0), (1.0, 0.0), 2.0, (-1.0, -5.0), (1.0, 5.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((0.0, 0.0), result.Value.Position);
        AssertClose((-1.0, 0.0), result.Value.Velocity);
    }

    [Fact]
    public void InsideBallReflectsABodyReachingTheWall()
    {
        var result = Bounce.InsideBall((4.5, 0.0), (2.0, 1.0), 1.0, (0.0, 0.0), 5.0, 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((4.0, 0.0), result.Value.Position);
        AssertClose((-2.0, 1.0), result.Value.Velocity);
    }

    [Fact]
    public void InsideBallReturnsNullClearOfTheWall()
    {
        Assert.Null(Bounce.InsideBall((3.0, 0.0), (2.0, 0.0), 1.0, (0.0, 0.0), 5.0, 1.0, 0.0));
    }

    [Fact]
    public void OffBoxInsideStopsASlowApproachOnTheNearestFace()
    {
        var result = Bounce.OffBox((3.5, 0.2), (-0.05, 1.0), 1.0, (0.0, -1.0), (4.0, 1.0), 0.8, 0.1);

        Assert.NotNull(result);
        AssertClose((5.0, 0.2), result.Value.Position);
        AssertClose((0.0, 1.0), result.Value.Velocity);
    }

    [Fact]
    public void InsideBoxStopsASlowApproachOnTheUpperWall()
    {
        var result = Bounce.InsideBox((0.0, 4.5), (1.0, 0.05), 1.0, (-5.0, -5.0), (5.0, 5.0), 0.8, 0.1);

        Assert.NotNull(result);
        AssertClose((0.0, 4.0), result.Value.Position);
        AssertClose((1.0, 0.0), result.Value.Velocity);
    }

    [Fact]
    public void InsideBallScalesTheNormalSpeedByTheRestitution()
    {
        var result = Bounce.InsideBall((0.0, 4.5), (1.0, 2.0), 1.0, (0.0, 0.0), 5.0, 0.5, 0.0);

        Assert.NotNull(result);
        AssertClose((0.0, 4.0), result.Value.Position);
        AssertClose((1.0, -1.0), result.Value.Velocity);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    public void OffBallRejectsAnInvalidRestitution(double restitution)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Bounce.OffBall((0.5, 0.0), (0.0, 0.0), 1.0, (0.0, 0.0), 0.0, restitution, 0.0));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    public void OffBoxInsideRejectsAnInvalidRestitution(double restitution)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Bounce.OffBox((0.5, 0.5), (0.0, 0.0), 1.0, (0.0, 0.0), (1.0, 1.0), restitution, 0.0));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    public void InsideBoxRejectsAnInvalidRestitution(double restitution)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Bounce.InsideBox((0.5, 0.0), (0.0, 0.0), 1.0, (0.0, 0.0), (1.0, 1.0), restitution, 0.0));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    public void OffPlaneRejectsAnInvalidRestingSpeed(double restingSpeed)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Bounce.OffPlane((0.0, 0.5), (0.0, 0.0), 1.0, (0.0, 0.0), (0.0, 1.0), 1.0, restingSpeed));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    public void InsideBallRejectsAnInvalidRestingSpeed(double restingSpeed)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Bounce.InsideBall((0.5, 0.0), (0.0, 0.0), 1.0, (0.0, 0.0), 5.0, 1.0, restingSpeed));
    }

    [Fact]
    public void OffBallRejectsANegativeObstacleRadius()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Bounce.OffBall((0.5, 0.0), (0.0, 0.0), 1.0, (0.0, 0.0), -1.0, 1.0, 0.0));
    }

    [Fact]
    public void InsideBallRejectsANegativeContainerRadius()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Bounce.InsideBall((0.5, 0.0), (0.0, 0.0), 1.0, (0.0, 0.0), -1.0, 1.0, 0.0));
    }

    [Fact]
    public void OffTriangleBouncesOffALeg()
    {
        var result = Bounce.OffTriangle((1.0, -0.5), (0.5, 2.0), 1.0, (0.0, 0.0), (4.0, 0.0), (0.0, 4.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((1.0, -1.0), result.Value.Position);
        AssertClose((0.5, -2.0), result.Value.Velocity);
    }

    [Fact]
    public void OffTriangleBouncesOffASlantedEdge()
    {
        var result = Bounce.OffTriangle((3.0, 2.0), (-1.0, -1.0), 1.0, (0.0, 0.0), (4.0, 0.0), (0.0, 4.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((2.5 + Math.Sqrt(0.5), 1.5 + Math.Sqrt(0.5)), result.Value.Position);
        AssertClose((1.0, 1.0), result.Value.Velocity);
    }

    [Fact]
    public void OffTriangleBouncesOffAVertex()
    {
        var result = Bounce.OffTriangle((-0.5, -0.5), (1.0, 1.0), 1.0, (0.0, 0.0), (4.0, 0.0), (0.0, 4.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((-Math.Sqrt(0.5), -Math.Sqrt(0.5)), result.Value.Position);
        AssertClose((-1.0, -1.0), result.Value.Velocity);
    }

    [Fact]
    public void OffTriangleLetsACentreInsideLeaveThroughTheNearestEdge()
    {
        var result = Bounce.OffTriangle((1.0, 0.5), (0.0, 1.0), 1.0, (0.0, 0.0), (4.0, 0.0), (0.0, 4.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((1.0, -1.0), result.Value.Position);
        AssertClose((0.0, -1.0), result.Value.Velocity);
    }

    [Fact]
    public void OffTriangleDoesNotDependOnTheOrderOfTheVertices()
    {
        var counterClockwise = Bounce.OffTriangle((1.0, 0.5), (0.3, 1.0), 1.0, (0.0, 0.0), (4.0, 0.0), (0.0, 4.0), 0.7, 0.0);
        var clockwise = Bounce.OffTriangle((1.0, 0.5), (0.3, 1.0), 1.0, (0.0, 4.0), (4.0, 0.0), (0.0, 0.0), 0.7, 0.0);

        Assert.NotNull(counterClockwise);
        Assert.NotNull(clockwise);
        AssertClose(counterClockwise.Value.Position, clockwise.Value.Position);
        AssertClose(counterClockwise.Value.Velocity, clockwise.Value.Velocity);
    }

    [Fact]
    public void OffTriangleIgnoresABodyThatDoesNotTouchIt()
    {
        Assert.Null(Bounce.OffTriangle((1.0, 5.0), (0.0, -1.0), 1.0, (0.0, 0.0), (4.0, 0.0), (0.0, 4.0), 1.0, 0.0));
    }

    [Fact]
    public void OffTriangleRejectsVerticesOnOneLine()
    {
        Assert.Throws<ArgumentException>(() =>
            Bounce.OffTriangle((1.0, 1.0), (0.0, 0.0), 1.0, (0.0, 0.0), (1.0, 1.0), (2.0, 2.0), 1.0, 0.0));
    }

    [Fact]
    public void SinglePrecisionGivesTheSameBounce()
    {
        var result = Bounce.OffBox((-0.5f, 0.0f), (3.0f, 0.5f), 1.0f, (0.0f, -1.0f), (4.0f, 1.0f), 1.0f, 0f);

        Assert.NotNull(result);
        Assert.Equal((-1.0f, 0.0f), result.Value.Position);
        Assert.Equal((-3.0f, 0.5f), result.Value.Velocity);
    }

    private static void AssertClose((double X, double Y) expected, (double X, double Y) actual)
    {
        Assert.Equal(expected.X, actual.X, Precision);
        Assert.Equal(expected.Y, actual.Y, Precision);
    }
}
