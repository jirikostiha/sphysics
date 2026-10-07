using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Velocity quantity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Velocity">wikipedia</a>
/// </remarks>
public static class Velocity
{
    public const string Name = "velocity";
    public const string DefaultSymbol = "v";
    public const string Dimension = Length.Dimension + " " + Time.Dimension + "-1";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromAcceleration<N>(N acceleration, N time)
        where N : IMultiplyOperators<N, N, N>
        =>
        acceleration * time;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromAngularVelocity<N>(N angularVelocity, N radius)
        where N : IMultiplyOperators<N, N, N>
        =>
        angularVelocity * radius;

    /// <summary> Average velocity over a distance travelled in a time span. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N distance, N time)
        where N : IDivisionOperators<N, N, N>
        =>
        distance / time;

    /// <summary> Velocity of a body of the given mass carrying the given momentum. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromMomentum<N>(N momentum, N mass)
        where N : IDivisionOperators<N, N, N>
        =>
        momentum / mass;

    /// <summary> Speed of a body of the given mass carrying the given kinetic energy, <c>sqrt(2 * E / m)</c>. </summary>
    /// <remarks> The inverse of <see cref="KineticEnergy.Linear{N}"/>; a negative energy has no real speed and gives NaN. </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromKineticEnergy<N>(N kineticEnergy, N mass)
        where N : IRootFunctions<N>
        =>
        N.Sqrt(N.CreateChecked(2) * kineticEnergy / mass);
}
