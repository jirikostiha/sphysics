using System.Numerics;

namespace SPhysics;

/// <summary>Center of gravity.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Center_of_mass#Center_of_gravity">wikipedia</a></remarks>
public static class CenterOfGravity
{
    /// <summary> Center of gravity of points in a plane, each weighted by its weight <c>m * g</c>. </summary>
    /// <param name="positions"> Positions of the points. </param>
    /// <param name="masses"> Masses of the points, as many as there are positions. </param>
    /// <param name="gravities"> Gravitational acceleration at each point, as many as there are positions. </param>
    public static (N X, N Y) Eval<N>(ReadOnlySpan<(N X, N Y)> positions, ReadOnlySpan<N> masses, ReadOnlySpan<N> gravities)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length, gravities.Length);

        N totalWeight = N.Zero;
        N x = N.Zero;
        N y = N.Zero;

        for (int i = 0; i < positions.Length; i++)
        {
            N weight = masses[i] * gravities[i];

            totalWeight += weight;
            x += positions[i].X * weight;
            y += positions[i].Y * weight;
        }

        return (x / totalWeight, y / totalWeight);
    }
}
