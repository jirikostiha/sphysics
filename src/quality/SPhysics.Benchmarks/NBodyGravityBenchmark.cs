using BenchmarkDotNet.Attributes;

namespace SPhysics;

/// <summary> One evaluation of all accelerations by each kernel, float bodies with a gravitational constant per body. </summary>
[MemoryDiagnoser]
public class NBodyGravityBenchmark
{
    private (float X, float Y)[] _planar = [];
    private (float X, float Y, float Z)[] _spatial = [];
    private float[] _x = [];
    private float[] _y = [];
    private float[] _z = [];
    private float[] _masses = [];
    private float[] _constants = [];
    private (float X, float Y)[] _planarAccelerations = [];
    private (float X, float Y, float Z)[] _spatialAccelerations = [];
    private float[] _ax = [];
    private float[] _ay = [];
    private float[] _az = [];

    [Params(4, 16, 64, 512, 3000)]
    public int Bodies { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        float[] Values() => Enumerable.Range(0, Bodies).Select(_ => (float)random.NextDouble() * 100).ToArray();

        (_x, _y, _z) = (Values(), Values(), Values());
        _masses = Values();
        _constants = Values();
        _planar = Enumerable.Range(0, Bodies).Select(i => (_x[i], _y[i])).ToArray();
        _spatial = Enumerable.Range(0, Bodies).Select(i => (_x[i], _y[i], _z[i])).ToArray();
        _planarAccelerations = new (float X, float Y)[Bodies];
        _spatialAccelerations = new (float X, float Y, float Z)[Bodies];
        (_ax, _ay, _az) = (new float[Bodies], new float[Bodies], new float[Bodies]);
    }

    [Benchmark]
    public void Scalar2D()
        => NBodyGravity.Scalar.ComputeAccelerations<float>(_planar, _masses, _constants, 0.1f, _planarAccelerations);

    [Benchmark]
    public void Tuple2D()
        => NBodyGravity.ComputeAccelerations<float>(_planar, _masses, _constants, 0.1f, _planarAccelerations);

    [Benchmark]
    public void Soa2D()
        => NBodyGravity.Soa.ComputeAccelerations<float>(_x, _y, _masses, _constants, 0.1f, _ax, _ay);

    [Benchmark]
    public void Scalar3D()
        => NBodyGravity.Scalar.ComputeAccelerations<float>(_spatial, _masses, _constants, 0.1f, _spatialAccelerations);

    [Benchmark]
    public void Tuple3D()
        => NBodyGravity.ComputeAccelerations<float>(_spatial, _masses, _constants, 0.1f, _spatialAccelerations);

    [Benchmark]
    public void Soa3D()
        => NBodyGravity.Soa.ComputeAccelerations<float>(_x, _y, _z, _masses, _constants, 0.1f, _ax, _ay, _az);
}
