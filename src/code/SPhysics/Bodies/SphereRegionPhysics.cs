using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary> Physics of a solid ball of uniform density, as members of <see cref="Sphere.Region"/>. </summary>
public static class SphereRegionPhysics
{
    extension(Sphere.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;

        /// <inheritdoc cref="CentralPressureFormulas"/>
        public static CentralPressureFormulas CentralPressure => default;

        /// <inheritdoc cref="GravitationalEnergyFormulas"/>
        public static GravitationalEnergyFormulas GravitationalEnergy => default;

        /// <inheritdoc cref="DragCoefficients"/>
        public static DragCoefficients DragCoefficient => default;

        /// <inheritdoc cref="EquilibriumTemperatureFormulas"/>
        public static EquilibriumTemperatureFormulas EquilibriumTemperature => default;
    }

    /// <summary> Mass of a solid ball. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a ball of the density and the radius. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N radius)
            where N : IFloatingPointConstants<N>
            => Mass.FromVolume(Sphere.Region.Volume.FromRadius(radius), density);
    }

    /// <summary> Moment of inertia of a solid ball about any axis through its centre. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a></remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> Two fifths of the mass times the squared radius. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromMass<N>(N mass, N radius)
            where N : INumberBase<N>
            => N.CreateChecked(2) * mass * radius * radius / N.CreateChecked(5);

        /// <summary> Moment of inertia of a ball of the density and the radius. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N radius)
            where N : IFloatingPointConstants<N>
            => FromMass(Sphere.Region.Mass.FromDensity(density, radius), radius);
    }

    /// <summary> Pressure at the centre of a self-gravitating ball. </summary>
    public readonly struct CentralPressureFormulas
    {
        /// <summary>Pressure at the center of a self-gravitating sphere of uniform density, P = 3 G M^2 / (8 pi R^4).</summary>
        /// <remarks>
        /// Follows from the hydrostatic equilibrium dP/dr = -G m(r) rho / r^2 integrated from the surface, where
        /// m(r) = 4/3 pi rho r^3, giving P = 2/3 pi G rho^2 R^2, the same value.
        /// Holds for a non-rotating, Newtonian sphere in hydrostatic equilibrium with constant density.
        /// Real bodies are denser towards the center, so their central pressure is higher: the Earth reaches about
        /// 364 GPa against 170 GPa of the uniform sphere, the Sun 26.5 PPa against 0.13 PPa. Treat the result as a lower bound.
        /// <a href="https://en.wikipedia.org/wiki/Hydrostatic_equilibrium">wikipedia</a>,
        /// <a href="https://home.strw.leidenuniv.nl/~keller/Teaching/Planets_2010/Planets2010_E04.pdf">derivation for a constant-density planet</a>,
        /// <a href="https://tohline.education/SelfGravitatingFluids/index.php/SSC/VirialEquilibrium/UniformDensity">uniform-density sphere</a>,
        /// <a href="https://en.wikipedia.org/wiki/Solar_core">central pressure of the Sun</a>,
        /// <a href="https://www.jamstec.go.jp/e/about/press_release/archive/2010/20100405.pdf">central pressure of the Earth</a>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromMass<N>(N mass, N radius, N gravitationalConstant)
            where N : IFloatingPointConstants<N>
        {
            N squaredRadius = radius * radius;
            return N.CreateTruncating(3) * gravitationalConstant * mass * mass
                / (N.CreateTruncating(8) * N.Pi * squaredRadius * squaredRadius);
        }
    }

    /// <summary> Gravitational self energy of a ball. </summary>
    public readonly struct GravitationalEnergyFormulas
    {
        /// <summary>
        /// Gravitational self energy of a uniform sphere, <c>-3/5 * G * m^2 / r</c>: the potential energy
        /// its own gravity holds. Its magnitude is the gravitational binding energy, the work needed to
        /// disperse the sphere to infinity.
        /// </summary>
        /// <remarks><a href="https://en.wikipedia.org/wiki/Gravitational_binding_energy">wikipedia</a></remarks>
        /// <typeparam name="N"> Number type </typeparam>
        /// <param name="mass"> Mass of the sphere. </param>
        /// <param name="radius"> Radius of the sphere. </param>
        /// <param name="gravitationConst"> constant in real world G = 6.67430e-11; // m^3 kg^-1 s^-2 </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromMass<N>(N mass, N radius, N gravitationConst)
            where N : INumberBase<N>
            =>
            -(N.CreateChecked(3) / N.CreateChecked(5)) * gravitationConst * mass * mass / radius;
    }

    /// <summary> Drag coefficients of a ball in real world, the input of <see cref="DragForce.InFluid{N}"/>. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/Drag_coefficient">wikipedia</a></remarks>
    public readonly struct DragCoefficients
    {
        /// <summary> Smooth sphere at the Reynolds number 10^6. </summary>
        public double Smooth => 0.1;

        /// <summary> Rough sphere at the Reynolds number 10^6. </summary>
        public double Rough => 0.47;
    }

    /// <summary> Equilibrium temperature of a fast rotating ball under the radiation of a distant source. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/Planetary_equilibrium_temperature">wikipedia</a></remarks>
    public readonly struct EquilibriumTemperatureFormulas
    {
        /// <summary>
        /// The ball absorbs over its cross section and re-radiates as a black body over its whole surface, four times larger:
        /// T = ((1 - albedo) * E / (4 * sigma))^(1/4).
        /// </summary>
        /// <param name="irradiance"> Irradiance of the source at the ball. </param>
        /// <param name="albedo"> Share of the radiation reflected back. </param>
        /// <param name="stefanBoltzmannConstant"> Stefan-Boltzmann constant, <see cref="ThermalRadiation.StefanBoltzmannConstant"/> in real world. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromIrradiance<N>(N irradiance, N albedo, N stefanBoltzmannConstant)
            where N : IRootFunctions<N>
            => ThermalRadiation.TemperatureFromPower((N.One - albedo) * irradiance, N.CreateTruncating(4), N.One, stefanBoltzmannConstant);
    }
}
