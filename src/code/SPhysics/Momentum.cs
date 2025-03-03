using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Momentum quantity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Momentum">wikipedia</a>
/// </remarks>
public static class Momentum
{
    public const string Name = "Momentum";
    public const string DefaultSymbol = "p";
    public const string Dimension = Mass.Dimension + " " + Length.Dimension + " " + Time.Dimension + "-1";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Scalar<N>(N mass, N velocity)
        where N : INumberBase<N>
        =>
        mass * velocity;

    public static (N px, N py) Total<N>((N vx, N vy, N m)[] bodies)
        where N : INumberBase<N>
    {
        N px = bodies.Aggregate(N.Zero, (sum, body) => body.vx * body.m);
        N py = bodies.Aggregate(N.Zero, (sum, body) => body.vy * body.m);

        return (px, py);
    }
}
