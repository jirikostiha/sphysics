using System.Numerics;
using System.Runtime.CompilerServices;

using SMath.Geometry3D;

namespace SPhysics;

/// <summary>Thermal radiation of a grey body, the Stefan-Boltzmann law.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Stefan%E2%80%93Boltzmann_law">wikipedia</a></remarks>
public static class ThermalRadiation
{
    /// <summary> Stefan-Boltzmann constant in real world, W m^-2 K^-4. </summary>
    public const double StefanBoltzmannConstant = 5.670374419e-8;

    /// <summary>Power radiated by a surface, P = emissivity * sigma * A * T^4.</summary>
    /// <param name="area"> Radiating surface area. </param>
    /// <param name="temperature"> Absolute temperature of the surface. </param>
    /// <param name="emissivity"> Emissivity, 1 for a black body. </param>
    /// <param name="stefanBoltzmannConstant"> Stefan-Boltzmann constant, <see cref="StefanBoltzmannConstant"/> in real world. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Power<N>(N area, N temperature, N emissivity, N stefanBoltzmannConstant)
        where N : INumberBase<N>
    {
        N squared = temperature * temperature;
        return emissivity * stefanBoltzmannConstant * area * (squared * squared);
    }

    /// <summary>Temperature of a surface that radiates the given power, the inverse of <see cref="Power{N}"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N TemperatureFromPower<N>(N power, N area, N emissivity, N stefanBoltzmannConstant)
        where N : IRootFunctions<N>
        =>
        FourthRoot(power / (emissivity * stefanBoltzmannConstant * area));

    /// <summary>Irradiance (flux density) at a distance from an isotropic source, E = P / (4 pi d^2).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Irradiance<N>(N power, N distance)
        where N : IFloatingPointConstants<N>
        =>
        power / Sphere.Surface.Area.FromRadius(distance);

    // inverse of the fourth power of the temperature, two square roots are cheaper and more exact than Pow(x, 0.25)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static N FourthRoot<N>(N value)
        where N : IRootFunctions<N>
        => N.Sqrt(N.Sqrt(value));
}
