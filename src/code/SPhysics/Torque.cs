using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>Torque quantity, the rotational analogue of force.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Torque">wikipedia</a></remarks>
public static class Torque
{
    public const string Name = "torque";
    public const string DefaultSymbol = "τ";
    public const string Dimension = Mass.Dimension + " " + Length.Dimension + "+2 " + Time.Dimension + "-2";

    /// <summary> Torque of a force acting perpendicular to the lever arm. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N force, N leverArm)
        where N : IMultiplyOperators<N, N, N>
        =>
        force * leverArm;

    /// <summary> Torque of a force acting at an angle in rads to the lever arm, <c>r * F * sin(theta)</c>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N force, N leverArm, N angle)
        where N : ITrigonometricFunctions<N>
        =>
        force * leverArm * N.Sin(angle);

    /// <summary> Torque that gives a body the angular acceleration, Newton's second law for rotation. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromAngularAcceleration<N>(N momentOfInertia, N angularAcceleration)
        where N : IMultiplyOperators<N, N, N>
        =>
        momentOfInertia * angularAcceleration;

    /// <summary> Torque as a change of angular momentum over a time span. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromAngularMomentum<N>(N angularMomentumChange, N time)
        where N : IDivisionOperators<N, N, N>
        =>
        angularMomentumChange / time;
}
