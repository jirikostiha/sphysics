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
        N totalMass = N.Zero;
        N x = N.Zero;
        N y = N.Zero;

        for (int i = 0; i < points.Length; i++)
        {
            var (px, py, m) = points[i];

            totalMass += m;
            x += px * m;
            y += py * m;
        }

        return (x / totalMass, y / totalMass);
    }
}
