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

    /// <summary>
    /// Angular acceleration of a rigid body in space in the frame of its principal axes, Euler's equations,
    /// <c>alpha = I^-1 * (tau - omega x (I * omega))</c>.
    /// </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/Euler%27s_equations_(rigid_body_dynamics)">wikipedia</a></remarks>
    /// <param name="principalMoments"> Moments of inertia about the principal axes. </param>
    /// <param name="angularVelocity"> Angular velocity in the frame of the principal axes. </param>
    /// <param name="torque"> Torque in the frame of the principal axes. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N X, N Y, N Z) FromEulerEquations<N>((N A, N B, N C) principalMoments, (N X, N Y, N Z) angularVelocity, (N X, N Y, N Z) torque)
        where N : INumberBase<N>
    {
        var (a, b, c) = principalMoments;
        var (x, y, z) = angularVelocity;

        // in the principal frame omega x (I * omega) reduces to the differences of the moments
        return ((torque.X - ((c - b) * y * z)) / a,
                (torque.Y - ((a - c) * z * x)) / b,
                (torque.Z - ((b - a) * x * y)) / c);
    }

    /// <summary> Angular acceleration of a point on a circle of the radius with the given tangential acceleration. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromTangentialAcceleration<N>(N tangentialAcceleration, N radius)
        where N : IDivisionOperators<N, N, N>
        =>
        tangentialAcceleration / radius;
}
