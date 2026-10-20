using Xunit;

namespace SPhysics.Tests;

public class NBodyGravityTests
{
    private static readonly (double X, double Y)[] _planar = [(0.0, 0.0), (3.0, 4.0), (-2.0, 1.0), (1.5, -2.5)];
    private static readonly (double X, double Y, double Z)[] _spatial = [(0.0, 0.0, 0.0), (1.0, 2.0, 3.0), (-2.0, 1.0, 0.5), (0.5, -1.0, -2.0)];
    private static readonly double[] _masses = [5.0, 1.0, 2.0, 3.0];

    [Fact]
    public void ComputeAccelerations3DTwoBodiesPullEachOtherAlongTheirLine()
    {
        (double X, double Y, double Z)[] positions = [(1.0, -1.0, 2.0), (1.0, -1.0, 4.0)];
        var accelerations = new (double X, double Y, double Z)[2];

        NBodyGravity.ComputeAccelerations<double>(positions, [1.0, 3.0], 1.0, 0.0, accelerations);

        // G * m / r^2 with r = 2, pointing at the other body.
        Assert.Equal((0.0, 0.0, 0.75), accelerations[0]);
        Assert.Equal((0.0, 0.0, -0.25), accelerations[1]);
    }

    [Fact]
    public void ComputeAccelerations3DPlanarBodiesMatchThe2DResult()
    {
        var planarAccelerations = new (double X, double Y)[_planar.Length];
        var spatialAccelerations = new (double X, double Y, double Z)[_planar.Length];
        var spatial = _planar.Select(p => (p.X, p.Y, 0.0)).ToArray();

        NBodyGravity.ComputeAccelerations<double>(_planar, _masses, 1.5, 0.1, planarAccelerations);
        NBodyGravity.ComputeAccelerations<double>(spatial, _masses, 1.5, 0.1, spatialAccelerations);

        Assert.Equal(planarAccelerations, spatialAccelerations.Select(a => (a.X, a.Y)));
        Assert.All(spatialAccelerations, a => Assert.Equal(0.0, a.Z));
    }

    [Fact]
    public void ComputeAccelerations3DManyBodiesKeepTheMomentum()
    {
        var accelerations = new (double X, double Y, double Z)[_spatial.Length];

        NBodyGravity.ComputeAccelerations<double>(_spatial, _masses, 1.0, 0.1, accelerations);

        Assert.Equal(0.0, accelerations.Select((a, i) => a.X * _masses[i]).Sum(), 12);
        Assert.Equal(0.0, accelerations.Select((a, i) => a.Y * _masses[i]).Sum(), 12);
        Assert.Equal(0.0, accelerations.Select((a, i) => a.Z * _masses[i]).Sum(), 12);
    }

    [Fact]
    public void ComputeAccelerations3DSameConstantForEveryBodyMatchesTheSharedConstant()
    {
        var shared = new (double X, double Y, double Z)[_spatial.Length];
        var perBody = new (double X, double Y, double Z)[_spatial.Length];

        NBodyGravity.ComputeAccelerations<double>(_spatial, _masses, 2.0, 0.1, shared);
        NBodyGravity.ComputeAccelerations<double>(_spatial, _masses, [2.0, 2.0, 2.0, 2.0], 0.1, perBody);

        Assert.Equal(shared, perBody);
    }

    [Fact]
    public void ComputeAccelerations3DPerBodyConstantScalesOnlyThatBody()
    {
        (double X, double Y, double Z)[] positions = [(0.0, 0.0, 0.0), (0.0, 0.0, 1.0)];
        var accelerations = new (double X, double Y, double Z)[2];

        NBodyGravity.ComputeAccelerations<double>(positions, [1.0, 1.0], [0.0, 1.0], 0.0, accelerations);

        Assert.Equal((0.0, 0.0, 0.0), accelerations[0]);
        Assert.Equal((0.0, 0.0, -1.0), accelerations[1]);
    }

    [Fact]
    public void ComputeAccelerations3DMismatchedLengthsThrow()
    {
        var positions = new (double X, double Y, double Z)[2];

        Assert.Throws<ArgumentException>(() => NBodyGravity.ComputeAccelerations<double>(positions, [1.0], 1.0, 0.0, new (double, double, double)[2]));
        Assert.Throws<ArgumentException>(() => NBodyGravity.ComputeAccelerations<double>(positions, [1.0, 1.0], [1.0], 0.0, new (double, double, double)[2]));
    }
}
