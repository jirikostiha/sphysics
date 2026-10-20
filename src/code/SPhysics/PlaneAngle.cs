using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Plane angle quantity, a dimensionless ratio of lengths measured in radians.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Angle">wikipedia</a>
/// </remarks>
public static class PlaneAngle
{
    public const string Name = "plane angle";
    public const string DefaultSymbol = "θ";
    public const string Dimension = "1";

    /// <summary> Angle subtended by an arc of a circle, theta = s / r. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromArcLength<N>(N arcLength, N radius)
        where N : IDivisionOperators<N, N, N>
        =>
        arcLength / radius;

    /// <summary> Angle swept at a constant angular velocity over a time span. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromAngularVelocity<N>(N angularVelocity, N time)
        where N : IMultiplyOperators<N, N, N>
        =>
        angularVelocity * time;
}
