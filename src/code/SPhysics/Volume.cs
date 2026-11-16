using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary> Volume a mass of a given density takes up; the geometry of a volume is in SMath. </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Density">wikipedia</a></remarks>
public static class Volume
{
    public const string Name = "Volume";
    public const string DefaultSymbol = "V";
    public const string Dimension = Length.Dimension + "+3";

    /// <summary> Volume of <paramref name="mass"/> of the <paramref name="density"/>, <c>V = m / rho</c>; the inverse of <see cref="Mass.FromVolume{N}"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromMass<N>(N mass, N density)
        where N : IDivisionOperators<N, N, N>
        =>
        mass / density;
}
