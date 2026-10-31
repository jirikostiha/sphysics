using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>Work, the energy transferred by a force along a displacement.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Work_(physics)">wikipedia</a></remarks>
public static class Work
{
    public const string Name = "work";
    public const string DefaultSymbol = "W";
    public const string Dimension = Energy.Dimension;

    /// <summary> Work of a constant force along a displacement in its direction. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N force, N displacement)
        where N : IMultiplyOperators<N, N, N>
        =>
        force * displacement;

    /// <summary> Work of a constant force at an angle in rads to the displacement, <c>F * d * cos(theta)</c>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N force, N displacement, N angle)
        where N : ITrigonometricFunctions<N>
        =>
        force * displacement * N.Cos(angle);

    /// <summary> Work of a constant torque turning a body through an angle in rads. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromTorque<N>(N torque, N angle)
        where N : IMultiplyOperators<N, N, N>
        =>
        torque * angle;

    /// <summary> Work of a constant power over a time span. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromPower<N>(N power, N time)
        where N : IMultiplyOperators<N, N, N>
        =>
        power * time;
}
