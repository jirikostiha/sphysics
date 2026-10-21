using Xunit;

namespace SPhysics.Tests;

public class NBodyGravitySoaTests
{
    private const int Runs = 400;

    [Fact]
    public void SoaMatchesTheDirectSumUpToRounding()
    {
        var random = new Random(5);
        for (int run = 0; run < Runs; run++)
        {
            int n = random.Next(0, 70);
            var (x, y, z) = (Values(random, n), Values(random, n), Values(random, n));
            var masses = Values(random, n).Select(Math.Abs).ToArray();
            var constants = Values(random, n).Select(v => Math.Abs(v) / 50).ToArray();
            var shared = Enumerable.Repeat(0.7, n).ToArray();
            var zeros = new double[n];
            var (ax, ay, az) = (new double[n], new double[n], new double[n]);

            NBodyGravity.Soa.ComputeAccelerations<double>(x, y, masses, constants, 0.3, ax, ay);
            AssertClose(DirectSum(x, y, zeros, masses, constants, 0.3), ax, ay, zeros);

            NBodyGravity.Soa.ComputeAccelerations<double>(x, y, masses, 0.7, 0.3, ax, ay);
            AssertClose(DirectSum(x, y, zeros, masses, shared, 0.3), ax, ay, zeros);

            NBodyGravity.Soa.ComputeAccelerations<double>(x, y, z, masses, constants, 0.3, ax, ay, az);
            AssertClose(DirectSum(x, y, z, masses, constants, 0.3), ax, ay, az);

            NBodyGravity.Soa.ComputeAccelerations<double>(x, y, z, masses, 0.7, 0.3, ax, ay, az);
            AssertClose(DirectSum(x, y, z, masses, shared, 0.3), ax, ay, az);
        }
    }

    [Fact]
    public void TupleOverloadsRunTheSoaKernel()
    {
        var random = new Random(3);
        for (int run = 0; run < Runs; run++)
        {
            int n = random.Next(0, 40);
            var (x, y, z) = (Values(random, n), Values(random, n), Values(random, n));
            var masses = Values(random, n).Select(Math.Abs).ToArray();
            var constants = Values(random, n).Select(v => Math.Abs(v) / 50).ToArray();
            var (ax, ay, az) = (new double[n], new double[n], new double[n]);
            var planar = new (double X, double Y)[n];
            var spatial = new (double X, double Y, double Z)[n];

            NBodyGravity.ComputeAccelerations<double>(x.Zip(y).ToArray(), masses, constants, 0.3, planar);
            NBodyGravity.Soa.ComputeAccelerations<double>(x, y, masses, constants, 0.3, ax, ay);
            Assert.Equal(ax.Zip(ay), planar);

            NBodyGravity.ComputeAccelerations<double>(Spatial(x, y, z), masses, 0.7, 0.3, spatial);
            NBodyGravity.Soa.ComputeAccelerations<double>(x, y, z, masses, 0.7, 0.3, ax, ay, az);
            Assert.Equal(Spatial(ax, ay, az), spatial);
        }
    }

    [Fact]
    public void SoaWithEqualConstantsPerBodyMatchesTheSharedConstant()
    {
        var random = new Random(7);
        var (x, y, z) = (Values(random, 37), Values(random, 37), Values(random, 37));
        var masses = Values(random, 37).Select(Math.Abs).ToArray();
        var (ax1, ay1, az1) = (new double[37], new double[37], new double[37]);
        var (ax2, ay2, az2) = (new double[37], new double[37], new double[37]);

        NBodyGravity.Soa.ComputeAccelerations<double>(x, y, z, masses, Enumerable.Repeat(0.7, 37).ToArray(), 0.1, ax1, ay1, az1);
        NBodyGravity.Soa.ComputeAccelerations<double>(x, y, z, masses, 0.7, 0.1, ax2, ay2, az2);

        Assert.Equal(ax2, ax1);
        Assert.Equal(ay2, ay1);
        Assert.Equal(az2, az1);
    }

    [Fact]
    public void SoaRunsTheScalarKernelForNumberTypesWithoutVectors()
    {
        Half[] x = [(Half)0, (Half)2];
        Half[] y = [(Half)0, (Half)0];
        var (ax, ay) = (new Half[2], new Half[2]);

        NBodyGravity.Soa.ComputeAccelerations<Half>(x, y, [(Half)1, (Half)4], (Half)1, (Half)0, ax, ay);

        // G * m / r^2 with r = 2, pointing at the other body
        Assert.Equal((Half)1, ax[0]);
        Assert.Equal((Half)(-0.25), ax[1]);
    }

    [Fact]
    public void SoaRejectsSpansOfDifferentLengths()
    {
        double[] two = [0, 1];
        double[] three = [0, 1, 2];

        Assert.Throws<ArgumentException>(() => NBodyGravity.Soa.ComputeAccelerations<double>(two, three, two, 1.0, 0.0, new double[2], new double[2]));
        Assert.Throws<ArgumentException>(() => NBodyGravity.Soa.ComputeAccelerations<double>(two, two, two, three, 0.0, new double[2], new double[2]));
        Assert.Throws<ArgumentException>(() => NBodyGravity.Soa.ComputeAccelerations<double>(two, two, two, [], 0.0, new double[2], new double[2]));
        Assert.Throws<ArgumentException>(() => NBodyGravity.Soa.ComputeAccelerations<double>(two, two, three, two, 1.0, 0.0, new double[2], new double[2], new double[2]));
        Assert.Throws<ArgumentException>(() => NBodyGravity.Soa.ComputeAccelerations<double>(two, two, [], two, 1.0, 0.0, new double[2], new double[2], new double[2]));
        Assert.Throws<ArgumentException>(() => NBodyGravity.Soa.ComputeAccelerations<double>(two, two, two, two, 1.0, 0.0, new double[2], new double[2], new double[3]));
    }

    // every pull summed directly, in a different order than the pair kernel
    private static (double X, double Y, double Z)[] DirectSum(
        double[] x, double[] y, double[] z, double[] masses, double[] constants, double softening)
    {
        var accelerations = new (double X, double Y, double Z)[x.Length];
        for (int i = 0; i < x.Length; i++)
        {
            for (int j = 0; j < x.Length; j++)
            {
                if (i == j)
                    continue;

                var (rx, ry, rz) = (x[j] - x[i], y[j] - y[i], z[j] - z[i]);
                double k = constants[i] * masses[j] / Math.Pow((rx * rx) + (ry * ry) + (rz * rz) + (softening * softening), 1.5);
                accelerations[i] = (accelerations[i].X + (rx * k), accelerations[i].Y + (ry * k), accelerations[i].Z + (rz * k));
            }
        }

        return accelerations;
    }

    // the pulls on a body may cancel out, so the rounding is measured against the largest acceleration
    private static void AssertClose((double X, double Y, double Z)[] expected, double[] ax, double[] ay, double[] az)
    {
        double tolerance = 1e-12 * expected.Select(a => Math.Abs(a.X) + Math.Abs(a.Y) + Math.Abs(a.Z)).DefaultIfEmpty().Max();
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.InRange(Math.Abs(ax[i] - expected[i].X), 0, tolerance);
            Assert.InRange(Math.Abs(ay[i] - expected[i].Y), 0, tolerance);
            Assert.InRange(Math.Abs(az[i] - expected[i].Z), 0, tolerance);
        }
    }

    private static (double X, double Y, double Z)[] Spatial(double[] x, double[] y, double[] z)
        => Enumerable.Range(0, x.Length).Select(i => (x[i], y[i], z[i])).ToArray();

    private static double[] Values(Random random, int n)
        => Enumerable.Range(0, n).Select(_ => (random.NextDouble() * 200) - 100).ToArray();
}
