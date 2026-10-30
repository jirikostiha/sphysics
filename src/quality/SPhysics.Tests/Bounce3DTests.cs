using Xunit;

namespace SPhysics.Tests;

/// <summary> The 3D bounces lying in the plane z = 0 must match the 2D ones; the rest checks what only space has. </summary>
public class Bounce3DTests
{
    private const int Precision = 12;

    [Fact]
    public void OffBallInThePlaneMatchesThe2DResult()
    {
        var planar = Bounce.OffBall((1.5, 0.4), (-2.0, 1.0), 1.0, (0.0, 0.0), 1.0, 0.7, 0.0);
        var spatial = Bounce.OffBall((1.5, 0.4, 0.0), (-2.0, 1.0, 0.0), 1.0, (0.0, 0.0, 0.0), 1.0, 0.7, 0.0);

        AssertMatches(planar, spatial);
    }

    [Fact]
    public void OffSegmentInThePlaneMatchesThe2DResult()
    {
        var planar = Bounce.OffSegment((1.0, 0.5), (0.5, -2.0), 1.0, (0.0, 0.0), (4.0, 1.0), 0.7, 0.0);
        var spatial = Bounce.OffSegment((1.0, 0.5, 0.0), (0.5, -2.0, 0.0), 1.0, (0.0, 0.0, 0.0), (4.0, 1.0, 0.0), 0.7, 0.0);

        AssertMatches(planar, spatial);
    }

    [Fact]
    public void OffPlaneInThePlaneMatchesThe2DResult()
    {
        var planar = Bounce.OffPlane((0.2, 0.5), (2.0, -3.0), 1.0, (0.0, 0.0), (1.0, 2.0), 0.7, 0.0);
        var spatial = Bounce.OffPlane((0.2, 0.5, 0.0), (2.0, -3.0, 0.0), 1.0, (0.0, 0.0, 0.0), (1.0, 2.0, 0.0), 0.7, 0.0);

        AssertMatches(planar, spatial);
    }

    [Fact]
    public void OffSegmentAlongZBouncesOffItsMiddle()
    {
        var result = Bounce.OffSegment((0.5, 0.0, 2.0), (-1.0, 0.0, 0.3), 1.0, (0.0, 0.0, 0.0), (0.0, 0.0, 4.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((1.0, 0.0, 2.0), result.Value.Position);
        AssertClose((1.0, 0.0, 0.3), result.Value.Velocity);
    }

    [Fact]
    public void OffBoxReflectsOffTheFaceAcrossZ()
    {
        var result = Bounce.OffBox((0.5, 0.5, 1.5), (1.0, 0.0, -2.0), 1.0, (0.0, 0.0, 0.0), (1.0, 1.0, 1.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((0.5, 0.5, 2.0), result.Value.Position);
        AssertClose((1.0, 0.0, 2.0), result.Value.Velocity);
    }

    [Fact]
    public void OffBoxLetsACentreInsideLeaveThroughTheNearestFaceOnZ()
    {
        var result = Bounce.OffBox((0.5, 0.5, 0.9), (0.0, 0.0, -1.0), 1.0, (0.0, 0.0, 0.0), (1.0, 1.0, 1.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((0.5, 0.5, 2.0), result.Value.Position);
        AssertClose((0.0, 0.0, 1.0), result.Value.Velocity);
    }

    /// <summary> Equally near faces are taken in the order x, y, z and the lower face before the upper one. </summary>
    [Theory]
    [InlineData(0.5, 0.5, 0.5, -1.0, 0.5, 0.5)]
    [InlineData(1.0, 0.5, 0.5, 1.0, -1.0, 0.5)]
    [InlineData(1.0, 1.0, 0.5, 1.0, 1.0, -1.0)]
    [InlineData(1.0, 1.0, 1.5, 1.0, 1.0, 3.0)]
    public void OffBoxLetsACentreEquallyNearToSeveralFacesLeaveThroughTheFirst(
        double x, double y, double z, double exitX, double exitY, double exitZ)
    {
        var result = Bounce.OffBox((x, y, z), (0.0, 0.0, 0.0), 1.0, (0.0, 0.0, 0.0), (2.0, 2.0, 2.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((exitX, exitY, exitZ), result.Value.Position);
    }

    [Fact]
    public void InsideBoxBouncesOffEveryWallOfACorner()
    {
        var result = Bounce.InsideBox((4.5, -4.8, 4.2), (1.0, -2.0, 3.0), 1.0, (-5.0, -5.0, -5.0), (5.0, 5.0, 5.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((4.0, -4.0, 4.0), result.Value.Position);
        AssertClose((-1.0, 2.0, -3.0), result.Value.Velocity);
    }

    [Fact]
    public void InsideBallReflectsABodyReachingTheWallAlongZ()
    {
        var result = Bounce.InsideBall((0.0, 0.0, -4.5), (1.0, 0.0, -2.0), 1.0, (0.0, 0.0, 0.0), 5.0, 0.5, 0.0);

        Assert.NotNull(result);
        AssertClose((0.0, 0.0, -4.0), result.Value.Position);
        AssertClose((1.0, 0.0, 1.0), result.Value.Velocity);
    }

    [Fact]
    public void OffTriangleInThePlaneMatchesThe2DResult()
    {
        var planar = Bounce.OffTriangle((3.0, 2.0), (-1.0, -1.0), 1.0, (0.0, 0.0), (4.0, 0.0), (0.0, 4.0), 0.7, 0.0);
        var spatial = Bounce.OffTriangle(
            (3.0, 2.0, 0.0), (-1.0, -1.0, 0.0), 1.0, (0.0, 0.0, 0.0), (4.0, 0.0, 0.0), (0.0, 4.0, 0.0), 0.7, 0.0);

        AssertMatches(planar, spatial);
    }

    [Fact]
    public void OffTriangleBouncesOffItsFace()
    {
        var result = Bounce.OffTriangle(
            (1.0, 1.0, 0.5), (0.5, 0.0, -2.0), 1.0, (0.0, 0.0, 0.0), (4.0, 0.0, 0.0), (0.0, 4.0, 0.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((1.0, 1.0, 1.0), result.Value.Position);
        AssertClose((0.5, 0.0, 2.0), result.Value.Velocity);
    }

    [Fact]
    public void OffTriangleBouncesOffAnEdgeBesideIt()
    {
        var result = Bounce.OffTriangle(
            (1.0, -0.5, 0.2), (0.0, 2.0, 0.0), 1.0, (0.0, 0.0, 0.0), (4.0, 0.0, 0.0), (0.0, 4.0, 0.0), 1.0, 0.0);

        // the nearest point is (1, 0, 0), at the distance d from the centre
        var d = Math.Sqrt(0.29);
        Assert.NotNull(result);
        AssertClose((1.0, -0.5 / d, 0.2 / d), result.Value.Position);
        AssertClose((0.0, 2.0 - 1.0 / 0.29, 0.4 / 0.29), result.Value.Velocity);
    }

    [Theory]
    [InlineData(-1.0, 1.0)]
    [InlineData(1.0, -1.0)]
    public void OffTriangleLetsACentreOnItLeaveOnTheSideItCameFrom(double velocityZ, double expectedSide)
    {
        var result = Bounce.OffTriangle(
            (1.0, 1.0, 0.0), (0.0, 0.0, velocityZ), 1.0, (0.0, 0.0, 0.0), (4.0, 0.0, 0.0), (0.0, 4.0, 0.0), 1.0, 0.0);

        Assert.NotNull(result);
        AssertClose((1.0, 1.0, expectedSide), result.Value.Position);
        AssertClose((0.0, 0.0, expectedSide), result.Value.Velocity);
    }

    [Fact]
    public void OffTriangleRejectsVerticesOnOneLine()
    {
        Assert.Throws<ArgumentException>(() =>
            Bounce.OffTriangle((1.0, 1.0, 0.0), (0.0, 0.0, 0.0), 1.0, (0.0, 0.0, 0.0), (1.0, 1.0, 1.0), (2.0, 2.0, 2.0), 1.0, 0.0));
    }

    [Fact]
    public void OffPlaneRejectsANormalOfZeroLength()
    {
        Assert.Throws<ArgumentException>(() =>
            Bounce.OffPlane((0.0, 0.0, 0.0), (0.0, 0.0, 0.0), 1.0, (0.0, 0.0, 0.0), (0.0, 0.0, 0.0), 1.0, 0.0));
    }

    private static void AssertMatches(
        ((double X, double Y) Position, (double X, double Y) Velocity)? planar,
        ((double X, double Y, double Z) Position, (double X, double Y, double Z) Velocity)? spatial)
    {
        Assert.NotNull(planar);
        Assert.NotNull(spatial);
        AssertClose((planar.Value.Position.X, planar.Value.Position.Y, 0.0), spatial.Value.Position);
        AssertClose((planar.Value.Velocity.X, planar.Value.Velocity.Y, 0.0), spatial.Value.Velocity);
    }

    private static void AssertClose((double X, double Y, double Z) expected, (double X, double Y, double Z) actual)
    {
        Assert.Equal(expected.X, actual.X, Precision);
        Assert.Equal(expected.Y, actual.Y, Precision);
        Assert.Equal(expected.Z, actual.Z, Precision);
    }
}
