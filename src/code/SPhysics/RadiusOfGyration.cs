using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary> Radius of gyration: the distance at which the whole mass would have the same moment of inertia. </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Radius_of_gyration">wikipedia</a></remarks>
public static class RadiusOfGyration
{
    public const string Name = "Radius of gyration";
    public const string DefaultSymbol = "k";
    public const string Dimension = Length.Dimension;

    /// <summary> Radius of gyration, <c>sqrt(I / m)</c>. </summary>
    /// <param name="momentOfInertia"> Moment of inertia about the axis or point. </param>
    /// <param name="mass"> Total mass. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N momentOfInertia, N mass)
        where N : IRootFunctions<N>
        =>
        N.Sqrt(momentOfInertia / mass);
}
