using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary> Physics of a thick spherical shell of uniform density, a ball with a centred spherical cavity, as members of <see cref="SphericalShell.Region"/>. </summary>
public static class SphericalShellRegionPhysics
{
    extension(SphericalShell.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of a spherical shell. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a shell of the density and the radii. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N innerRadius, N outerRadius)
            where N : IFloatingPointConstants<N>
            => Mass.FromVolume(SphericalShell.Region.Volume.FromRadii(innerRadius, outerRadius), density);
    }

    /// <summary> Moment of inertia of a spherical shell about any axis through its centre. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a></remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> Two fifths of the mass times (r2^5 - r1^5) / (r2^3 - r1^3). </summary>
        /// <remarks>
        /// Evaluated with the common factor r2 - r1 cancelled, so a thin shell keeps its precision and an infinitely thin one,
        /// both radii equal, gives the hollow sphere 2/3 m r^2 instead of 0 / 0. An inner radius of zero gives the solid ball.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromMass<N>(N mass, N innerRadius, N outerRadius)
            where N : INumberBase<N>
        {
            N r1 = innerRadius;
            N r2 = outerRadius;
            N r1Squared = r1 * r1;
            N r2Squared = r2 * r2;
            N quartics = r2Squared * r2Squared + r2Squared * r2 * r1 + r2Squared * r1Squared + r2 * r1Squared * r1 + r1Squared * r1Squared;
            N quadratics = r2Squared + r2 * r1 + r1Squared;
            return N.CreateChecked(2) * mass * quartics / (N.CreateChecked(5) * quadratics);
        }

        /// <summary> Moment of inertia of a shell of the density and the radii. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N innerRadius, N outerRadius)
            where N : IFloatingPointConstants<N>
            => FromMass(SphericalShell.Region.Mass.FromDensity(density, innerRadius, outerRadius), innerRadius, outerRadius);
    }
}
