using System.Numerics;

namespace SPhysics;

/// <summary>Center of mass or balance point.</summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Center_of_mass">wikipedia</a>
/// <para> Without mass the center is not defined: a floating point type then gives NaN. </para>
/// </remarks>
public static class CenterOfMass
{
    /// <summary> Center of mass of points in a plane, <c>sum(m * r) / sum(m)</c>. </summary>
    /// <param name="positions"> Positions of the points. </param>
    /// <param name="masses"> Masses of the points, as many as there are positions. </param>
    public static (N X, N Y) Eval<N>(ReadOnlySpan<(N X, N Y)> positions, ReadOnlySpan<N> masses)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length);

        N totalMass = N.Zero;
        N x = N.Zero;
        N y = N.Zero;

        for (int i = 0; i < positions.Length; i++)
        {
            totalMass += masses[i];
            x += positions[i].X * masses[i];
            y += positions[i].Y * masses[i];
        }

        return (x / totalMass, y / totalMass);
    }

    /// <summary> Center of mass of points in space, <c>sum(m * r) / sum(m)</c>. </summary>
    /// <param name="positions"> Positions of the points. </param>
    /// <param name="masses"> Masses of the points, as many as there are positions. </param>
    public static (N X, N Y, N Z) Eval<N>(ReadOnlySpan<(N X, N Y, N Z)> positions, ReadOnlySpan<N> masses)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length);

        N totalMass = N.Zero;
        N x = N.Zero;
        N y = N.Zero;
        N z = N.Zero;

        for (int i = 0; i < positions.Length; i++)
        {
            totalMass += masses[i];
            x += positions[i].X * masses[i];
            y += positions[i].Y * masses[i];
            z += positions[i].Z * masses[i];
        }

        return (x / totalMass, y / totalMass, z / totalMass);
    }

    /// <summary> Velocity of the center of mass of points in a plane, <c>sum(m * v) / sum(m)</c>. </summary>
    /// <param name="velocities"> Velocities of the points. </param>
    /// <param name="masses"> Masses of the points, as many as there are velocities. </param>
    public static (N X, N Y) Velocity<N>(ReadOnlySpan<(N X, N Y)> velocities, ReadOnlySpan<N> masses)
        where N : INumberBase<N>
        =>
        // the velocity is weighted by mass exactly as the position is
        Eval(velocities, masses);

    /// <summary> Velocity of the center of mass of points in space, <c>sum(m * v) / sum(m)</c>. </summary>
    /// <param name="velocities"> Velocities of the points. </param>
    /// <param name="masses"> Masses of the points, as many as there are velocities. </param>
    public static (N X, N Y, N Z) Velocity<N>(ReadOnlySpan<(N X, N Y, N Z)> velocities, ReadOnlySpan<N> masses)
        where N : INumberBase<N>
        =>
        Eval(velocities, masses);
}
