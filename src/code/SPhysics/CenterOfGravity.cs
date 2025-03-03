using System.Numerics;

namespace SPhysics;

/// <summary>
/// Center of gravity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Center_of_mass#Center_of_gravity">wikipedia</a>
/// </remarks>
public static class CenterOfGravity
{
    public static (N x, N y) Eval<N>((N x, N y, N m, N g)[] points)
           where N : INumberBase<N>
    {
        N totalWeight = points.Aggregate(N.Zero, (sum, point) => sum + point.m * point.g);

        N x = points.Aggregate(N.Zero, (sum, point) => point.x * point.m * point.g) / totalWeight;
        N y = points.Aggregate(N.Zero, (sum, point) => point.y * point.m * point.g) / totalWeight;

        return (x, y);
    }
}