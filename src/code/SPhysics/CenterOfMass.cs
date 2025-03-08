using System.Numerics;

namespace SPhysics;

/// <summary>
/// Center of mass or balance point.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Center_of_mass">wikipedia</a>
/// </remarks>
public static class CenterOfMass
{
    public static (N x, N y) Eval<N>((N x, N y, N m)[] points)
        where N : INumberBase<N>
    {
        N totalMass = points.Aggregate(N.Zero, (sum, point) => sum + point.m);

        N x = points.Aggregate(N.Zero, (sum, point) => point.x * point.m) / totalMass;
        N y = points.Aggregate(N.Zero, (sum, point) => point.y * point.m) / totalMass;

        return (x, y);
    }
}
