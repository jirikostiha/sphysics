using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>Momentum quantity.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Momentum">wikipedia</a></remarks>
public static class Momentum
{
    public const string Name = "Momentum";
    public const string DefaultSymbol = "p";
    public const string Dimension = Mass.Dimension + " " + Length.Dimension + " " + Time.Dimension + "-1";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Scalar<N>(N mass, N velocity)
        where N : IMultiplyOperators<N, N, N>
        =>
        mass * velocity;

    /// <summary> Total momentum of bodies in a plane, <c>sum(m * v)</c>. </summary>
    /// <param name="velocities"> Velocities of the bodies. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are velocities. </param>
    public static (N X, N Y) Total<N>(ReadOnlySpan<(N X, N Y)> velocities, ReadOnlySpan<N> masses)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(velocities.Length, masses.Length);

        N px = N.Zero;
        N py = N.Zero;

        for (int i = 0; i < velocities.Length; i++)
        {
            px += velocities[i].X * masses[i];
            py += velocities[i].Y * masses[i];
        }

        return (px, py);
    }

    /// <summary> Total momentum of bodies in space, <c>sum(m * v)</c>. </summary>
    /// <param name="velocities"> Velocities of the bodies. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are velocities. </param>
    public static (N X, N Y, N Z) Total<N>(ReadOnlySpan<(N X, N Y, N Z)> velocities, ReadOnlySpan<N> masses)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(velocities.Length, masses.Length);

        N px = N.Zero;
        N py = N.Zero;
        N pz = N.Zero;

        for (int i = 0; i < velocities.Length; i++)
        {
            px += velocities[i].X * masses[i];
            py += velocities[i].Y * masses[i];
            pz += velocities[i].Z * masses[i];
        }

        return (px, py, pz);
    }
}
