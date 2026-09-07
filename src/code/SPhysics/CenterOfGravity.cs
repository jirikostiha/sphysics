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
        N totalWeight = N.Zero;
        N x = N.Zero;
        N y = N.Zero;

        for (int i = 0; i < points.Length; i++)
        {
            var (px, py, m, g) = points[i];
            N weight = m * g;

            totalWeight += weight;
            x += px * weight;
            y += py * weight;
        }

        return (x / totalWeight, y / totalWeight);
    }
}
