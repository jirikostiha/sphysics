using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary> Heat a body takes in or gives off as its temperature changes, through its specific heat capacity. </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Specific_heat_capacity">wikipedia</a></remarks>
public static class Heat
{
    public const string Name = "Heat";
    public const string DefaultSymbol = "Q";
    public const string Dimension = Energy.Dimension;

    /// <summary> Heat that changes the temperature of a body by <paramref name="temperatureChange"/>, <c>Q = m * c * dT</c>. </summary>
    /// <param name="mass"> Mass of the body. </param>
    /// <param name="specificHeatCapacity"> Specific heat capacity of its material. </param>
    /// <param name="temperatureChange"> Change of its temperature. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromTemperatureChange<N>(N mass, N specificHeatCapacity, N temperatureChange)
        where N : IMultiplyOperators<N, N, N>
        =>
        mass * specificHeatCapacity * temperatureChange;

    /// <summary> Change of the temperature of a body that takes in <paramref name="heat"/>, <c>dT = Q / (m * c)</c>. </summary>
    /// <param name="heat"> Heat taken in, negative when given off. </param>
    /// <param name="mass"> Mass of the body. </param>
    /// <param name="specificHeatCapacity"> Specific heat capacity of its material. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N TemperatureChange<N>(N heat, N mass, N specificHeatCapacity)
        where N : IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>
        =>
        heat / (mass * specificHeatCapacity);
}
