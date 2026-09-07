using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Frequency quantity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Frequency">wikipedia</a>
/// </remarks>
public static class Frequency
{
    public const string Name = "frequency";
    public const string DefaultSymbol = "f";
    public const string Dimension = Time.Dimension + "-1";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N period)
        where N : INumberBase<N>
        =>
        N.One / period;

    /// <summary> Frequency of a rotation with the given angular velocity. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromAngularVelocity<N>(N angularVelocity)
        where N : ITrigonometricFunctions<N>
        =>
        angularVelocity / (N.CreateTruncating(2) * N.Pi);
}
