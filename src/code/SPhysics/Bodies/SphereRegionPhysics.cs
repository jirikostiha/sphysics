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
}
