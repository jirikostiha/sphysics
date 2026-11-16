using System.Numerics;

using SMath.Geometry2D;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary>Moment of inertia quantity.</summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Moment_of_inertia">wikipedia</a>
/// <a href="https://mathworld.wolfram.com/MomentofInertia.html">wolfram</a>
/// <a href="https://en.wikipedia.org/wiki/Moment_of_inertia">wikipedia</a>
/// <a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a>
/// </remarks>
public static class MomentOfInertia
{
    public const string Name = "Moment of inertia";
    public const string DefaultSymbol = "I";
    public const string Dimension = Mass.Dimension + " " + Length.Dimension + "+2";

    public static N Eval<N>(N angularMomentum, N angularVelocity)
        where N : IDivisionOperators<N, N, N>
        =>
        angularMomentum / angularVelocity;

    /// <summary>
    /// Moment of inertia about an axis parallel to one through the centre of mass, the parallel axis (Steiner) theorem,
    /// <c>I = I_cm + m * d^2</c>.
    /// </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/Parallel_axis_theorem">wikipedia</a></remarks>
    /// <param name="centerOfMassMoment"> Moment of inertia about the axis through the centre of mass. </param>
    /// <param name="mass"> Mass of the body. </param>
    /// <param name="distance"> Distance between the two parallel axes. </param>
    public static N ParallelAxis<N>(N centerOfMassMoment, N mass, N distance)
        where N : IMultiplyOperators<N, N, N>, IAdditionOperators<N, N, N>
        =>
        centerOfMassMoment + mass * (distance * distance);

    #region objects

    /// <summary> Point mass at a distance from the axis of rotation.. </summary>
    public static N Point<N>(N mass, N distanceRadius)
        where N : IMultiplyOperators<N, N, N>
        =>
        mass * (distanceRadius * distanceRadius);

    /// <summary> Two point masses at a distance from the axis of rotation.. </summary>
    public static N TwoPoints<N>(N mass1, N mass2, N distance)
        where N : IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>, IAdditionOperators<N, N, N>
        =>
        ReducedMass.Eval(mass1, mass2) * (distance * distance);

    /// <summary>
    /// Point masses in a plane about the axis through <paramref name="origin"/> perpendicular to the plane,
    /// <c>sum(m * |r - o|^2)</c>.
    /// </summary>
    /// <param name="positions"> Positions of the points. </param>
    /// <param name="masses"> Masses of the points, as many as there are positions. </param>
    /// <param name="origin"> Point the axis passes through. </param>
    public static N Total<N>(ReadOnlySpan<(N X, N Y)> positions, ReadOnlySpan<N> masses, (N X, N Y) origin)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length);

        N total = N.Zero;
        for (int i = 0; i < positions.Length; i++)
        {
            var arm = GeometricVector2.Direction.FromCartesian(origin, positions[i]);
            total += masses[i] * GeometricVector2.DotProduct.FromCartesian(arm, arm);
        }

        return total;
    }

    /// <summary>
    /// Polar moment of point masses in space about the point <paramref name="origin"/>, <c>sum(m * |r - o|^2)</c>; it is
    /// half the sum of the moments about three perpendicular axes through the point, not a moment about an axis.
    /// </summary>
    /// <param name="positions"> Positions of the points. </param>
    /// <param name="masses"> Masses of the points, as many as there are positions. </param>
    /// <param name="origin"> Point the moment is taken about. </param>
    public static N Polar<N>(ReadOnlySpan<(N X, N Y, N Z)> positions, ReadOnlySpan<N> masses, (N X, N Y, N Z) origin)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length);

        N total = N.Zero;
        for (int i = 0; i < positions.Length; i++)
        {
            var arm = GeometricVector3.Direction.FromCartesian(origin, positions[i]);
            total += masses[i] * GeometricVector3.DotProduct.FromCartesian(arm, arm);
        }

        return total;
    }

    #endregion
}
