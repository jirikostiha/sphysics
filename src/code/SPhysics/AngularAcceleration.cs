using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>Angular acceleration quantity, the rate of change of angular velocity.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Angular_acceleration">wikipedia</a></remarks>
public static class AngularAcceleration
{
    public const string Name = "angular acceleration";
    public const string DefaultSymbol = "α";
    public const string Dimension = Time.Dimension + "-2";

    /// <summary> Angular acceleration as a change of angular velocity over a time span. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N angularVelocityChange, N time)
        where N : IDivisionOperators<N, N, N>
        =>
        angularVelocityChange / time;

    /// <summary> Angular acceleration of a body under a torque, Newton's second law for rotation. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromTorque<N>(N torque, N momentOfInertia)
        where N : IDivisionOperators<N, N, N>
        =>
        torque / momentOfInertia;

    /// <summary> Angular acceleration of a point on a circle of the radius with the given tangential acceleration. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromTangentialAcceleration<N>(N tangentialAcceleration, N radius)
        where N : IDivisionOperators<N, N, N>
        =>
        tangentialAcceleration / radius;
}
