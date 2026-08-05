using BenchmarkDotNet.Attributes;

namespace SPhysics;

public class GravitationalForce_Benchmark
{
    private const int N = 10000;
    private const double G = 6.67430e-11;

    [Benchmark]
    public double Outside()
    {
        var sum = 0d;
        for (double i = 1; i < N; i++)
            sum += GravitationalForce.Outside(i, i + 3, i * 10);

        return sum;
    }
}
