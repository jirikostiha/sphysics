using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary> Physics of a solid ellipsoid of uniform density, given by its semi-axes, as members of <see cref="Ellipsoid.Region"/>. </summary>
public static class EllipsoidRegionPhysics
{
    extension(Ellipsoid.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of a solid ellipsoid. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of an ellipsoid of the density and the semi-axes. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N r1, N r2, N r3)
            where N : IFloatingPointConstants<N>
            => Mass.FromVolume(Ellipsoid.Region.Volume.FromRadii(r1, r2, r3), density);
    }

    /// <summary> Moments of inertia of a solid ellipsoid about its principal axes, along the semi-axes r1, r2 and r3. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a></remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> About each axis, the mass times the sum of the other two squared semi-axes, over five. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (N R1, N R2, N R3) FromMass<N>(N mass, N r1, N r2, N r3)
            where N : INumberBase<N>
        {
            N fifth = mass / N.CreateChecked(5);
            return (fifth * (r2 * r2 + r3 * r3), fifth * (r1 * r1 + r3 * r3), fifth * (r1 * r1 + r2 * r2));
        }

        /// <summary> Moments of inertia of an ellipsoid of the density and the semi-axes. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (N R1, N R2, N R3) FromDensity<N>(N density, N r1, N r2, N r3)
            where N : IFloatingPointConstants<N>
            => FromMass(Ellipsoid.Region.Mass.FromDensity(density, r1, r2, r3), r1, r2, r3);
    }
}
